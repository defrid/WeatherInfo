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
using Tomers.WPF.Localization;
using WeatherInfo.Classes;

namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for Days.xaml
    /// </summary>
    public partial class Days : Window
    {
        ForecastDay[] days;
        int curDay = 0;

        public Days(ForecastDay[] forecast)
        {
            days = forecast;
            InitializeComponent();
            Normalize();
            prev.IsEnabled = false;
            WeatherPanel.Children.Add(GetWeather(days[curDay].hours.ToArray(), days[curDay].date));
        }

        private void Normalize()
        {
            Dictionary<string, string> dayParts = InitDaysDictionary();

            for (int i = 0; i < days.Length; i++)
            {
                ForecastHour[] fors = days[i].hours.ToArray();
                int temp = 0;
                fors = fors.Where(el => !Int32.TryParse(el.time, out temp)).ToArray();
                foreach (var el in fors)
                {
                    el.time = dayParts[el.time];
                }
                days[i].hours = fors.ToList();
            }
        }

        private Dictionary<string, string> InitDaysDictionary()
        {
            var days = new Dictionary<string, string>();
            days.Add("morning", LanguageDictionary.Current.Translate<string>("morning_mainWin", "Content"));//"Утро"
            days.Add("day", LanguageDictionary.Current.Translate<string>("day_mainWin", "Content"));//"День"
            days.Add("evening", LanguageDictionary.Current.Translate<string>("evening_mainWin", "Content"));//"Вечер"
            days.Add("night", LanguageDictionary.Current.Translate<string>("night_mainWin", "Content"));//"Ночь"

            return days;
        }

        private DockPanel GetWeather(ForecastHour[] forecasts, string date)
        {
            var docResult = new DockPanel();
            var titleLabel = new Label
            {
                Content = DateTime.Parse(date, System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU")).ToString("m"),
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyles.Italic
            };
            DockPanel.SetDock(titleLabel, Dock.Top);
            docResult.Children.Add(titleLabel);

            var grid = new Grid();
            for (var i = 0; i < 2; i++)
                grid.RowDefinitions.Add(new RowDefinition());
            for (var i = 0; i < 2; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            for (var i = 0; i < 2; i++)
                for (var j = 0; j < 2; j++)
                {
                    ForecastHour adding = forecasts[2 * i + j];
                    var container = new StackPanel
                    {
                        Margin = new Thickness(0, 0, 10, 0),
                        Orientation = Orientation.Horizontal
                    };
                    Grid.SetColumn(container, i);
                    Grid.SetRow(container, j);

                    var timeLabel = new Label
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Content = adding.time
                    };
                    container.Children.Add(timeLabel);

                    var bitmapImage = YandexWeatherAPI.GetBitmapImageById(adding.icon);
                    var icon = new Image
                    {
                        Source = bitmapImage,
                        Width = 32,
                        Height = 32
                    };
                    container.Children.Add(icon);

                    var stringTemp = adding.temp > 0 ? "+" + adding.temp.ToString() : adding.temp.ToString();
                    var temperLabel = new Label()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 14,
                        FontWeight = FontWeights.Bold,
                        Content = stringTemp
                    };
                    container.Children.Add(temperLabel);
                    grid.Children.Add(container);
                }
            docResult.Children.Add(grid);
            return docResult;
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            curDay++;
            WeatherPanel.Children.Clear();
            WeatherPanel.Children.Add(GetWeather(days[curDay].hours.ToArray(), days[curDay].date));
            if (curDay == 9) { next.IsEnabled = false; }
            if (curDay > 0) { prev.IsEnabled = true; }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            curDay--;
            WeatherPanel.Children.Clear();
            WeatherPanel.Children.Add(GetWeather(days[curDay].hours.ToArray(), days[curDay].date));
            if (curDay == 0) { prev.IsEnabled = false; }
            if (curDay < 9) { next.IsEnabled = true; }
        }
    }
}
