using System;
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
//using MySql.Data.MySqlClient;
//using MySql.Data;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace DokiDoki
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TcpClient client = new TcpClient();
        private IPEndPoint ServerPass = new IPEndPoint(IPAddress.Loopback, 9999); //IP_CHG 192.168.1.1
        public MainWindow()
        {
            InitializeComponent();

            //Rooms rm = new Rooms(ServerPass, tbx_username.Text);
            //rm.Show();
            //Close();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void wnd_main_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ErrDisp(string msg)
        {
            lbl_error_disp.Content = msg;
            DoubleAnimation anim = new DoubleAnimation(1, TimeSpan.FromSeconds(0.4));
            DoubleAnimation anim2 = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
            anim.Completed += new EventHandler((object sender2, EventArgs e2) => {
                lbl_error_disp.BeginAnimation(OpacityProperty, anim2);
            });
            lbl_error_disp.BeginAnimation(OpacityProperty, anim);
        }
        private void img_next_MouseDown(object sender, MouseButtonEventArgs e)
        {   
            if (tbx_username.Text == "" || tbx_username.Text.Length <= 4 || passbox.Password == "" || passbox.Password.Length <= 4)
            {
                ErrDisp("Invalid Input");
            }

            else
            {
                client.Connect(new IPEndPoint(IPAddress.Loopback, 7777)); //IP_CHG 192.168.1.1

                string salt = "MP1Sfss==";
                string username = tbx_username.Text;
                string passwordNoP = passbox.Password.GetHashCode().ToString();
                string pepper = username.Substring(username.Length - 4) + passwordNoP.Substring(passwordNoP.Length - 4);
                string password = Convert.ToBase64String(Encoding.UTF8.GetBytes((salt + passwordNoP + pepper).GetHashCode().ToString()));
                passbox.Password = "";
                                
                Byte[] data = Encoding.ASCII.GetBytes(username + ";" + password);
                NetworkStream stream = client.GetStream();
                stream.Write(new byte[] { 0 },0, 1);
                stream.Flush();

                stream.Write(data, 0, data.Length);
                data = new Byte[256];
                string responsedata = string.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responsedata = Encoding.ASCII.GetString(data, 0, bytes);

                stream.Close();
                client.Close();

                string evuv = null;
                string ev = null;
                string uv = null;
                if (responsedata == "01")
                {
                    evuv = responsedata;
                }
                else if (responsedata == "11")
                {
                    ev = responsedata;
                }

                else
                {
                    uv = responsedata;
                }

                if (evuv == "01")
                {

                    Rooms rm = new Rooms(ServerPass, tbx_username.Text);
                    rm.Show();
                    Close();
                }
                else if (ev == "11")
                {
                    ErrDisp("Verify Your Email");
                    client = new TcpClient();
                }

                else
                {
                    ErrDisp("Check Your Username or Password");
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

        private void lbl_deko_noaccount_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.4, 0.6, TimeSpan.FromSeconds(0.2));
           
            lbl_deko_noaccount.BeginAnimation(OpacityProperty, anim);
        }

        private void lbl_deko_noaccount_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.6, 0.4, TimeSpan.FromSeconds(0.2));
          
            lbl_deko_noaccount.BeginAnimation(OpacityProperty, anim);
        }

        private void lbl_deko_noaccount_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Register rg = new Register();
            rg.Show();
            Close();
        }

            private void passbox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                img_next_MouseDown(null, null);
        }

        private void wnd_main_Loaded(object sender, RoutedEventArgs e)
        {
            Trans_BG_Blur.EnableBlur(this);
        }
    }
}
