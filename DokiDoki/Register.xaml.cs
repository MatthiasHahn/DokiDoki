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
    /// Interaktionslogik für Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void img_next_reg_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.4, 0.6, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(0.2));
            img_next_reg.BeginAnimation(OpacityProperty, anim);
            lbl_deko_next_reg.BeginAnimation(OpacityProperty, anim2);
        }

        private void img_next_reg_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.6, 0.4, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(1, 0.8, TimeSpan.FromSeconds(0.2));
            img_next_reg.BeginAnimation(OpacityProperty, anim);
            lbl_deko_next_reg.BeginAnimation(OpacityProperty, anim2);
        }

        private void img_back_reg_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.4, 0.6, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(0.2));
            img_back_reg.BeginAnimation(OpacityProperty, anim);
            lbl_deko_back_reg.BeginAnimation(OpacityProperty, anim2);
        }

        private void img_back_reg_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0.6, 0.4, TimeSpan.FromSeconds(0.2));
            DoubleAnimation anim2 = new DoubleAnimation(1, 0.8, TimeSpan.FromSeconds(0.2));
            img_back_reg.BeginAnimation(OpacityProperty, anim);
            lbl_deko_back_reg.BeginAnimation(OpacityProperty, anim2);
        }
        private void img_back_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            Close();
        }

        private void img_next_reg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Code for checks and Server send
            //--------------------------------
            //--------------------------------
            Email_Verify ev = new Email_Verify();
            ev.Show();
            Close();
        }
    }
}
