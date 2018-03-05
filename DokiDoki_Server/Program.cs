using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DokiDoki_Server
{
    class Program
    {
        static Socket socket;
        static IPEndPoint localEndPoint;
        static void Main(string[] args)
        {

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            localEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.10"), 9999);
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
                if ((DateTime.Now - fps_lock).Milliseconds >= 8)
                {
                    User.GetWindowRect(proc.MainWindowHandle, ref rect);
                    using (var bmp = new Bitmap(rect.right - rect.left, rect.bottom - rect.top, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        Graphics.FromImage(bmp).CopyFromScreen(rect.left, rect.top, 0, 0, new System.Drawing.Size(rect.right - rect.left, rect.bottom - rect.top), CopyPixelOperation.SourceCopy);
                        Send(bmp);
                    }
                    fps_lock = DateTime.Now;
                }
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach(ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;
            return null;
        }
        private static void Send(Bitmap bmp)
        {
            ImageConverter con = new ImageConverter();
            byte[] arr;
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 30L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            using (var stream = new MemoryStream())
            {
                bmp.Save(stream, jpgEncoder, myEncoderParameters);
                arr = stream.ToArray();
            }
            List<byte[]> parts = new List<byte[]>();
            int partsize = 64000;
            for(int i = 0; i < arr.Length; i += partsize)
            {
                byte[] res;
                if (i + partsize <= arr.Length)
                {
                    res = new byte[partsize];
                    Array.Copy(arr, i, res, 0, partsize);
                }
                else
                {
                    res = new byte[arr.Length - i];
                    Array.Copy(arr, i, res, 0, arr.Length - i);
                }
                parts.Add(res);
            }
            socket.Send(new byte[] { Convert.ToByte(parts.Count) });
            for (int i = 0; i < parts.Count; i++)
                socket.Send(parts[i]);
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
