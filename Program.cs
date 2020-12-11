using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace kjutcli
{
    public class KjutClient
    {

        private static int TAM = 1024;
        private static string _ip = "127.0.0.1";
        private static int _port = 11000;

        public static int Main(String[] args)
        {
            ReadServerIpPort();
            string r = null;
            r = ReadValue();
            while (r != null)
            {
                Vote(r);
                r = ReadValue();
            }
            return 0;
        }

        public static string ReadValue()
        {
            string val = null;
            Console.Write("Escribe un valor [0,1,2]: ");
            try
            {
                val = Console.ReadLine();
                int x = int.Parse(val);
                if (x < 0 || x > 2)
                {
                    val = null;
                    throw new Exception("Fuera de rango.");
                }
            }
            catch (Exception)
            {
                val = null;
                Console.WriteLine("Valor no valido. FIN.");
            }
            return val;
        }

        public static void ReadServerIpPort()
        {
            string s;
            System.Console.WriteLine("Datos del servidor: ");
            string defIp = GetFirstIpV4Address().ToString();
            System.Console.Write("Dir. IP [{0}]: ", defIp);
            s = Console.ReadLine();
            if ((s.Length > 0) && (s.Replace(".", "").Length == s.Length - 3))
            {
                _ip = s;
            }
            else
            {
                _ip = defIp;
            }
            System.Console.Write("PUERTO [{0}]: ", _port);
            s = Console.ReadLine();
            if (Int32.TryParse(s, out int i))
            {
                _port = i;
            }
        }

        private static IPAddress GetFirstIpV4Address()
        {
            List<IPAddress> ipAddressList = new List<IPAddress>();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            int t = ipHostInfo.AddressList.Length;
            string ip;
            for (int i = 0; i < t; i++)
            {
                ip = ipHostInfo.AddressList[i].ToString();
                if (ip.Contains(".") && !ip.Equals("127.0.0.1")) ipAddressList.Add(ipHostInfo.AddressList[i]);
            }
            if (ipAddressList.Count > 0)
            {
                return ipAddressList[0]; // devuelve la primera posible
            }
            return null;
        }

        public static void Vote(string r)
        {
            byte[] bytes = new byte[TAM];

            try
            {
                IPAddress ipAddress = System.Net.IPAddress.Parse(_ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, _port);
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEP);
                    //Console.WriteLine("\tConectado a {0}", sender.RemoteEndPoint.ToString());
                    byte[] msg = Encoding.ASCII.GetBytes(r);
                    int bytesSent = sender.Send(msg);
                    int bytesRec = sender.Receive(bytes);
                    //Console.WriteLine("\tConfirmado {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    // Console.WriteLine("SocketException : {0}", se.ToString());
                    Console.WriteLine("ERROR DE COMUNICACIÓN");
                    Console.WriteLine("Compruebe la conexión o ");
                    Console.WriteLine("pulse ENTER para terminar");
                    throw se;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception)//(Exception e)
            {
                //Console.WriteLine(e.ToString());
            }
        }

    }
}