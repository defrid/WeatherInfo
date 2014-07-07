using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WeatherInfo.Classes;

namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for Two_Windows.xaml
    /// </summary>
    public partial class Two_Windows : Window
    {
        public Two_Windows(List<MainWindow> mv)
        {
            InitializeComponent();

            List<ContentControl> cll = new List<ContentControl>();
            foreach (var a in mv)
            {
                ContentControl t = new ContentControl();
                t.Content = a.Content;
                cll.Add(t);
            }

            foreach(var a in mv)
            {
              //  a.Close();
            }

            int h = (int)this.Height/cll.Count;


            foreach(var a in cll)
            {
                this.stackP.Children.Add(a);
            }

            this.Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.F1:

                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();

                    break;
            }
        }
    }
}
