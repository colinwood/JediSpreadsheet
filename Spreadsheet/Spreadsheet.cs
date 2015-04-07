using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    /// <summary>
    /// Namgi Yoon u0759547
    /// let's make a spreadsheet!!
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        // there is empty sheet 
        private Dictionary<String, Cell> sheet = new Dictionary<string, Cell>();
        // to keep track of the relationships among spreasheet cells
        private DependencyGraph DG = new DependencyGraph();
        

        /// <summary>
        /// It should create an empty spreadsheet that imposes no extra validity conditions, 
        /// normalizes every cell name to itself, and has version "default"
        /// PS5
        /// </summary>
        public Spreadsheet()
            : base(t => true, t => t, "default")
        {          
        }

        /// <summary>
        /// Just like the zero-argument constructor, it should create an empty spreadsheet. 
        /// However, it should allow the user to provide a validity delegate (first parameter),
        /// a normalization delegate (second parameter), and a version (third parameter).
        /// PS5
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
        }
              
        /// <summary>
        ///  It should allow the user to provide a string representing a path to a file (first parameter),
        /// a validity delegate (second parameter), a normalization delegate (third parameter), and a version (fourth parameter).
        /// It should read a saved spreadsheet from a file (see the Save method) and use it to construct a new spreadsheet. 
        /// The new spreadsheet should use the provided validity delegate, normalization delegate, and version.
        /// PS5
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(String filename, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            if (!Version.Equals(GetSavedVersion(filename))) { throw new SpreadsheetReadWriteException("The version is not equal"); }

            //initialize two local variables to avoid the error.
            string currentName = "";
            string currentContent = "";

            try
            {
                using (XmlReader xr = XmlReader.Create(filename))
                {
                    while (xr.Read())
                    {
                        if (xr.IsStartElement())
                        {
                            if (xr.Name.Equals("name"))
                            {
                                xr.Read();  // for getting next node.
                                currentName = xr.Value;
                                if (!IsValid(currentName)) { throw new SpreadsheetReadWriteException("Invalid cell name"); }
                            }
                            else if (xr.Name.Equals("contents"))
                            {
                                xr.Read();  // for getting next node.
                                currentContent = xr.Value;
                                SetContentsOfCell(currentName, currentContent);
                                continue; 
                            }                          
                        }
                    }
                    Changed = false;
                }
            }
            catch (SpreadsheetReadWriteException e) { throw new SpreadsheetReadWriteException(e.Message); }
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// sheet keys are cell names, so using foreach loop store if the name exist in sheet.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            // first check all the keys and they are not empty than add into set of cellName.
            HashSet<string> setOfCellName = new HashSet<string>();
            foreach (String s in sheet.Keys)
            {
                if (!sheet[s].getContent().Equals(""))
                    setOfCellName.Add(s);
            }

            return setOfCellName;
        }

        /// <summary> 
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// First task of this method is checking parameter name is valid or not.
        /// As we know valid name starts with an underscore or a letter 
        /// and remaining characters are underscores and/or letters and/or digits.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellContents(string name)
        {
            // check it out // modified at PS5
            if (name == null) { throw new InvalidNameException(); }
            // In PS4, I used regax for check the name's validation.
            // However, we have delegate 'IsValid' function. 
            // we can check the name using the delegate function
            if (!IsValid(name)) { throw new InvalidNameException(); }

            // we have to normalize the name after checking all exceptions.
            string normalizedName = Normalize(name);
                       
            // Now, we know the name is in sheet, so get cell contents.
            if (sheet.ContainsKey(normalizedName)) { return sheet[normalizedName].getContent(); }
            else return ""; //empty cell contents
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1 
        /// 
        /// </summary>
        /// <param name="name">cell name</param>
        /// <returns></returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            // check it out, always.
            //if (name == null) { throw new ArgumentNullException(); }

            // In PS4, I used regax for check the name's validation.
            // However, we have delegate 'IsValid' function. 
            // we can check the name using the delegate function
            //if (!IsValid(name)) { throw new InvalidNameException(); }

            // we have to normalize the name after checking all exceptions.
            string normalizedName = Normalize(name);

            // Now, we can get all dependees form PS2 DependencyGraph, so I declare DG
            IEnumerable<String> tempDG = DG.GetDependees(name);
            HashSet<String> dircDependents = new HashSet<string>(tempDG);
            return dircDependents;

        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// </summary>
        /// <param name="name">cell name</param>
        /// <param name="number">content is double type number</param>
        /// <returns></returns>
        protected override ISet<string> SetCellContents(string name, double number)
        {                        
            // In PS4, I used regax for check the name's validation.
            // However, we have delegate 'IsValid' function. 
            // we can check the name using the delegate function
            //if (name == null) { throw new InvalidNameException(); }
            //if (!IsValid(name)) { throw new InvalidNameException(); }

            // we have to normalize the name after checking all exceptions.
            //string normalizedName = Normalize(name);

            // prepare the empty Iset
            HashSet<string> result = new HashSet<string>();

            // input the empty hashset
            DG.ReplaceDependents(name, new HashSet<string>());
            // the name's DG is empty now, so need to recalculate it.
            result = new HashSet<string>(GetCellsToRecalculate(name));          

            // Now, set the number in the sheet.
            // Important thing is dependencyGraph.. keep track it!
            if (sheet.ContainsKey(name))
            {
                sheet.Remove(name);
                Cell c = new Cell(name, number, lookup);
                sheet.Add(name, c);
            }
            else // if not remove previous content in the cellname and add new contents.
            {
                Cell c = new Cell(name, number, lookup);
                sheet.Add(name, c);
            }

            //update if the cell has a formula!
            foreach (String s in result)
            {
                if (sheet.ContainsKey(s))
                {
                    if (sheet[s].getContent() is Formula)
                    {
                        //For re-Evaluate
                        Cell c = new Cell(s, sheet[s].getContent(), lookup);
                        sheet.Remove(s);
                        sheet.Add(s, c);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// </summary>
        /// <param name="name">cellname</param>
        /// <param name="text">content is string type text</param>
        /// <returns></returns>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            // check check check
            //if (text == null) { throw new ArgumentNullException(); }
            //if (name == null) { throw new InvalidNameException(); }
            // In PS4, I used regax for check the name's validation.
            // However, we have delegate 'IsValid' function. 
            // we can check the name using the delegate function
            //if (!IsValid(name)) { throw new InvalidNameException(); }           

            // empty text case
            if (text == "")
            {
                if (sheet.ContainsKey(name))
                {
                    sheet.Remove(name);
                    DG.ReplaceDependents(name, new HashSet<string>());
                }
                return new HashSet<string>(GetCellsToRecalculate(name));
            }                       

            // prepare the empty Iset
            HashSet<string> result = new HashSet<string>();

            // input the empty hashset
            DG.ReplaceDependents(name, new HashSet<string>());
            // the name's DG is empty now, so need to recalculate it.
            result = new HashSet<string>(GetCellsToRecalculate(name));

            // Now, set the number in the sheet.
            // Important thing is dependencyGraph.. keep track it!
            if (sheet.ContainsKey(name))
            {
                // and the remove the cellcontent.
                sheet.Remove(name);
                Cell c = new Cell(name, text, lookup);
                sheet.Add(name, c);
            }
            else // if not remove previous content in the cellname and add new contents.
            {
                Cell c = new Cell(name, text, lookup);
                sheet.Add(name, c);
            }

            return result;
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <param name="name">cellname</param>
        /// <param name="formula">content is formula</param>
        /// <returns></returns>
        protected override ISet<string> SetCellContents(string name, SpreadsheetUtilities.Formula formula)
        {
            // check check check
            //if (ReferenceEquals(formula, null)) { throw new ArgumentNullException(); }
            //if (name == null) { throw new InvalidNameException(); }
            //if (!IsValid(name)) { throw new InvalidNameException(); }

            // initialized result and tempDependent
            HashSet<string> result = new HashSet<string>();
            // store the name's Dependents in temporary hashset to prepare the circular dependency case
            HashSet<string> tempDependents = new HashSet<string>(DG.GetDependents(name));

            // this try-catch statement will check circular dependency
            try
            {   
                // input the empty hashset for dependent.
                if (sheet.ContainsKey(name))
                    DG.ReplaceDependents(name, new HashSet<string>());
                // we have to return all veriables like set {A1, B1, C1} and store into DG
                foreach (String s in formula.GetVariables()) { DG.AddDependency(name, s); }
                //recalculate the cellname's contents
                result = new HashSet<string>(GetCellsToRecalculate(name));
            }
            catch (CircularException e)
            {
                //just keep tracking previous dependents..
                DG.ReplaceDependents(name, tempDependents);
                throw e;
            }

            // Now, set the sheet!
            if (sheet.ContainsKey(name))
            {
                sheet.Remove(name);     //make sure
                sheet[name] = new Cell(name, formula, lookup);
            }
            else
            {
                sheet.Add(name, new Cell(name, formula, lookup));
            }

            //update if the cell has a formula!
            foreach (String s in result)
            {
                if (sheet.ContainsKey(s) && sheet[s].getContent() is Formula)
                {
                    //For re-Evaluate                   
                    Cell c = new Cell(s, sheet[s].getContent(), lookup);
                    sheet.Remove(s);
                    sheet.Add(s, c);
                }// exception handle??
            }

            return result;
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            // All kinda exception cases
            if (content == null) { throw new ArgumentNullException(); }          
            if (name == null || !IsValid(name)) { throw new InvalidNameException(); }

            // normalize the name for all of type setcellcontents methods
            String normalizedName = Normalize(name);

            // Otherwise, if content parses as a double, the contents of the named
            // cell becomes that double.
            Double number = 0;

            if (Double.TryParse(content, out number))
            {
                Changed = true;
                return SetCellContents(normalizedName, number);
            }
            else if (content.StartsWith("="))
            {
                // formula is already handling the three issues.
                Changed = true;
                Formula f = new Formula(content.Substring(1), Normalize, IsValid);               
                return SetCellContents(normalizedName, f);
            }
            // just string.
            else
            {
                Changed = true;
                return SetCellContents(normalizedName, content);
            }
        } 

        /// <summary>
        /// helper method for the cell contains double type content.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private double lookup(string name)
        {              
            if (!sheet.ContainsKey(name)) { throw new ArgumentException(); }        
            
            // temp cell
            Cell newCell = sheet[name];

            if (newCell.getValue() is double)
                return (double)newCell.getValue();
            else
                throw new ArgumentException();
            
        }

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message./// 
        /// </summary>
        /// <param name="filename"></param>
        public override void Save(string filename)
        {
            // exception case
            if (filename == null || filename == "") { throw new InvalidNameException(); }

            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    foreach (String s in this.GetNamesOfAllNonemptyCells())
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", s);
                        object judge = sheet[s].getContent();
                        if (judge is Formula) { writer.WriteElementString("contents", "=" + judge.ToString()); }
                        else writer.WriteElementString("contents", judge.ToString());

                        writer.WriteEndElement();      //end cell
                    }
                    writer.WriteEndElement();      //end spreadsheet                                      
                    writer.WriteEndDocument();
                    writer.Close();
                }
                Changed = false;
            }
            catch (Exception e) { throw new SpreadsheetReadWriteException("can't save: " + e.Message); }
        }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public override string GetSavedVersion(string filename)
        {
            if (filename == null || filename == "") { throw new InvalidNameException(); }

            String fileVersion = "";

            try
            {
                using (XmlReader xr = XmlReader.Create(filename))
                {

                    while (xr.Read())
                    {
                        if (xr.IsStartElement())
                        {
                            if (xr.Name.Equals("spreadsheet"))
                            {
                                fileVersion = xr.GetAttribute("version");
                                break;
                            }
                                
                        }
                    }//end while

                }//end using

                return fileVersion;
            }
            catch (Exception e) { throw new SpreadsheetReadWriteException(e.Message); }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellValue(string name)
        {
            // If name is null or invalid, throws an InvalidNameException.
            if (!IsValid(name) || name == null) { throw new InvalidNameException(); }

            // normalize 
            String normalizedName = Normalize(name);

            // check the key
            if (sheet.ContainsKey(normalizedName)) { return sheet[name].getValue();}
            else
                return "";
        }
    }
}
