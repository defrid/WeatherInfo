﻿using System;
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
using System.Threading;
using DataHandlerInterface.Classes;
using Tomers.WPF.Localization;
using WeatherInfo.Classes;



namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for curWeather.xaml
    /// </summary>
    public partial class curWeather : Window
    {

        ForecastHour cur;
        bool isKelv;
        bool isFahr;

        public curWeather(ForecastHour forecast)
        {
            isKelv = App.settings.temperatureUnits.rusName.Equals("Кельвины");
            isFahr = App.settings.temperatureUnits.rusName.Equals("Фаренгейты");

            InitializeComponent();
            cur = forecast;
            CurrentWeather.Children.Add(GetWeaterElement());
            this.Show();
        }


        private Grid GetWeaterElement()
        {
            var gridResult = new Grid();
            gridResult.Width = this.Width;
            gridResult.SetValue(Grid.RowProperty, 0);
            gridResult.SetValue(Grid.ColumnProperty, 0);
            for (var i = 0; i < 2; i++)
            {
                gridResult.RowDefinitions.Add(new RowDefinition());
                gridResult.ColumnDefinitions.Add(new ColumnDefinition());
            }
            gridResult.RowDefinitions.Add(new RowDefinition());
            var specRowDef = new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) };
            gridResult.ColumnDefinitions.Add(specRowDef);

            var dayLabel = new Label
            {
                Content = DateTime.Now.ToString("t", LanguageContext.Instance.Culture),
                FontWeight = FontWeights.Bold
            };
            gridResult.Children.Add(dayLabel);

            var tempLabel = new Label()
            {
                Content = isKelv ? cur.temp + "K" : 
                        isFahr ? ((cur.temp > 0) ? "+" : "") + cur.temp + "F" :
                        cur.temp > 0 ? "+" + cur.temp.ToString() : cur.temp.ToString(),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 15,
                FontWeight = FontWeights.Bold
            };
            tempLabel.SetValue(Grid.ColumnProperty, 2);
            tempLabel.SetValue(Grid.ColumnSpanProperty, 2);
            tempLabel.SetValue(Grid.RowProperty, 0);
            gridResult.Children.Add(tempLabel);
            var image = new Image
            {
                Source = true ? new BitmapImage(
                    new Uri(OpenWeatherAPI.ImageRequestString + String.Format("{0}.png", cur.icon)))
                    : YandexWeatherAPI.GetBitmapImageById(cur.icon)
            };
            image.SetValue(Grid.RowProperty, 1);
            image.SetValue(Grid.RowSpanProperty, 2);
            image.SetValue(Grid.ColumnSpanProperty, 2);
            gridResult.Children.Add(image);
            return gridResult;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
    }
}
