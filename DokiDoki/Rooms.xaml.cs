using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Controls;
using Image = System.Drawing.Image;

namespace DokiDoki
{
    /// <summary>
    /// Interaktionslogik für Rooms.xaml
    /// </summary>
    public partial class Rooms : Window
    {
        static IPEndPoint m = new IPEndPoint(IPAddress.Parse("224.168.55.25"), 8888);
        static IPEndPoint local = new IPEndPoint(IPAddress.Any, 8888);
        static ObservableCollection<string> chat = new ObservableCollection<string>();
        private static TcpClient tcpClient;
        static string Name;

        public Rooms(IPEndPoint Server, string name)
        {
            InitializeComponent();
            Name = name;
            lbx_chat.ItemsSource = chat;
            tcpClient = new TcpClient(new IPEndPoint(IPAddress.Loopback, 777));
            tcpClient.Connect(new IPEndPoint(IPAddress.Loopback, 887));
            StreamWriter sw = new StreamWriter(tcpClient.GetStream());
            StreamReader rdr = new StreamReader(tcpClient.GetStream());
            Task.Run(() =>
            {
                while (true)
                {
                    var msg = rdr.ReadLine();
                    lbx_chat.Dispatcher.Invoke(() => chat.Add(msg));
                }
            });
            chat.CollectionChanged +=
                new NotifyCollectionChangedEventHandler((object sender, NotifyCollectionChangedEventArgs e) =>
                {
                    Decorator border = VisualTreeHelper.GetChild(lbx_chat, 0) as Decorator;
                    if (border != null && border.Child is ScrollViewer)
                        ((ScrollViewer)border.Child).ScrollToBottom();

                    //Decorator border = VisualTreeHelper.GetChild(lbx_chat, 0) as Decorator;
                    //if (border != null)
                    //{
                    //    ScrollViewer scrollViewer = border.Child as ScrollViewer;
                    //    if (scrollViewer != null)
                    //    {
                    //        scrollViewer.ScrollToBottom();
                    //    }
                    //}

                    foreach (var i in e.NewItems)
                        if (i.ToString().Split(':')[0] == Name)
                            sw.WriteLine(i);
                    sw.Flush();
                });

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.MulticastLoopback = true;
            socket.Bind(local);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                new MulticastOption(m.Address, IPAddress.Any));
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        List<byte[]> parts = new List<byte[]>();
                        byte[] buffer = new byte[1];
                        socket.Receive(buffer);
                        var count = Convert.ToInt32(buffer[0]);
                        for (int i = 0; i < count; i++)
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
                            bmp = (Bitmap) Image.FromStream(ms);
                        }
                        Display(bmp);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            });
        }

        public void Display(Bitmap bmp)
        {
            img_disp.Dispatcher.Invoke(() =>
            {
                var hbit = bmp.GetHbitmap();
                img_disp.Source = Imaging.CreateBitmapSourceFromHBitmap(hbit, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
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
                img_disp.Margin = new Thickness(10, 10, 10, 210);
            }
        }


        private void tbx_chat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && tbx_chat.Text != "")
            {
                chat.Add(Name + ": " + tbx_chat.Text);
                tbx_chat.Text = "";
            }
        }
    }
}