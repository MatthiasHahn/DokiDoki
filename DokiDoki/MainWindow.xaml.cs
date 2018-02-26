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

namespace DokiDoki
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
