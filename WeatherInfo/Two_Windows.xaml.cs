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

namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for Two_Windows.xaml
    /// </summary>
    public partial class Two_Windows : Window
    {
        public Two_Windows(Window one, Window two)
        {
            InitializeComponent();

            cc1.Content = one.Content;
            cc2.Content = two.Content;

            one.Close();
            two.Close();
        }
    }
}
