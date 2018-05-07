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
using CSCore;
using CSCore.SoundOut;
using CSCore.Codecs.MP3;
using System.Threading;
using CSCore.Codecs.WAV;

namespace DokiDoki
{
    /// <summary>
    /// Interaktionslogik für Rooms.xaml
    /// </summary>
    public partial class Rooms : Window
    {
        static IPEndPoint m = new IPEndPoint(IPAddress.Parse("224.168.55.25"), 8888);
        static IPEndPoint local = new IPEndPoint(IPAddress.Loopback, 8888); //IP_CHG 192.168.1.10
        static ObservableCollection<string> chat = new ObservableCollection<string>();
        private static TcpClient tcpClient;
        static string Name;
        static long rdrpos, wrtpos;

        static MemoryStream soundstrm = new MemoryStream();
        static bool played = false;

        public Rooms(IPEndPoint Server, string name)
        {
            InitializeComponent();
            Name = name;
            lbx_chat.ItemsSource = chat;
            tcpClient = new TcpClient(new IPEndPoint(IPAddress.Loopback, 0)); //IP_CHG 192.168.1.1
            tcpClient.Connect(new IPEndPoint(IPAddress.Loopback, 888)); //IP_CHG 192.168.1.1
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
                    lbx_chat.ScrollIntoView(lbx_chat.Items[lbx_chat.Items.Count - 1]);
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
                bool exep;
                while (true)
                {
                    exep = false;
                    List<byte[]> parts = new List<byte[]>();
                    byte[] buffer = new byte[1];
                    byte[] sbuffer = new byte[1];
                    try { socket.Receive(buffer); /*socket.Receive(sbuffer);*/ }
                    catch (Exception ex)
                    { exep = true; }
                    if (!exep)
                    {
                        var count = Convert.ToInt32(buffer[0]);
                        for (int i = 0; i < count - 1; i++)
                        {
                            buffer = new byte[32000];
                            socket.Receive(buffer, 32000, SocketFlags.None);
                            parts.Add(buffer);
                        }
                        buffer = new byte[32000];
                        socket.Receive(buffer);
                        parts.Add(buffer);

                        //List<byte[]> soundparts = new List<byte[]>();
                        //count = Convert.ToInt32(sbuffer[0]);
                        //for (int i = 0; i < count; i++)
                        //{
                        //    buffer = new byte[32000];
                        //    socket.Receive(buffer);
                        //    soundparts.Add(buffer);
                        //}

                        Task.Run(() => new Action<List<byte[]>>((prts) =>
                        {
                            List<byte> arr = new List<byte>();
                            foreach (var b in prts)
                                arr.AddRange(b);

                            //List<byte> soundarr = new List<byte>();
                            //foreach (var b in soundparts)
                            //    soundarr.AddRange(b);

                            if (arr.Count > 0 && arr[0] == 255 && arr[1] == 216)
                            {
                                Bitmap bmp;
                                using (var ms = new MemoryStream(arr.ToArray()))
                                {
                                    bmp = (Bitmap)Image.FromStream(ms);
                                    Display(bmp);
                                }
                            }
                            //if (soundarr.Count > 10 && soundstrm.CanWrite)
                            //{
                            //    rdrpos = soundstrm.Position;
                            //    soundstrm.Position = wrtpos;

                            //    soundstrm.Write(soundarr.ToArray(), 0, soundarr.Count);

                            //    wrtpos = soundstrm.Position;
                            //    soundstrm.Position = rdrpos;

                            //    if(!played)
                            //        PlaySound();
                            //}
                        }).Invoke(parts));
                    }
                }
            });
        }
        private static IWaveSource GetSoundSource(Stream stream)
        {
            return new WaveFileReader(stream);
        }
        public void PlaySound()
        {
            try
            {
                played = true;
                ISoundOut soundOut = new WasapiOut();
                soundOut.Initialize(GetSoundSource(soundstrm));
                soundOut.Volume = 1;
                soundOut.Play();
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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
            var pos = e.GetPosition(img_disp);
            var r = new System.Windows.Shapes.Rectangle();
            r.Width = r.Height = 10;
            r.Fill = System.Windows.Media.Brushes.Red;
            r.VerticalAlignment = VerticalAlignment.Top;
            r.HorizontalAlignment = HorizontalAlignment.Left;
            r.Margin = new Thickness(pos.X + r.Width, pos.Y + r.Height, 0, 0);
            grd_main.Children.Add(r);
            LeftMouseClick((int)pos.X+734+10, (int)pos.Y+17+200);
            //if (wnd_main.WindowState == WindowState.Normal)
            //{
            //    wnd_main.WindowState = WindowState.Maximized;
            //    wnd_main.WindowStyle = WindowStyle.None;
            //    img_disp.Margin = new Thickness(0);
            //}
            //else
            //{
            //    wnd_main.WindowState = WindowState.Normal;
            //    wnd_main.WindowStyle = WindowStyle.SingleBorderWindow;
            //    img_disp.Margin = new Thickness(10, 10, 10, 210);
            //}
        }

        private void tbx_chat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && tbx_chat.Text != "")
            {
                chat.Add(Name + ": " + tbx_chat.Text);
                tbx_chat.Text = "";
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }
    }
}