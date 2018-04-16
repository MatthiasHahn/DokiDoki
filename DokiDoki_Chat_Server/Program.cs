using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace DokiDoki_Chat_Server
{
    class Program
    {
        private static TcpListener tcpListener = new TcpListener(new IPEndPoint(IPAddress.Parse("192.168.1.1"), 888));
        static List<TcpClient> clnt_list = new List<TcpClient>();

        static void Main(string[] args)
        {
            tcpListener.Start();
            while (true)
            {
                var client = tcpListener.AcceptTcpClient();
                clnt_list.Add(client);
                Task.Run(() =>
                {
                    StreamReader rdr = new StreamReader(client.GetStream());
                    StreamWriter wrt2 = new StreamWriter(client.GetStream()); //For Test Only
                    while (true)
                    {
                        string msg = rdr.ReadLine();
                        Console.WriteLine(msg);
                        foreach (var clnt in clnt_list)
                            if (clnt != client)
                            {
                                using (StreamWriter wrt = new StreamWriter(clnt.GetStream(), Encoding.ASCII, 4096,
                                    true))
                                {
                                    wrt.WriteLine(msg);
                                    wrt.Flush();
                                }
                            }
                        wrt2.WriteLine("Server: Accepted");
                        wrt2.Flush();
                    }
                });
            }
        }
    }
}