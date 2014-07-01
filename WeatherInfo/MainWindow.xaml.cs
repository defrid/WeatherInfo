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
using WeatherInfo.Classes;


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
            Tray.SetupTray(this, test, full, shortI);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch(this.WindowState)
            {
                case System.Windows.WindowState.Minimized:
                    Tray.Update(new Forecast(10, 12, "clouds", "date", "02n"));
                    break;
            }
        }

        void test()
        {
            MessageBox.Show("Опции");
        }

        void full()
        {
            MessageBox.Show("Полная инфа");
        }

        void shortI()
        {
            MessageBox.Show("Краткая инфа");
        }
    }
}
