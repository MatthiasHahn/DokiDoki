using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Mail;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DokiDoki_Register_Server
{
    class Register_Server
    {
        public static string conn;
        public static MySqlConnection connect;
        static TcpListener listen = null;
        static void Main(string[] args)
        {
            dbconnect();
            Console.WriteLine("Register Server Connected");

            listen = new TcpListener(IPAddress.Loopback, 8888);
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
                    string user = data.Split(';')[0];
                    string pass = data.Split(';')[1];
                    string email = data.Split(';')[2];
                    bool valid = validate_register(user,email);

                    string tr = "1";
                    string fl = "0";

                    if (valid)
                    {
                        Random rnd = new Random();
                        int code = rnd.Next(100000,999999);
                        Create_User(user,pass,email);
                        byte[] msg = Encoding.ASCII.GetBytes(tr+";"+code);
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("User Created");

                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                        mail.From = new MailAddress("noreply.dokidoki@gmail.com");
                        mail.To.Add(email);
                        mail.Subject = "Your Code [Doki Doki]";
                        mail.Body = "Here is your code: " + code.ToString();

                        SmtpServer.Port = 25;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("noreply.dokidoki@gmail.com", "noreplydokidoki");
                        SmtpServer.EnableSsl = true;

                        SmtpServer.Send(mail);

                    }

                    else
                    {
                        byte[] msg = Encoding.ASCII.GetBytes(fl);
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("User Already Exists");
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

        private static bool validate_register(string user,string email)
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

        private static void Create_User(string user,string pass, string email)
        {
            dbconnect();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = string.Format("INSERT INTO `users` (`id`, `username`, `password`, `email`) VALUES (NULL, '{0}', '{1}', '{2}')",user,pass,email);
            cmd.Connection = connect;
            cmd.ExecuteNonQuery();
            connect.Close();



 
        }
    
    }
}
