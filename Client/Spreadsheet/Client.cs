using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Spreadsheet
{
    public class Client
    {
        public static void StartClient()
        {
            byte[] bytes = new byte[1024];

            try
            {
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 2120);

                Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Connection!!", sender.RemoteEndPoint.ToString());

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                
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
    }
}
