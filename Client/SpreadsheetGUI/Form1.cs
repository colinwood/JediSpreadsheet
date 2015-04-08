using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using CustomNetworking;

namespace SpreadsheetGUI
{
    /// <summary>
    /// New GUI!!
    /// </summary>
    public partial class Form1 : Form
    {



        public static void main()
        {

            Spreadsheet g = new Spreadsheet();


            createSocket("155.98.111.62", 2120);


        }
        // create my spreadsheet
        private Spreadsheet myspreadsheet;
        // tracking filename
        private String recentName;
        // this is for cancel
        private Boolean seeyou = true;
        private static StringSocket socket;
        private Timer timer;
        private bool timeStart;


        /// <summary>
        ///  basic form of myspreadsheet
        /// </summary>
        public Form1()
        {

            timer = new Timer();
            timer.Interval = 30000;
            timer.Tick += timerFired;
            timeStart = false;
            // initialize my spreadsheet
            InitializeComponent();

            // The SelectionChanged event is declared with a
            // delegate that specifies that all methods that register with it must
            // take a SpreadsheetPanel as its parameter and return nothing.  So we
            // register the displaySelection method below.
            spreadsheetPanel1.SelectionChanged += displaySelection;
            spreadsheetPanel1.SetSelection(0, 0);

            // A1 in the upper left and Z99 in the lower right. lower case to upper case and version is ps6
            string patt = @"^[A-Z][1-9][0-9]?$";
            myspreadsheet = new Spreadsheet(s => Regex.IsMatch(s, patt), s => s.ToUpper(), "ps6");
            this.Text = "MySpreadSheet";

        }

        /// <summary>
        ///  this constractor for open tab.
        /// </summary>
        /// <param name="filename"></param>
        public Form1(String filename)
        {
            InitializeComponent();
            spreadsheetPanel1.SelectionChanged += displaySelection;
            spreadsheetPanel1.SetSelection(0, 0);

            string patt = @"^[A-Z][1-9][0-9]?$";
            myspreadsheet = new Spreadsheet(filename, s => Regex.IsMatch(s, patt), s => s.ToUpper(), "ps6");
            this.Text = filename;
        }

        public static bool createSocket(string ip, int port)
        {
            try
            {
                TcpClient spreadsheetClient = new TcpClient(ip, port);
                socket = new StringSocket(spreadsheetClient.Client, ASCIIEncoding.Default);
                return true;
            }

            catch (SocketException e)
            {
                return false;
            }
            // return false;
        }

        private void timerFired(object send, EventArgs e)
        {
            SaveSend();
        }


        private void receiveCallback(string s, Exception e, object payload)
        {


            //socket.BeginReceive();
            //check the commands from the server

            if (s != null)
            {
                //if (s=="ERROR")



            }
        }


        // what else ? ?
        public void UndoSend()
        {
            socket.BeginSend("UNDO " + "\n", (e, o) => { }, null);
            socket.BeginReceive(receiveCallback, null);

        }


        //fires the send every 30 secs i guess.....fack it's getting tedious lol xD 
        public void SaveSend()
        {

            //todo: check the protocol. what does the save command again ??
            socket.BeginSend("SAVE" + "\n", (e, o) => { }, null);
            socket.BeginReceive(receiveCallback, null);
        }
        /// <summary>
        ///  Every time the selection changes, this method is called with the
        ///  Spreadsheet as its parameter.  We display the current time in the cell.
        /// </summary>
        /// <param name="sender"></param>
        private void displaySelection(SpreadsheetPanel sender)
        {
            int row, col;
            String value;
            sender.GetSelection(out col, out row);
            sender.GetValue(col, row, out value);

            // set the cellname on my spreadsheet and result value of cell content 
            CellName.Text = convertToCellName(col, row);
            CellValue.Text = myspreadsheet.GetCellValue(CellName.Text).ToString();
            // if the cell content is formula, then compute it.
            Object tempcontent = myspreadsheet.GetCellContents(CellName.Text);

            if (tempcontent is Formula)
            {
                CellContent.Text = "=" + tempcontent;
            }
            else
            {
                CellContent.Text = tempcontent.ToString();
            }

        }

        /// <summary>
        ///  helper method to convert (col, row) to cellname
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private string convertToCellName(int col, int row)
        {
            return "" + ((char)(col + 65)) + (row + 1);
        }

        /// <summary>
        ///  helper method to convert cellname to (col, row)
        /// </summary>
        /// <param name="cellname"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void convertToColRow(string cellname, out int col, out int row)
        {
            //the cell name consisted of one letter and one number.
            string[] temp = new string[2];
            temp[0] = cellname.Substring(0, 1);     // Store the letter
            temp[1] = cellname.Substring(1);        // Store the number
            int a = Convert.ToInt32(temp[1]);       // Now, the number is string type so covert it as integer.
            row = a - 1;                            // cell name start with A1, not A0
            col = temp[0][0] - 65;                  // unicode.
        }

        /// <summary>
        ///  Deals with the New menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyApplicationContext.getAppContext().RunForm(new Form1());
        }

