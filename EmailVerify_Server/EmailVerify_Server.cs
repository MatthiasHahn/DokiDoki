using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Mail;
using MySql.Data;

namespace EmailVerify_Server
{
    class EmailVerify_Server
    {
        public static string conn;
        static TcpListener listen = null;
        public static MySql.Data.MySqlClient.MySqlConnection connect;

        static void Main(string[] args)
        {
            dbconnect();
            Console.WriteLine("Email Server Connected");
            listen = new TcpListener(IPAddress.Parse("127.0.0.1"), 1010);
            listen.Start();

            Byte[] bytes = new Byte[256];
            string data = null;

            while (true)
            {
                Console.WriteLine("Waiting for a connection ...");
                TcpClient client = listen.AcceptTcpClient();
                Console.WriteLine("Client connected");
                data = null;
                NetworkStream stream = client.GetStream();
                int i;

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine(data);
                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand();
                    cmd.CommandText = string.Format("UPDATE `users` SET `ev` = '1' WHERE `users`.`email` =\"{0}\";", data);
                    cmd.Connection = connect;
                    cmd.ExecuteNonQuery();
                    connect.Close();
                }
            }


        }

        private static void dbconnect()
        {
            conn = "Server=localhost;Database=DokiDoki;Uid=root;Pwd=;";
            connect = new MySql.Data.MySqlClient.MySqlConnection(conn);
            connect.Open();
        }
    }
}
