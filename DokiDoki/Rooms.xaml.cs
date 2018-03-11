using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;

namespace DokiDoki
{
    /// <summary>
    /// Interaktionslogik für Rooms.xaml
    /// </summary>
    public partial class Rooms : Window
    {
        static IPEndPoint m = new IPEndPoint(IPAddress.Parse("224.168.55.25"), 8888);
        static IPEndPoint local = new IPEndPoint(IPAddress.Any, 8888);
        public Rooms(IPEndPoint Server)
        {
            InitializeComponent();
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.MulticastLoopback = true;
            socket.Bind(local);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(m.Address, IPAddress.Any));        
            Task.Run(() => {
                while (true)
                {
                    try
                    {
                        List<byte[]> parts = new List<byte[]>();
                        byte[] buffer = new byte[1];
                        socket.Receive(buffer);
                        var count = Convert.ToInt32(buffer[0]);
                        for(int i = 0; i < count; i++)
                        {
                            buffer = new byte[64000];
                            socket.Receive(buffer);
                            parts.Add(buffer);
                        }
                        List<byte> arr = new List<byte>();
                        foreach (var b in parts)
                            foreach (var bb in b)
                                arr.Add(bb);
                        Bitmap bmp;
                        using (var ms = new MemoryStream(arr.ToArray()))
                        {
                            bmp = (Bitmap)Image.FromStream(ms);
                        }
                        Display(bmp);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.ToString());
                    }
                }
            });
        }

        public void Display(Bitmap bmp)
        {
            img_disp.Dispatcher.Invoke(() => {
                var hbit = bmp.GetHbitmap();
                img_disp.Source = Imaging.CreateBitmapSourceFromHBitmap(hbit, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(hbit);
            });
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private void img_disp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (wnd_main.WindowState == WindowState.Normal)
            {
                wnd_main.WindowState = WindowState.Maximized;
                wnd_main.WindowStyle = WindowStyle.None;
                img_disp.Margin = new Thickness(0);
            }
            else
            {
                wnd_main.WindowState = WindowState.Normal;
                wnd_main.WindowStyle = WindowStyle.SingleBorderWindow;
                img_disp.Margin = new Thickness(10,10,10,210);
            }
        }
    }
}
