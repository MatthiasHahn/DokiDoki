﻿using System;
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
using CSCore.Codecs.MP3;
using CSCore.SoundIn;
using System.Threading;
using System.Windows.Threading;
using CSCore.Codecs.WAV;

namespace DokiDoki_Server
{
    class Program
    {
        static Socket socket;
        static MemoryStream soundarr = new MemoryStream();
        //static WasapiCapture cptr;

        static ImageConverter con = new ImageConverter();
        static ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
        static EncoderParameters myEncoderParameters = new EncoderParameters(2);
        static MemoryStream ConverterStream = new MemoryStream();
        static IPEndPoint m = new IPEndPoint(IPAddress.Parse("224.168.55.25"), 8888);
        static IPEndPoint local = new IPEndPoint(IPAddress.Loopback, 9999); //IP_CHG 192.168.1.1
        static System.Timers.Timer CaptureLoop;

        static List<byte> ScreenS;
        static void Main(string[] args)
        {
            myEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 30L);
            myEncoderParameters.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, 1L);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.MulticastLoopback = true;
            socket.Bind(local);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(m.Address, local.Address));
            string procid = Console.ReadLine();

            //WaveFormat format = new WaveFormat(4000, 6, 1);
            //cptr = new WasapiLoopbackCapture(10, format, ThreadPriority.AboveNormal, true);
            //WaveWriter wrt = new WaveWriter(soundarr, cptr.WaveFormat);
            //cptr.Initialize();
            //cptr.DataAvailable += (s, e) =>
            //{
            //    wrt.Write(e.Data, e.Offset, e.ByteCount);
            //};
            //cptr.Start();

            var procs = Process.GetProcessesByName(procid);
            Process proc = null;
            var rect = new User.RECT();
            foreach (var prc in procs)
            {
                User.GetWindowRect(prc.MainWindowHandle, ref rect);
                if ((rect.left != 0 || rect.right != 0) || (rect.bottom != 0 || rect.top != 0))
                {
                    proc = prc;
                    break;
                }
            }
            if (proc == null)
                return;

            Capture(proc);

            CaptureLoop = new System.Timers.Timer();
            CaptureLoop.Interval = 32;
            CaptureLoop.Elapsed += (sender, e) => { Send(ScreenS); };
            CaptureLoop.Start();
            while(true)
            {                
                Capture(proc);
            } 
        }
        static void Capture(Process proc)
        {
            var rect = new User.RECT();
            User.GetWindowRect(proc.MainWindowHandle, ref rect);
            if (rect.top >= 0 && rect.bottom >= 0 && rect.left >= 0 && rect.right >= 0)
            {
                using (var bmp = new Bitmap(rect.right - rect.left - 20, rect.bottom - rect.top - 50, System.Drawing.Imaging.PixelFormat.Format16bppRgb555))
                {
                    Graphics.FromImage(bmp).CopyFromScreen(rect.left + 10, rect.top + 40, 0, 0, new System.Drawing.Size(rect.right - rect.left - 20, rect.bottom - rect.top - 50), CopyPixelOperation.SourceCopy);
                    ScreenS = ConvertTo(bmp);
                }
            }
            /*
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
            }*/
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)            
                if (codec.FormatID == format.Guid)                
                    return codec;            
            return null;
        }
        private static List<byte> ConvertTo(Bitmap bmp)
        {
            List<byte> arr = new List<byte>();
            using (ConverterStream = new MemoryStream())
            {
                bmp.Save(ConverterStream, GetEncoder(ImageFormat.Jpeg), myEncoderParameters);
                arr.AddRange(ConverterStream.ToArray());
            }
            return arr;
        }
        private static Task Send(List<byte> arr)
        {            
            List<byte[]> parts = new List<byte[]>();
            int partsize = 32000;            
            for (int i = 0; i < arr.Count; i += partsize)
            {
                parts.Add(i + partsize <= arr.Count ?
                    arr.GetRange(i, partsize).ToArray() :
                    arr.GetRange(i, arr.Count - i).ToArray());
            }

            //List<byte[]> soundparts = new List<byte[]>();
            //List<byte> soundar = soundarr.ToArray().ToList();
            //soundarr = new MemoryStream();
            //for (int i = 0; i < soundar.Count; i += partsize)
            //{
            //    soundparts.Add(i + partsize <= soundar.Count ?
            //        soundar.GetRange(i, partsize).ToArray() :
            //        soundar.GetRange(i, soundar.Count - i).ToArray());
            //}            
            Console.WriteLine("Parts: " + parts.Count + ", Arr: " + arr.Count);// + " | Sound_Parts: " + soundparts.Count + ", Sound_Arr: " + soundar.Count);
            

            socket.SendTo(new byte[] { Convert.ToByte(parts.Count) }, m);
            //socket.SendTo(new byte[] { Convert.ToByte(soundparts.Count) }, m);

            foreach (var p in parts)
                socket.SendTo(p,m);
            
            //foreach (var p in soundparts)
            //    socket.SendTo(p, m);

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
            [return: MarshalAs(UnmanagedType.AsAny)]
            public static extern void GetWindowRect(IntPtr hWnd, ref RECT rect);
        }
    }
}