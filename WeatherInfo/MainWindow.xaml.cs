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
            WeatherTable.Children.Add(GetWeaterElement(1, 1, "2", "+16", "+32", "01d"));
        }

        private static Grid GetWeaterElement(int column, int row, string day, string minTemp, string maxTemp, string imageId)
        {
            var gridResult = new Grid();
            gridResult.SetValue(Grid.RowProperty,row);
            gridResult.SetValue(Grid.ColumnProperty,column);
            for (var i = 0; i < 2; i++)
            {
                gridResult.RowDefinitions.Add(new RowDefinition());
                gridResult.ColumnDefinitions.Add(new ColumnDefinition());
            }
            gridResult.RowDefinitions.Add(new RowDefinition());
            var specRowDef = new ColumnDefinition {Width = new GridLength(1.2, GridUnitType.Star)};
            gridResult.ColumnDefinitions.Add(specRowDef);
            var dayLabel = new Label {Content = day,FontWeight = FontWeights.Bold};
            gridResult.Children.Add(dayLabel);
            var maxTempLabel = new Label()
                {
                    Content = maxTemp,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    FontSize = 15,
                    FontWeight = FontWeights.Bold
                };
            maxTempLabel.SetValue(Grid.ColumnProperty,1);
            maxTempLabel.SetValue(Grid.ColumnSpanProperty,2);
            gridResult.Children.Add(maxTempLabel);
            var minTempLabel = new Label()
                {
                    Content = minTemp,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
            minTempLabel.SetValue(Grid.RowProperty,1);
            minTempLabel.SetValue(Grid.ColumnProperty,2);
            gridResult.Children.Add(minTempLabel);
            var image = new Image
                {
                    Source = new BitmapImage(
                        new Uri(WeatherAPI.ImageRequestString + String.Format("{0}.png", imageId)))
                };
            image.SetValue(Grid.RowProperty,1);
            image.SetValue(Grid.RowSpanProperty,2);
            image.SetValue(Grid.ColumnSpanProperty,2);
            gridResult.Children.Add(image);
            return gridResult;
        }

        /*
         <Image Source="http://openweathermap.org/img/w/10d.png" Grid.Row="1"  Grid.RowSpan="2" Grid.ColumnSpan="2"></Image> 
         
         */
        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch(this.WindowState)
            {
                case System.Windows.WindowState.Minimized:
                    Tray.Update(new Forecast(10, 12, "clouds", "date", "04d"));
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