        /// <summary>
        ///  Deals with the Save menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //first saving the file
            if (recentName == null)
            {
                SaveFileDialog saveButton = new SaveFileDialog();
                saveButton.Filter = "Sprd Document (*.sprd)|*.sprd|All files(*.*)|*.*";
                saveButton.Title = "Save";
                saveButton.InitialDirectory = @"C:\";

                if (saveButton.ShowDialog() == DialogResult.OK)
                {
                    string filename = saveButton.FileName;
                    if (saveButton.FilterIndex == 1)
                        saveButton.AddExtension = true;
                    else
                        saveButton.AddExtension = false;
                    recentName = filename;
                    myspreadsheet.Save(filename);
                    //changing the title name
                    this.Text = saveButton.FileName;
                }
                else
                {
                    if (sender.ToString() == "Close")
                    {
                        seeyou = false;
                    }
                }
            }
            else
            {
                myspreadsheet.Save(recentName);   //second or more.
            }
        }

        /// <summary>
        ///  Deal with the Save As menu, only difference is you do not check first save or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Sprd Document (*.sprd)|*.sprd|All files(*.*)|*.*";
            saveDialog.Title = "Save As";
            saveDialog.InitialDirectory = @"C:\";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveDialog.FileName;
                if (saveDialog.FilterIndex == 1)
                    saveDialog.AddExtension = true;
                else
                    saveDialog.AddExtension = false;
                recentName = filename;
                myspreadsheet.Save(filename);
                this.Text = saveDialog.FileName;
            }
            else
            {
                if (sender.ToString() == "Close")
                {
                    seeyou = false;
                }
            }
        }

        /// <summary>
        ///  Deals with the Open menu
        ///  This will handle existed spreadsheet. set all cell contents and values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // pop up the open file dialog form exsising DLL.
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Sprd Document (*.sprd)|*.sprd|All files(*.*)|*.*";
            openDialog.Title = "0pen";
            openDialog.InitialDirectory = @"C:\";
            int col, row;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                //find the path
                string filename = openDialog.FileName;
                Form1 openedForm = new Form1(filename);
                openedForm.Text = filename;
                openedForm.recentName = filename;
                MyApplicationContext.getAppContext().RunForm(openedForm);

                try
                {
                    //find non-empty cells and save
                    foreach (string cellname in openedForm.myspreadsheet.GetNamesOfAllNonemptyCells())
                    {
                        convertToColRow(cellname, out col, out row);
                        openedForm.spreadsheetPanel1.SetValue(col, row, openedForm.myspreadsheet.GetCellValue(cellname).ToString());
                        openedForm.spreadsheetPanel1.SetSelection(col, row);
                        openedForm.displaySelection(spreadsheetPanel1);
                    }

                }
                catch (Exception er)
                {
                    MessageBox.Show(er.Message, "Error");
                    openedForm.Close();
                }
            }

        }

        /// <summary>
        ///  Deals with the Close menu
        ///  using seeyou variable, checking the spreadsheet is closed or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // nothing changed
            if (!myspreadsheet.Changed)
                Close();
            else
            {
                DialogResult saveDialog = MessageBox.Show("Want to Save your changes?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (saveDialog == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(sender, e);
                    if (seeyou)
                        Close();
                    else
                        seeyou = false;
                }
                else if (saveDialog == DialogResult.No)
                {
                    Close();
                }
                else if (saveDialog == DialogResult.Cancel)
                {
                    if (e is FormClosingEventArgs)
                        ((FormClosingEventArgs)e).Cancel = true;
                }
            }
        }

        /// <summary>
        ///  This method will put the content into the spreadsheet and evaluate if it is formula type. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Evaluation_Click(object sender, EventArgs e)
        {
            int row, col;
            spreadsheetPanel1.GetSelection(out col, out row);

            //handle any exceptions about setting the contents
            try
            {
                myspreadsheet.SetContentsOfCell(convertToCellName(col, row), CellContent.Text);
            }
            catch (FormulaFormatException error1)
            {
                MessageBox.Show(error1.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (CircularException error2)
            {
                MessageBox.Show(error2.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //get the value from my spreadsheet.
            CellValue.Text = myspreadsheet.GetCellValue(convertToCellName(col, row)).ToString();

            //loop to find out non-empty cells, then show it upto my spreadsheet
            foreach (String c_name in myspreadsheet.GetNamesOfAllNonemptyCells())
            {
                convertToColRow(c_name, out col, out row);    //now, col and row are changed.
                try
                {
                    spreadsheetPanel1.SetValue(col, row, myspreadsheet.GetCellValue(c_name).ToString());    //show it upto my spreadsheet
                }
                catch (Exception error3)
                {
                    MessageBox.Show(error3.Message, "Error");
                }
            }
        }

        /// <summary>
        ///  just help tab 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialog =
              MessageBox.Show("This spreadsheet imitated the Microsoft-Excel program.\n\t-googling 'how to use Excel'\n\t-this application is not perfect\nbasic rule: \n\t 1. selects one cell \n\t 2. types anything into content text box \n\t 3. Evaluation is a magic button, click it", "How to use PS6",
              MessageBoxButtons.OK,
              MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// just help tab 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void techinalSupportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialog =
              MessageBox.Show("http://stackoverflow.com/", "Reference",
              MessageBoxButtons.OK,
              MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// just help tab 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutMySpreadSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialog =
                MessageBox.Show("Creator: Namgi Yoon\nVersion: ps6\nAll rights reserved by Jim", "Information of PS6",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
        }

        /// <summary>
        ///  almost same with close action. However, this is slighty different.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // the spread sheet is changed so confirm save or not.
            if (myspreadsheet.Changed)
            {
                DialogResult dialog = MessageBox.Show("Your SpreadSheet is changed, but you did not save it, Do you want to save changed?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(sender, e);
                    if (!seeyou)
                        seeyou = !seeyou;
                }
                else if (dialog == DialogResult.Cancel)
                {
                    if (e is FormClosingEventArgs)
                        ((FormClosingEventArgs)e).Cancel = true;

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void CellContent_TextChanged(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void connectionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //connect
            Button b = (Button)sender;
            createSocket(IP.Text, Convert.ToInt32(textBox2.Text));
            UndoSend();
            usetheForce();


        }

        public void usetheForce()
        {
            socket.BeginSend("USE THE FORCE " + "\n", (e, o) => { }, null);
            socket.BeginReceive(receiveCallback, null);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }





    }
}
