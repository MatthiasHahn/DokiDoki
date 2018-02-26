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

namespace DokiDoki
{
    /// <summary>
    /// Interaktionslogik für Rooms.xaml
    /// </summary>
    public partial class Rooms : Window
    {
        public Rooms(IPEndPoint Server)
        {
            InitializeComponent();
        }

        public void Display(Bitmap bmp)
        {
            img_disp.Dispatcher.Invoke(() => {
                ImageConverter converter = new ImageConverter();
                byte[] arr = (byte[])converter.ConvertTo(bmp, typeof(byte[]));

                Bitmap bmp2;
                using (var ms = new MemoryStream(arr))
                {
                    bmp2 = (Bitmap)Image.FromStream(ms);
                }
                var hbit = bmp2.GetHbitmap();
                img_disp.Source = Imaging.CreateBitmapSourceFromHBitmap(hbit, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(hbit);
            });
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}
