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
        public ThirdWindow()
        {
            InitializeComponent();
        }

        private void fillStackPanel(ref StackPanel panel, Forecast fore)
        {
            var dayImage = new Image
            {
                Source =
                    new BitmapImage(new Uri(WeatherAPI.ImageRequestString + String.Format("{0}.png", fore.icon))),
                Width = 64
            };
            panel.Children.Add(dayImage);
            var dayValues = new StackPanel();
            var date = DateTime.Parse(fore.date, new CultureInfo("ru-RU")).ToString("d MMM ddd");
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
        }

        private Border GetForecastElement(Forecast forecast)
        {
            var borderResult = new Border { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Margin = new Thickness(0, 2, 0, 2) };
            var dockPanel = new DockPanel();

            if (forecast != null)
            {
                var dayStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
                fillStackPanel(ref dayStackPanel, forecast);
                dockPanel.Children.Add(dayStackPanel);
            }

            borderResult.Child = dockPanel;
            return borderResult;
        }
    }
}
