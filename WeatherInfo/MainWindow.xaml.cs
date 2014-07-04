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
using System.Globalization;
using WeatherInfo.Classes;

namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static XMLParser forecasts;
        private static string town;
        private static int emptyDays;

        public MainWindow()
        {
            town = App.settings.city.cityName;
            try
            {
                InitializeComponent();
            }
            catch (Exception exc)
            {
                MessageBox.Show("1. " + exc.Message);
            }
            fillTable();
            Tray.SetupTray(this, options, expandFull, expandShort);
            //town = App.settings.city.name;
        }

        private void fillTable()
        {
            //town = "Moscow";
            forecasts = new XMLParser(town);
            ForecastDay[] days = forecasts.getBigForecast();
            DateTime date = DateTime.Parse(days[0].date, CultureInfo.InvariantCulture);
            int dayOfWeek = (int)date.DayOfWeek - 1;
            emptyDays = dayOfWeek;
            int index = 0;
            City.Content = town;
            MonthYear.Content = date.Month + "/" + date.Year;
            for (int i = 0; i < 2; i++)
            {
                for (; dayOfWeek < 7; dayOfWeek++)
                {
                    WeatherTable.Children.Add(GetWeaterElement(dayOfWeek, i + 1, days[index]));
                    index++;
                }
                dayOfWeek = 0;
            }
        }

        private static Grid GetWeaterElement(int column, int row, ForecastDay fore)
        {
            var gridResult = new Grid();
            gridResult.MouseLeftButtonUp += gridResult_MouseLeftButtonUp;
            gridResult.SetValue(Grid.RowProperty, row);
            gridResult.SetValue(Grid.ColumnProperty, column);
            for (var i = 0; i < 2; i++)
            {
                gridResult.RowDefinitions.Add(new RowDefinition());
                gridResult.ColumnDefinitions.Add(new ColumnDefinition());
            }
            gridResult.RowDefinitions.Add(new RowDefinition());
            var specRowDef = new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) };
            gridResult.ColumnDefinitions.Add(specRowDef);
            string day = fore.date.Substring(8, 2);
            var dayLabel = new Label { Content = day, FontWeight = FontWeights.Bold };
            gridResult.Children.Add(dayLabel);
            var maxTempLabel = new Label()
                {
                    Content = (fore.day > 0 ? "+" + fore.day.ToString() : fore.day.ToString()),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    FontSize = 15,
                    FontWeight = FontWeights.Bold
                };
            maxTempLabel.SetValue(Grid.ColumnProperty, 1);
            maxTempLabel.SetValue(Grid.ColumnSpanProperty, 2);
            gridResult.Children.Add(maxTempLabel);
            var minTempLabel = new Label()
                {
                    Content = (fore.ngt > 0 ? "+" + fore.ngt.ToString() : fore.ngt.ToString()),
                    HorizontalAlignment = HorizontalAlignment.Right
                };
            minTempLabel.SetValue(Grid.RowProperty, 1);
            minTempLabel.SetValue(Grid.ColumnProperty, 2);
            gridResult.Children.Add(minTempLabel);
            var image = new Image
                {
                    Source = new BitmapImage(
                        new Uri(OpenWeatherAPI.ImageRequestString + String.Format("{0}.png", fore.icon)))
                };
            image.SetValue(Grid.RowProperty, 1);
            image.SetValue(Grid.RowSpanProperty, 2);
            image.SetValue(Grid.ColumnSpanProperty, 2);
            gridResult.Children.Add(image);
            return gridResult;
        }

        static void gridResult_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void moreInfoClick(object sender, RoutedEventArgs e)
        {
        }

        private void updateClick(object sender, RoutedEventArgs e)
        {
            WeatherTable.Children.RemoveRange(7, 14);
            fillTable();
        }
        /*
         <Image Source="http://openweathermap.org/img/w/10d.png" Grid.Row="1"  Grid.RowSpan="2" Grid.ColumnSpan="2"></Image> 
         
         */
        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case System.Windows.WindowState.Minimized:
                    Tray.Update(forecasts.getCurHour());
                    break;
            }
        }

        void expandShort()
        {
            this.WindowState = System.Windows.WindowState.Normal;
        }

        void expandFull()
        {
        }

        void options()
        {
            new SettingsWindow().Show();
        }
    }
}
