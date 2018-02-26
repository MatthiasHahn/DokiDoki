using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DokiDoki_Server
{
    class Program
    {
        static Socket socket;
        static IPEndPoint localEndPoint;
        static void Main(string[] args)
        {

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            localEndPoint = new IPEndPoint(IPAddress.Loopback, 9999);
            socket.Bind(localEndPoint);
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 8888);
            socket.SendBufferSize = 819200;
            byte[] buffer = new byte[3];
            socket.ReceiveFrom(buffer, ref remoteEP);
            socket.Connect(remoteEP);
            string procid = Console.ReadLine();
            Capture(int.Parse(procid));
        }

        static void Capture(int procid)
        {
            var proc = Process.GetProcessById(procid);
            var rect= new User.RECT();
            DateTime fps_lock = DateTime.Now;            
            while (true)
            {
                if ((DateTime.Now - fps_lock).Milliseconds >= 32)
                {
                    User.GetWindowRect(proc.MainWindowHandle, ref rect);
                    using (var bmp = new Bitmap(rect.right - rect.left, rect.bottom - rect.top, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        bmp.SetResolution((rect.right - rect.left) / 4, (rect.bottom - rect.top) / 4);
                        Graphics.FromImage(bmp).CopyFromScreen(rect.left, rect.top, 0, 0, new System.Drawing.Size(rect.right - rect.left, rect.bottom - rect.top), CopyPixelOperation.SourceCopy);
                        Send(bmp);
                    }
                    fps_lock = DateTime.Now;
                }
            }
        }
        private static void Send(Bitmap bmp)
        {
            ImageConverter con = new ImageConverter();
            byte[] arr = (byte[])con.ConvertTo(bmp, typeof(byte[]));
            for (int i = 0; i < arr.Length; i += 1500)
            {
                if (i + 1500 < arr.Length)
                    socket.Send(arr, i, i + 1500, SocketFlags.None);
                else
                    socket.Send(arr, i, arr.Length, SocketFlags.None);
            }
            socket.Send(Encoding.ASCII.GetBytes("Stop"));
        }
        private class User
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        }
    }
}
