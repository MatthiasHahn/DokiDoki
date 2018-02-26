using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DokiDoki_Login_Server
{
    class Program
    {
        public static string conn;
        public static MySqlConnection connect;

        static void Main(string[] args)
        {
            dbconnect();
            Console.Write("Connected");
            Console.ReadKey();

        }

        private static void dbconnect()
        {
            conn = "Server=localhost;Database=DokiDoki;Uid=root;Pwd=;";
            connect = new MySqlConnection(conn);
            connect.Open();
        }

        private bool validate_login(string user, string pass)
        {
            dbconnect();
            string password = pass.GetHashCode().ToString();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "Select * from users where username=@user and password=@pass";
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@pass", password);
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