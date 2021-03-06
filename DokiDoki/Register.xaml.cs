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
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net;

namespace DokiDoki
{
    /// <summary>
    /// Interaktionslogik für Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        
        public  int code;  
        public Register()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        
        private void wnd_register_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            
        }

        private void img_next_reg_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.4, 0.6, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(0.2));
            img_next_reg.BeginAnimation(OpacityProperty, anim);
            lbl_deko_next_reg.BeginAnimation(OpacityProperty, anim2);
        }

        private void img_next_reg_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.6, 0.4, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(1, 0.8, TimeSpan.FromSeconds(0.2));
            img_next_reg.BeginAnimation(OpacityProperty, anim);
            lbl_deko_next_reg.BeginAnimation(OpacityProperty, anim2);
        }

        private void img_back_reg_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.4, 0.6, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(0.2));
            img_back_reg.BeginAnimation(OpacityProperty, anim);
            lbl_deko_back_reg.BeginAnimation(OpacityProperty, anim2);
        }

        private void img_back_reg_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.6, 0.4, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(1, 0.8, TimeSpan.FromSeconds(0.2));
            img_back_reg.BeginAnimation(OpacityProperty, anim);
            lbl_deko_back_reg.BeginAnimation(OpacityProperty, anim2);
        }
        private void img_back_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            Close();
        }

        private void img_next_reg_MouseDown(object sender, MouseButtonEventArgs e)
        {


            bool swear = BasicInputCheck(tbx_username_reg, passbox_reg, tbx_email);
            if (!swear)
            {                
                DoubleAnimation anim = new DoubleAnimation(1, TimeSpan.FromSeconds(0.4));
                DoubleAnimation anim2 = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
                anim.Completed += new EventHandler((object sender2, EventArgs e2) => {
                    lbl_error_disp_reg.BeginAnimation(OpacityProperty, anim2);
                });
                lbl_error_disp_reg.BeginAnimation(OpacityProperty, anim);
            }

            else
            {
                TcpClient client = new TcpClient();
                client.Connect(new IPEndPoint(IPAddress.Loopback, 7777)); //IP_CHG 192.168.1.1
                bool servervalid;
                string salt = "MP1Sfss==";
                string username = tbx_username_reg.Text;
                string email = tbx_email.Text;
                string passwordNoP = passbox_reg.Password.GetHashCode().ToString();
                string pepper = username.Substring(username.Length - 4) + passwordNoP.Substring(passwordNoP.Length - 4);
                string password = Convert.ToBase64String(Encoding.UTF8.GetBytes((salt + passwordNoP + pepper).GetHashCode().ToString()));

                Byte[] data = Encoding.ASCII.GetBytes(username + ";" + password + ";" + email);
                NetworkStream stream = client.GetStream();
                stream.Write(new byte[] { 1 }, 0, 1);
                stream.Flush();

                stream.Write(data, 0, data.Length);
                data = new Byte[256];
                string responsedata = string.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responsedata = Encoding.ASCII.GetString(data, 0, bytes);

                string trfl = responsedata.Split(';')[0];
                if (trfl == "1")
                {
                    code = Convert.ToInt32(responsedata.Split(';')[1]);
                }



                stream.Close();
                client.Close();

                if (trfl == "1")
                {
                    servervalid = true;
                }
                else
                {
                    servervalid = false;
                }

                if (servervalid)
                {
                    Email_Verify ev = new Email_Verify();
                    ev.code = code;
                    ev.email = email;
                    ev.Show();
                    Close();
                }

                else
                {
                    lbl_error_disp_reg.Text = "User Already Exists";
                    DoubleAnimation anim = new DoubleAnimation(1, TimeSpan.FromSeconds(0.4));
                    DoubleAnimation anim2 = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
                    anim.Completed += new EventHandler((object sender2, EventArgs e2) => {
                        lbl_error_disp_reg.BeginAnimation(OpacityProperty, anim2);
                    });
                    lbl_error_disp_reg.BeginAnimation(OpacityProperty, anim);
                    client = new TcpClient();
                }
            }

            
        }

        private bool BasicInputCheck(TextBox username,PasswordBox password,TextBox email)
        {

           
            string usernameT = username.Text;
            string passwordT = password.Password;
            string emailT = email.Text;
            if(usernameT =="" || passwordT == "" || emailT == "" )
            {
                lbl_error_disp_reg.Text = "Invalid Empty Input";
                return false;
            }

            else if(usernameT.Length <=4)
            {
                lbl_error_disp_reg.Text = "Username Too Short";
                return false;
            }

            else if(passwordT.Length <=8)
            {
                lbl_error_disp_reg.Text = "Password Too Short";
                return false;
            }
            else if((!passwordT.Any(c => char.IsUpper(c)) || !Regex.IsMatch(passwordT, @"\d")))
            {
                lbl_error_disp_reg.Text = "Missing Upper Character or Number in Password";
                return false;
            }
            else if(!Regex.IsMatch(emailT, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"))
            {
                lbl_error_disp_reg.Text = "E-Mail Invalid";
                return false;
            }
            return true;
           

            

            
            
            
        }
    }
}
