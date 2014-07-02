using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for ThirdWindow.xaml
    /// </summary>
    public partial class ThirdWindow : Window
    {
        string town;
        XMLParser xml;
        int day;

        public ThirdWindow(string _town, XMLParser forecasts, int _day)
        {
            InitializeComponent();
            xml = forecasts;
            town = _town;
            day = _day;
            fillWindow();
        }

        private void fillWindow()
        {
            City.Content = town;
            Container.Children.Clear();
            List<Forecast> curDay = xml.getDetailedWeek()[day];
            for (int i = 0; i < curDay.Count; i++)
            {
                Forecast fore = curDay[i];
                Container.Children.Add(fillStackPanel(fore));
            }
        }

        private StackPanel fillStackPanel(Forecast fore)
        {
            StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
            var dayImage = new Image
            {
                Source =
                    new BitmapImage(new Uri(WeatherAPI.ImageRequestString + String.Format("{0}.png", fore.icon))),
                Width = 64
            };
            panel.Children.Add(dayImage);
            var dayValues = new StackPanel();
            var date = DateTime.Parse(fore.date, new CultureInfo("ru-RU")).ToString("HH:mm");
            var dayDateLabel = new Label { Content = date, Padding = new Thickness(0) };
            dayValues.Children.Add(dayDateLabel);
            var temp = (fore.max + fore.min) / 2;
            var dayTemp = new Label
            {
                Content = (temp > 0 ? "+" + temp.ToString() : temp.ToString()),
                Padding = new Thickness(0)
            };
            dayValues.Children.Add(dayTemp);
            panel.Children.Add(dayValues);
            return panel;
        }

    }
}
