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
using System.Globalization;


namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for SecondWindow.xaml
    /// </summary>
    public partial class SecondWindow : Window
    {
        XMLParser xml;

        public SecondWindow(XMLParser mainParser, string town)
        {
            InitializeComponent();
            xml = mainParser;
            City.Content = town;
            fillForm();
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

        private void fillForm()
        {
            Container.Children.Clear();
            List<Forecast>[] days = xml.getDetailedWeek();
            for (int i = 0; i < 5; i++)
            {
                Forecast night = days[i].Last();
                if (days[i].Count < 4)
                {
                    Container.Children.Add(GetForecastElement(null, night));
                    continue;
                }
                Forecast day = days[i][days[i].Count - 3];
                Container.Children.Add(GetForecastElement(day, night));
            }
        }

        private void fillStackPanel(ref StackPanel panel, Forecast fore)
        {
            var dayImage = new Image
            {
                Source =
                    new BitmapImage(new Uri(OpenWeatherAPI.ImageRequestString + String.Format("{0}.png", fore.icon))),
                Width = 64
            };
            panel.Children.Add(dayImage);
            var dayValues = new StackPanel();
            string date = DateTime.Parse(fore.date, new CultureInfo("ru-RU")).ToString("d MMM ddd");
            var dayDateLabel = new Label { Content = date, Padding = new Thickness(0) };
            dayValues.Children.Add(dayDateLabel);
            int temp = (fore.max + fore.min) / 2;
            var dayTemp = new Label 
            { 
                Content = (temp > 0 ? "+" + temp.ToString() : temp.ToString()), 
                Padding = new Thickness(0) 
            };
            dayValues.Children.Add(dayTemp);
            panel.Children.Add(dayValues);
        }

        private Border GetForecastElement(Forecast dayForecast, Forecast nightForecast)
        {
            var borderResult = new Border { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Margin = new Thickness(0, 2, 0, 2) };
            var dockPanel = new DockPanel();
            
            if (dayForecast != null)
            {
                var dayStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
                fillStackPanel(ref dayStackPanel, dayForecast);
                dockPanel.Children.Add(dayStackPanel);
            }

            if (nightForecast != null)
            {
                var nightStackPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(0, 0, 10, 0)
                    };
                fillStackPanel(ref nightStackPanel, nightForecast);
                dockPanel.Children.Add(nightStackPanel);
            }

            borderResult.Child = dockPanel;
            return borderResult;
        }

        private void shortInfoClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void updateClick(object sender, RoutedEventArgs e)
        {
            fillForm();
        }
    }
}
