using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Spreadsheet
{

    //State object for when we receive data
    public class StateObject
    {
        //Our socket
        public Socket workSocket = null;
        //Received buffer, we're assuming it's 1024 bytes.
        public byte[] buffer = new Byte[1024];
        //Received data string
        public StringBuilder sb = new StringBuilder();
    }

    public class Client
    {

        static Socket user;
        public static void StartClient(string inputIP)
        {
            byte[] bytes = new byte[1024];

            try
            {
                //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                //IPEndPoint remoteEP = new IPEndPoint(ipAddress, 2120);

                IPAddress ipAddress = IPAddress.Parse(inputIP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 2120);

                user = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    user.Connect(remoteEP);

                    Console.WriteLine("Connection!!", user.RemoteEndPoint.ToString());

                    Receive();

                    //sender.Shutdown(SocketShutdown.Both);
                    //sender.Close();
                
                }
                catch(ArgumentException ae)
                {
                    Console.WriteLine("ArgumentNullException: {0}", ae.ToString());
                }
                catch(SocketException se)
                {
                    Console.WriteLine("SocketException: {0}", se.ToString());
                }
                catch(Exception e)
                {
                    Console.WriteLine("Unexpected Exception: {0}", e.ToString());
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static int Main(String[] args)
        {
            StartClient();
            return 0;
        }

        //Filter through which all messages sent to the Server should go.
        public static void Send(String data)
        {
            byte[] converted_data = Encoding.ASCII.GetBytes(data);

            //We can just begin sending information here.
            user.BeginSend(converted_data, 0, converted_data.Length, 0, new AsyncCallback(Sent), user);
        }


        //Confirmed that a message was sent
        //Do something in response? We can debug/test for errors here.
        //Otherwise, this is only necessary to have a callback.
        public static void Sent(IAsyncResult result)
        {

        }

        public static void Receive()
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = user;

                user.BeginReceive(state.buffer, 0, 1024, 0, new AsyncCallback(Receive_callback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        //The filter through which all received messages will go.
        //From here, filter to appropriate methods to modify the Spreadsheet
        //or notify the user of Errors or Successes as necessary.
        public static void Receive_callback(IAsyncResult result)
        {
            try
            {
                StateObject state = (StateObject)result.AsyncState;

                int bytesread = user.EndReceive(result);

                //Check number of bytes read
                //If we have, there might still be data to read, so store what we've received
                //and begin receiving again.
                if (bytesread > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesread));

                    user.BeginReceive(state.buffer, 0, 1024, 0, new AsyncCallback(Receive_callback), state);
                }
                else
                {
                    //If we've reached this, we've received everything.
                    if (state.sb.Length > 1)
                    {
                        /*
                         * DO STUFF 
                         */
                    }
                }
            }
            catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }


        //Sends a Client name and a Spreadsheet name to the Server to initially connect
        //For all intents and purposes, client_name will always be 'Sysadmin'
        //to ease connection
        public static void Connect(string client_name, string spreadsheet_name)
        {
            string connect_command = "connect " + client_name + " " + spreadsheet_name;
            Send(connect_command);
        }


        /*
         * Sends a register command. 
         * According to the protocol, we should not allow usernames with '\n' or whitespace
         * Although the Error 4 occurs when a space occurs, we can just assume that it'll be prevented here.
         */
        public static void Register(string client_name)
        {
            if(client_name.Contains(' '))
            {
                /*
                 * throw "Provided username contains an illegal character (whitespace)
                 * Probably a pop-up window or something.
                 */
                return;
            }
            else if (client_name.Contains('\n'))
            {
                /*
                 * throw "Provided username contains an illegal character (newline)
                 * Probably a pop-up window or something.
                 */
                return;
            }

            //Checked if the name was valid. Since it is, send it.

            string register_command = "register " + client_name;
            Send(register_command);
        }


        //Sends a cell's contents to a particular cell, as indicated.
        //This should never be able to execute without having completed a Connect procedure.
        public static void Update(string cell_name, string cell_contents)
        {
            string cell_command = "cell " + cell_name + " " + cell_contents;

            Send(cell_command);
        }


        //Sends an Undo command to the Server.
        //This should never execute without having completed a Connect procedure.
        public static void Undo()
        {
            Send("undo");
        }
    }
}
