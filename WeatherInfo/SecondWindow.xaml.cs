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
    /// Interaction logic for SecondWindow.xaml
    /// </summary>
    public partial class SecondWindow : Window
    {
        XMLParser xml;
        public SecondWindow(XMLParser mainParser)
        {
            InitializeComponent();
            xml = mainParser;
            Container.Children.Add(GetForecastElement(
                new Forecast(20, 25, "cld", "dta", "13d"),
                new Forecast(10, 15, "cloud", "data", "13n")));
        }

        //<Border BorderBrush="Black" BorderThickness="1">
        //        <DockPanel >
        //        <StackPanel Orientation="Horizontal" >
        //            <Image Source="http://openweathermap.org/img/w/10d.png" Width="64"></Image>
        //            <StackPanel>
        //                <Label Content="1 july,BT,Day" Padding="0"></Label>
        //                <Label Content="Temp:+25" Padding="0"></Label>
        //                <Label Content="Vlaga:33%" Padding="0"></Label>
        //                <Label Content="Wind: 3..4 mps" Padding="0"></Label>
        //            </StackPanel>
        //        </StackPanel>
        //        <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Margin="0 0 10 0">
        //            <Image Source="http://openweathermap.org/img/w/10d.png" Width="64"></Image>
        //            <StackPanel>
        //                <Label Content="1 july,BT,Day" Padding="0"></Label>
        //                <Label Content="Temp:+25" Padding="0"></Label>
        //                <Label Content="Vlaga:33%" Padding="0"></Label>
        //                <Label Content="Wind: 3..4 mps" Padding="0"></Label>
        //            </StackPanel>
        //        </StackPanel>
        //        </DockPanel>
        //    </Border>

        private static Border GetForecastElement(Forecast dayForecast,Forecast nightForecast)
        {
            var borderResult = new Border {BorderBrush = Brushes.Black, BorderThickness = new Thickness(1),Margin = new Thickness(0,2,0,2)};
            var dockPanel = new DockPanel();
            var dayStackPanel = new StackPanel {Orientation = Orientation.Horizontal};
            var dayImage = new Image
                {
                    Source =
                        new BitmapImage(new Uri(WeatherAPI.ImageRequestString + String.Format("{0}.png", dayForecast.icon))),
                    Width=64
                };
            dayStackPanel.Children.Add(dayImage);
            var dayValues = new StackPanel();
            var dayDateLabel = new Label {Content = dayForecast.date, Padding = new Thickness(0)};
            dayValues.Children.Add(dayDateLabel);
            var dayTemp = new Label { Content = (dayForecast.max+dayForecast.min)/2, Padding = new Thickness(0) };
            dayValues.Children.Add(dayTemp);
            dayStackPanel.Children.Add(dayValues);

            var nightStackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0,0,10,0)
                };
            var nightImage = new Image
            {
                Source =
                    new BitmapImage(new Uri(WeatherAPI.ImageRequestString + String.Format("{0}.png", nightForecast.icon))),
                Width = 64
            };
            nightStackPanel.Children.Add(nightImage);
            var nightValues = new StackPanel();
            var nightDateLabel = new Label { Content = nightForecast.date, Padding = new Thickness(0) };
            nightValues.Children.Add(nightDateLabel);
            var nightTemp = new Label { Content = (nightForecast.max + nightForecast.min) / 2, Padding = new Thickness(0) };
            nightValues.Children.Add(nightTemp);
            nightStackPanel.Children.Add(nightValues);
            dockPanel.Children.Add(dayStackPanel);
            dockPanel.Children.Add(nightStackPanel);
            borderResult.Child = dockPanel;
            return borderResult;

        }

    }
}
