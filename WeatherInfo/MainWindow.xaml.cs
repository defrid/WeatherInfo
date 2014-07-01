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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Tray.SetupTray(this, test);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch(this.WindowState)
            {
                case System.Windows.WindowState.Minimized:
                    Tray.Update(Tray.weatherState.rain, 10);
                    break;
            }
        }

        void test()
        {
            MessageBox.Show("Опции");
        }
    }
}
