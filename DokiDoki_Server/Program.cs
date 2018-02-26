using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DokiDoki_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string procid = Console.ReadLine();
            Capture(int.Parse(procid));
        }

        static void Capture(int procid)
        {
            var proc = Process.GetProcessById(procid);
            var rect= new User.RECT();
            DateTime fps_lock = DateTime.Now;
            Task.Run(() =>
            {
                while (true)
                {
                    if ((DateTime.Now - fps_lock).Milliseconds >= 32)
                    {
                        User.GetWindowRect(proc.MainWindowHandle, ref rect);
                        using (var bmp = new Bitmap(rect.right - rect.left, rect.bottom - rect.top, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                        {
                            bmp.SetResolution((rect.right - rect.left) / 4, (rect.bottom - rect.top) / 4);
                            Graphics.FromImage(bmp).CopyFromScreen(rect.left, rect.top, 0, 0, new System.Drawing.Size(rect.right - rect.left, rect.bottom - rect.top), CopyPixelOperation.SourceCopy);
                            //TODO: Send bmp
                        }
                        fps_lock = DateTime.Now;
                    }
                }
            });
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

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
