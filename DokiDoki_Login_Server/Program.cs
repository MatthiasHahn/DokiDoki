using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DokiDoki_Login_Server
{
    class Program
    {
        public static string conn;
        public static MySqlConnection connect;
        static TcpListener listen = null;

        static void Main(string[] args)
        {
            dbconnect();
            Console.WriteLine("Connected");

            listen = new TcpListener(IPAddress.Parse("127.0.0.1"), 7777);
            listen.Start();

            Byte[] bytes = new Byte[256];
            string data = null;

            while(true)
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
                    string user = data.Split(';')[0];
                    string pass = data.Split(';')[1];
                    bool valid = validate_login(user, pass);

                    string tr = "1";
                    string fl = "0";

                    if (valid)
                    {
                        byte[] msg = Encoding.ASCII.GetBytes(tr);
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Message Valid");

                    }

                    else
                    {
                        byte[] msg = Encoding.ASCII.GetBytes(fl);
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Message UnValid");
                    }
                }
            }

        }

        private static void dbconnect()
        {
            conn = "Server=localhost;Database=DokiDoki;Uid=root;Pwd=;";
            connect = new MySqlConnection(conn);
            connect.Open();
        }

        private static bool validate_login(string user, string pass)
        {
            dbconnect();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "Select * from users where username=@user and password=@pass";
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@pass", pass);
            cmd.Connection = connect;
            MySqlDataReader login = cmd.ExecuteReader();

            
            
            if (login.Read())
            {
                connect.Close();
                return true;
            }
            else
            {
                connect.Close();
                return false;
            }
        }
    }
}