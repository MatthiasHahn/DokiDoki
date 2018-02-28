﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Net.Sockets;
using System.Net;

namespace DokiDoki
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TcpClient client = new TcpClient();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void wnd_main_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Fix this shit please --> DragMove();
        }

        private void img_next_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbx_username.Text == "" || tbx_username.Text.Length <= 4 || passbox.Password == "" || passbox.Password.Length <= 4)
            {
                MessageBox.Show("Please enter valid information","Error",MessageBoxButton.OK,MessageBoxImage.Error,MessageBoxResult.OK,MessageBoxOptions.DefaultDesktopOnly); 
            }

            else
            {
                client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7777));

                string salt = "MP1Sfss==";
                string username = tbx_username.Text;
                string passwordNoP = passbox.Password.GetHashCode().ToString();
                string pepper = username.Substring(username.Length - 4) + passwordNoP.Substring(passwordNoP.Length - 4);
                string password = Convert.ToBase64String(Encoding.UTF8.GetBytes((salt + passwordNoP + pepper).GetHashCode().ToString()));

                Byte[] data = Encoding.ASCII.GetBytes(username + ";" + password);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                data = new Byte[256];
                string responsedata = string.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responsedata = Encoding.ASCII.GetString(data, 0, bytes);

                stream.Close();
                client.Close();

                bool trfr;
                if (responsedata == "1")
                {
                    trfr = true;
                }
                else
                {
                    trfr = false;
                }
                if (trfr)
                {
                    Rooms rm = new Rooms();
                    rm.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Please check your username or password","Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                    client = new TcpClient();
                }
            }
           

        }

      private String CreateSalt(int size)
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buffer = new byte[size];
            rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        private void img_next_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.4, 0.6, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(0.2));
            img_next.BeginAnimation(OpacityProperty, anim);
            lbl_deko_next.BeginAnimation(OpacityProperty, anim2);
        }

        private void img_next_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.6, 0.4, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(1, 0.8, TimeSpan.FromSeconds(0.2));
            img_next.BeginAnimation(OpacityProperty, anim);
            lbl_deko_next.BeginAnimation(OpacityProperty, anim2);
        }
    }
}
