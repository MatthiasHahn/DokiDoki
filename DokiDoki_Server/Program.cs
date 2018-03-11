using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.SoundIn;
using System.Threading;

namespace DokiDoki_Server
{
    class Program
    {
        static Socket socket;
        static MemoryStream SoundStream = new MemoryStream();

        static ImageConverter con = new ImageConverter();
        static ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
        static EncoderParameters myEncoderParameters = new EncoderParameters(1);
        static MemoryStream ConverterStream = new MemoryStream();
        static IPEndPoint m = new IPEndPoint(IPAddress.Parse("224.168.55.25"), 8888);
        static IPEndPoint local = new IPEndPoint(IPAddress.Any, 9999);
        static void Main(string[] args)
        {
            myEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 30L);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.MulticastLoopback = true;
            socket.Bind(local);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(m.Address, IPAddress.Any));
            string procid = Console.ReadLine();

            /*WaveFormat waveFormat = new WaveFormat(4000, 8, 1, AudioEncoding.Gsm);
            WasapiCapture cptr = new WasapiLoopbackCapture(10, waveFormat, ThreadPriority.AboveNormal, true);            
            cptr.Initialize();
            WaveWriter w = new WaveWriter(SoundStream, cptr.WaveFormat);            
            cptr.DataAvailable += (s, e) =>
            {
                w.Write(e.Data, e.Offset, e.ByteCount);
            };
            cptr.Start();*/
            Capture(procid);
        }
        static void Capture(string procid)
        {
            var procs = Process.GetProcessesByName(procid);
            Process proc = null;
            var rect = new User.RECT();
            foreach (var prc in procs)
            {
                User.GetWindowRect(prc.MainWindowHandle, ref rect);
                if ((rect.left != 0 && rect.right != 0) || (rect.bottom != 0 && rect.top != 0))
                {
                    proc = prc;
                    break;
                }
            }
            if (proc == null)
                return;
            DateTime fps_lock = DateTime.Now;    
            while (true)
            {
                if ((DateTime.Now - fps_lock).Milliseconds >= 16)
                {
                    User.GetWindowRect(proc.MainWindowHandle, ref rect);
                    using (var bmp = new Bitmap(rect.right - rect.left, rect.bottom - rect.top, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
                    {                        
                        Graphics.FromImage(bmp).CopyFromScreen(rect.left, rect.top, 0, 0, new System.Drawing.Size(rect.right - rect.left, rect.bottom - rect.top), CopyPixelOperation.SourceCopy);
                        fps_lock = DateTime.Now;
                        Send(bmp);
                    }                    
                }
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)            
                if (codec.FormatID == format.Guid)                
                    return codec;            
            return null;
        }
        private static Task Send(Bitmap bmp)
        {
            List<byte> arr = new List<byte>();
            ConverterStream = new MemoryStream();
            bmp.Save(ConverterStream, jpgEncoder, myEncoderParameters);
            arr.AddRange(ConverterStream.ToArray());
            ConverterStream.Dispose();
            List<byte[]> parts = new List<byte[]>();
            int partsize = 64000;
            for (int i = 0; i < arr.Count; i += partsize)
            {
                parts.Add(i + partsize <= arr.Count ?
                    arr.GetRange(i, partsize).ToArray() :
                    arr.GetRange(i, arr.Count - i).ToArray());
            }

            /*List<byte> soundarr = new List<byte>();
            List<byte[]> soundparts = new List<byte[]>();
            soundarr.AddRange(SoundStream.ToArray());
            SoundStream.Flush();
            for(int i = 0; i<soundarr.Count; i += partsize)
            {
                soundparts.Add(i + partsize <= soundparts.Count ? 
                    soundarr.GetRange(i, partsize).ToArray() : 
                    soundarr.GetRange(i, soundarr.Count - i).ToArray());
            }*/

            socket.SendTo(new byte[] { Convert.ToByte(parts.Count) }, m);
            foreach (var p in parts)
                socket.SendTo(p,m);
            return Task.FromResult(true);
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
