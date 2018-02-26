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
            DragMove();
        }

        private void img_next_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string username = tbx_username.Text;
            string password = passbox.Password;
            client.Connect(new IPEndPoint(IPAddress.Any, 7777));
            Rooms rm = new Rooms();            
            rm.Show();
            Close();

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
