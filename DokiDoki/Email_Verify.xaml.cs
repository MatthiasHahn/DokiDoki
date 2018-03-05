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
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace DokiDoki
{
    /// <summary>
    /// Interaktionslogik für Email_Verify.xaml
    /// </summary>
    public partial class Email_Verify : Window
    {
        public Email_Verify()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void wnd_email_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

        }
        private void img_next_em_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.4, 0.6, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(0.2));
            img_next_em.BeginAnimation(OpacityProperty, anim);
            lbl_deko_next_em.BeginAnimation(OpacityProperty, anim2);
        }

        private void img_next_em_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.6, 0.4, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(1, 0.8, TimeSpan.FromSeconds(0.2));
            img_next_em.BeginAnimation(OpacityProperty, anim);
            lbl_deko_next_em.BeginAnimation(OpacityProperty, anim2);
        }

        private void img_back_em_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.4, 0.6, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(0.2));
            img_back_em.BeginAnimation(OpacityProperty, anim);
            lbl_deko_back_em.BeginAnimation(OpacityProperty, anim2);
        }

        private void img_back_em_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.6, 0.4, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(1, 0.8, TimeSpan.FromSeconds(0.2));
            img_back_em.BeginAnimation(OpacityProperty, anim);
            lbl_deko_back_em.BeginAnimation(OpacityProperty, anim2);
        }
        private void img_back_em_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Register rg = new Register();
            rg.Show();
            Close();
        }

        private void img_next_em_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            Close();
        }
        }
}
