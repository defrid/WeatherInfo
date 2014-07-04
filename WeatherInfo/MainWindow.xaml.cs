using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
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
using System.Windows.Threading;
using WeatherInfo.Classes;

namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ForecastDay[] shrtForecast;
        private ForecastDay[] dtldForecast;
        private XMLParser forecasts;
        private string town;
        private string townID;
        private const int BaseRowCount = 2;
        private const int BaseColumnCount = 2;
        private const string HourTitle = "Почасовой прогноз";
        private const string HoutTimeEnd = ":00";
        private Dictionary<string, string> dayParts;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            town = App.settings.city.cityName;
            townID = App.settings.city.cityId.ToString();

            forecasts = new XMLParser(town, townID);

            InitializeComponent();

            
            using (var stream=new MemoryStream())
            {
                Properties.Resources.Gear.Save(stream,ImageFormat.Png);
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption=BitmapCacheOption.OnLoad;
                image.EndInit();
                SettingsImage.Source = image;
            }


            //SettingsImage.Source = new BitmapImage(new Uri(Properties.Resources.Gear));
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromMinutes(App.settings.updatePeriod);

            shrtForecast = forecasts.getBigForecast();
            dtldForecast = forecasts.getDetailedWeek();

            dayParts = new Dictionary<string, string>();
            dayParts.Add("morning", "Утро");
            dayParts.Add("day", "День");
            dayParts.Add("evening", "Вечер");
            dayParts.Add("night", "Ночь");

            fillTable();
            timer.Start();
            Tray.SetupTray(this, options, expandShort);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            applySettings();
        }


        private void fillTable()
        {
            forecasts = new XMLParser(town, townID);

            shrtForecast = forecasts.getBigForecast();
            dtldForecast = forecasts.getDetailedWeek();

            DateTime date = DateTime.Parse(shrtForecast[0].date, CultureInfo.InvariantCulture);
            int dayOfWeek = (int)date.DayOfWeek - 1;
            int index = 0;
            City.Content = town;
            MonthYear.Content = date.ToString("y");
            for (int i = 0; i < 2; i++)
            {
                for (; dayOfWeek < 7; dayOfWeek++)
                {
                    WeatherTable.Children.Add(GetWeaterElement(dayOfWeek, i + 1, index));
                    index++;
                }
                dayOfWeek = 0;
            }
        }


        private Grid GetWeaterElement(int column, int row, int index)
        {
            ForecastDay fore = shrtForecast[index];
            var gridResult = new Grid();
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
                    Content = (fore.max > 0 ? "+" + fore.max.ToString() : fore.max.ToString()),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    FontSize = 15,
                    FontWeight = FontWeights.Bold
                };
            maxTempLabel.SetValue(Grid.ColumnProperty, 1);
            maxTempLabel.SetValue(Grid.ColumnSpanProperty, 2);
            gridResult.Children.Add(maxTempLabel);
            var minTempLabel = new Label()
                {
                    Content = (fore.min > 0 ? "+" + fore.min.ToString() : fore.min.ToString()),
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
            ToolTipService.SetShowDuration(gridResult, 15000);
            if (index < 2)
            {
                ForecastHour[] fors = dtldForecast[index].hours.ToArray().Take(24).ToArray();
                int temp = 0;
                fors = fors.Where(el => Int32.TryParse(el.time, out temp)).ToArray();
                if (index == 0)
                {
                    int curHour = DateTime.Now.Hour;
                    fors = fors.Where(el => Int32.Parse(el.time) >= curHour).ToArray();
                }
                int rows = rowsAndColumns(fors.Length)[0];
                int cols = rowsAndColumns(fors.Length)[1];
                gridResult.ToolTip = GetTooltipForecast(rows, cols, HourTitle, fors, HoutTimeEnd);
                return gridResult;
            }
            if (index < 10)
            {
                ForecastHour[] fors = dtldForecast[index].hours.ToArray();
                int temp = 0;
                fors = fors.Where(el => !Int32.TryParse(el.time, out temp)).ToArray();
                foreach(var el in fors)
                {
                    if(dayParts!=null)
                    el.time = dayParts[el.time];
                }
                gridResult.ToolTip = GetTooltipForecast(BaseRowCount, BaseColumnCount, "Суточный прогноз", fors, "");
            }
            return gridResult;
        }

        private int[] rowsAndColumns(int len)
        {
            if (len > 10)
            {
                int row = len % 3 == 0 ? len / 3 : len / 3 + 1;
                return new int[] { row, 3 };
            }
            if (len > 2)
            {
                int row = len % 2 == 0 ? len / 2 : len / 2 + 1;
                return new int[] { row, 2 };
            }
            return new int[] { 1, 2 };
        }

        /// <summary>
        /// Метод для получения всплывающего окна с прогнозом
        /// </summary>
        /// <param name="rowsCount">Кол-во строк</param>
        /// <param name="columnsCount">Кол-во столбцов</param>
        /// <param name="title">Заголовок</param>
        /// <param name="forecasts">массив прогнозов</param>
        /// <param name="timeEnd">Постфикс ко времени</param>
        /// <returns>Возвращает DockPanel c прогнозами или null(прогнозов меньше числа ячеек)</returns>
        private DockPanel GetTooltipForecast(int rowsCount, int columnsCount,
                                            string title, ForecastHour[] forecasts, string timeEnd)
        {
            //if (forecasts.Count() < rowsCount * columnsCount)
            // return null;
            int remain = forecasts.Length;
            var docResult = new DockPanel();
            var titleLabel = new Label
                {
                    Content = title,
                    FontWeight = FontWeights.Bold,
                    FontStyle = FontStyles.Italic
                };
            DockPanel.SetDock(titleLabel, Dock.Top);
            docResult.Children.Add(titleLabel);

            var grid = new Grid();
            for (var i = 0; i < rowsCount; i++)
                grid.RowDefinitions.Add(new RowDefinition());
            for (var i = 0; i < columnsCount; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            for (var i = 0; i < columnsCount; i++)
                for (var j = 0; j < rowsCount && remain != 0; j++)
                {
                    ForecastHour adding = forecasts[rowsCount * i + j];
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
                        Content = adding.time + timeEnd
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
                    remain--;
                }
            docResult.Children.Add(grid);
            return docResult;
        }

        //private DockPanel GetFourTime

        
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

        public void applySettings()
        {
            WeatherTable.Children.RemoveRange(7, 14);
            town = App.settings.city.cityName;
            townID = App.settings.city.cityId.ToString();

            timer.Stop();
            forecasts = new XMLParser(town, townID);
            fillTable();
            timer.Interval = TimeSpan.FromMinutes(App.settings.updatePeriod);
            timer.Start();
        }

        void options()
        {
            new SettingsWindow(this).Show();
        }


        private void SettingsImage_MouseLeave(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Leave");
        }

        private void SettingsImage_MouseEnter(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Enter");
        }

        private void SettingsImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new SettingsWindow(this).Show();
        }


    }
}
