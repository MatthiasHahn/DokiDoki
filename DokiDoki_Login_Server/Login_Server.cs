using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
//using MySql.Data;
//using MySql.Data.MySqlClient;

namespace DokiDoki_Login_Server
{
    class Login_Server
    {
        public static string conn;
        public static MySqlConnection connect;
        static TcpListener listen = null;

        static void Main(string[] args)
        {
            dbconnect();
            Console.WriteLine("Login Server Connected");

            listen = new TcpListener(IPAddress.Loopback, 7777);
            listen.Start();

            Byte[] bytes = new Byte[1];
            string data = null;

            while (true)
            {
                Console.WriteLine("Waiting for a connection ...");
                TcpClient client = listen.AcceptTcpClient();
                Console.WriteLine("Client connected");
                data = null;
                NetworkStream stream = client.GetStream();
                int i;
                bool login;

                stream.Read(bytes, 0, 1);
                if (bytes[0] == new byte[] { 0 }[0])
                    login = true;
                else
                    login = false;

                bytes = new Byte[256];
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine(data);
                    string user = data.Split(';')[0];
                    string pass = data.Split(';')[1];
                    string email = login ? "" : data.Split(';')[2];
                    bool valid = login ? validate_login(user, pass) : validate_register(user, email); ;
                    if(!login)
                        Create_User(user, pass, email);
                    if (valid)
                    {
                        byte[] msg = Encoding.ASCII.GetBytes("1");
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine(login ? "Message Valid" : "User created");

                    }

                    else
                    {
                        byte[] msg = Encoding.ASCII.GetBytes("0");
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine(login ? "Message UnValid" : "User already exists");
                    }
                }
            }

        }

        private static void Create_User(string user, string pass, string email)
        {
            dbconnect();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = string.Format("INSERT INTO `users` (`id`, `username`, `password`, `email`) VALUES (NULL, '{0}', '{1}', '{2}')", user, pass, email);
            cmd.Connection = connect;
            cmd.ExecuteNonQuery();
            connect.Close();
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

        private static bool validate_register(string user, string email)
        {
            dbconnect();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "Select * from users where username=@user or email=@email";
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Connection = connect;
            MySqlDataReader login = cmd.ExecuteReader();



            if (login.HasRows)
            {
                connect.Close();
                return false;
            }
            else
            {
                connect.Close();
                return true;
            }
        }
    }
}