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
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Net;
using System.Net.Sockets;

namespace DokiDoki
{
    /// <summary>
    /// Interaktionslogik für Email_Verify.xaml
    /// </summary>
    public partial class Email_Verify : Window
    {
        TcpClient client = new TcpClient();
        public int code;
        public string email;
        public Email_Verify()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
           
        }

        

        private void wnd_email_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

        }
        private void img_next_em_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.4, 0.6, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(0.2));

            img_next_em.BeginAnimation(OpacityProperty, anim);
            lbl_deko_next_em.BeginAnimation(OpacityProperty, anim2);
        }

        private void img_next_em_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.6, 0.4, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(1, 0.8, TimeSpan.FromSeconds(0.2));
            img_next_em.BeginAnimation(OpacityProperty, anim);
            lbl_deko_next_em.BeginAnimation(OpacityProperty, anim2);
            
        }

        private void img_back_em_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.4, 0.6, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(0.2));
            img_back_em.BeginAnimation(OpacityProperty, anim);
            lbl_deko_back_em.BeginAnimation(OpacityProperty, anim2);
        }

        private void img_back_em_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.6, 0.4, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(1, 0.8, TimeSpan.FromSeconds(0.2));
            img_back_em.BeginAnimation(OpacityProperty, anim);
            lbl_deko_back_em.BeginAnimation(OpacityProperty, anim2);
        }
        private void img_back_em_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Register rg = new Register();
            rg.Show();
            Close();
        }

        private void img_next_em_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbx_username.Text == "")
            {
                lbl_error_disp_em.Content = "Please Enter Your Code";
                DoubleAnimation anim = new DoubleAnimation(1, TimeSpan.FromSeconds(0.4));
                DoubleAnimation anim2 = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
                anim.Completed += new EventHandler((object sender2, EventArgs e2) =>
                {
                    lbl_error_disp_em.BeginAnimation(OpacityProperty, anim2);
                });
                lbl_error_disp_em.BeginAnimation(OpacityProperty, anim);
            }

            else
            {



                if (code == Convert.ToInt32(tbx_username.Text))
                {
                    client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1010));

                    
                    Byte[] data = Encoding.ASCII.GetBytes(email);
                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);

                    MainWindow mw = new MainWindow();
                    mw.Show();
                    Close();
                }
                else
                {
                    lbl_error_disp_em.Content = "Wrong Code";
                    DoubleAnimation anim = new DoubleAnimation(1, TimeSpan.FromSeconds(0.4));
                    DoubleAnimation anim2 = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
                    anim.Completed += new EventHandler((object sender2, EventArgs e2) =>
                    {
                        lbl_error_disp_em.BeginAnimation(OpacityProperty, anim2);
                    });
                    lbl_error_disp_em.BeginAnimation(OpacityProperty, anim);
                }
            }
        }


        
        }
}
