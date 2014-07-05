﻿﻿using System;
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
        DispatcherTimer timer;

        private DispatcherTimer rotationTimer;
        private int rotationAngle = 0;

        public MainWindow()
        {
            town = App.settings.GetFirstCity().city.cityName;
            townID = App.settings.GetFirstCity().city.cityId.ToString();

            forecasts = new XMLParser(town, townID);

            InitializeComponent();

            SettingsImage.Source = ConvertBitmabToImage(Properties.Resources.Gear);
            rotationTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 10) };
            rotationTimer.Tick += rotationTimer_Tick;

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

        /// <summary>
        /// Обработчик таймера для поворота шестерни
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void rotationTimer_Tick(object sender, EventArgs e)
        {
            rotationAngle += 1;
            SettingsImage.RenderTransform = new RotateTransform(rotationAngle);
            if (rotationAngle == 360)
                rotationAngle = 0;

        }

        void timer_Tick(object sender, EventArgs e)
        {
            applySettings();
        }

        /// <summary>
        /// Метод для перевода Bitmap в BitmapImage
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        private BitmapImage ConvertBitmabToImage(System.Drawing.Bitmap bitmapImage)
        {
            using (var stream = new MemoryStream())
            {
                bitmapImage.Save(stream, ImageFormat.Png);
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                return image;
            }
        }

        private DockPanel GetContainerForCity()
        {
            var docResult = new DockPanel {Margin = new Thickness(10)};
            
            var docPanelCityYear = new DockPanel();
            DockPanel.SetDock(docPanelCityYear, Dock.Top);

            var cityLabel = new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(50, 5, 50, 5)
                    //добавить в содержимое название города
                };
            docPanelCityYear.Children.Add(cityLabel);

            var monthYearLabel = new Label
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(50, 5, 50, 5)
                    //добавить в содержимое месяц и год
                };
            docPanelCityYear.Children.Add(monthYearLabel);

            var gridBorder = new Border {BorderBrush = Brushes.Black, BorderThickness = new Thickness(1)};

            var weatherGrid = new Grid() {ShowGridLines = true, MinWidth = 580, MinHeight = 170};
            var dayStyle = new Style(){TargetType = typeof(Label)};
            dayStyle.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));
            weatherGrid.Resources.Add("CenterAlligment",dayStyle);
            for (var i = 0; i < 7;i++ )
                weatherGrid.ColumnDefinitions.Add(new ColumnDefinition());
            weatherGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(30)});
            weatherGrid.RowDefinitions.Add(new RowDefinition());
            weatherGrid.RowDefinitions.Add(new RowDefinition());

            //Добавление дней недель и погодных ячеек

            gridBorder.Child = weatherGrid;

            docResult.Children.Add(gridBorder);
            return docResult;
        }

        // <DockPanel Margin="10">
        //    <DockPanel DockPanel.Dock="Top">
        //        <Label Name="City" HorizontalAlignment="Left" Margin="50 5" Content="City"/>
        //        <DockPanel HorizontalAlignment="Right" VerticalAlignment="Center">
        //            <Label Name="MonthYear" Margin="50 5" Content="Month Year" VerticalAlignment="Center"/>
        //            <Image Name="SettingsImage" RenderTransformOrigin="0.5 0.5" HorizontalAlignment="Right" Margin="0 0 20 0" Height="16" VerticalAlignment="Center" MouseLeave="SettingsImage_MouseLeave" MouseEnter="SettingsImage_MouseEnter" MouseDown="SettingsImage_MouseDown"/>
        //        </DockPanel>
        //    </DockPanel>
        //    <Border BorderBrush="Black" BorderThickness="1">
        //        <Grid Name="WeatherTable" ShowGridLines="True" MinWidth="580" MinHeight="170">
        //            <Grid.Resources>
        //                <Style TargetType="Label">
        //                    <Setter Property="HorizontalAlignment" Value="Center"></Setter>
        //                </Style>
        //            </Grid.Resources>
        //            <Grid.ColumnDefinitions>
        //                <ColumnDefinition/>
        //                <ColumnDefinition/>
        //                <ColumnDefinition/>
        //                <ColumnDefinition/>
        //                <ColumnDefinition/>
        //                <ColumnDefinition/>
        //                <ColumnDefinition/>
        //            </Grid.ColumnDefinitions>
        //            <Grid.RowDefinitions>
        //                <RowDefinition Height="30"/>
        //                <RowDefinition/>
        //                <RowDefinition/>
        //            </Grid.RowDefinitions>
        //            <Border BorderBrush="Black" BorderThickness="1">
        //                <Label Content="ПН"/>
        //            </Border>
        //            <Border Grid.Column="1" BorderBrush="Black" BorderThickness="1">
        //                <Label Content="ВТ" />
        //            </Border>
        //            <Border Grid.Column="2" BorderBrush="Black" BorderThickness="1">
        //                <Label Content="СР"/>
        //            </Border>
        //            <Border Grid.Column="3" BorderBrush="Black" BorderThickness="1">
        //                <Label Content="ЧТ" />
        //            </Border>
        //            <Border Grid.Column="4" BorderBrush="Black" BorderThickness="1">
        //                <Label Content="ПТ" />
        //            </Border>
        //            <Border Grid.Column="5" BorderBrush="Black" BorderThickness="1">
        //                <Label Content="СБ" />
        //            </Border>
        //            <Border Grid.Column="6" BorderBrush="Black" BorderThickness="1">
        //                <Label Content="ВС" />
        //            </Border>
        //        </Grid>
        //    </Border>
        //</DockPanel>

        private void fillTable()
        {
            WeatherTable.Children.Clear();
            forecasts = new XMLParser(town, townID);

            shrtForecast = forecasts.getBigForecast();
            dtldForecast = forecasts.getDetailedWeek();

            DateTime curDay = DateTime.Now;
            for (int i = 0; i < 7; i++)
            {
                Label day = new Label()
                {
                    Content = curDay.ToString("ddd")
                };
                Grid.SetRow(day, 0);
                Grid.SetColumn(day, i);
                WeatherTable.Children.Add(day);
                curDay = curDay.AddDays(1);
            }
            int index = 0;
            City.Content = town;
            MonthYear.Content = DateTime.Now.ToString("y");
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    WeatherTable.Children.Add(GetWeaterElement(j, i + 1, index));
                    index++;
                }
            }
        }

        private Grid dayOfWeek(int column, string day)
        {
            var gridResult = new Grid();
            gridResult.SetValue(Grid.RowProperty, 0);
            gridResult.SetValue(Grid.ColumnProperty, column);
            return gridResult;
        }

        /// <summary>
        /// Метод для создания элемента прогнозы для сетки
        /// </summary>
        /// <param name="column">Номер столбца</param>
        /// <param name="row">Номер строки</param>
        /// <param name="index">Номер добавляемого дня(в массиве полученных дней)</param>
        /// <returns></returns>
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
                foreach (var el in fors)
                {
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
                    Tray.Update(forecasts.getCurHour(), 1.5f, false);
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
            town = App.settings.GetFirstCity().city.cityName; //работа с несколькими городами, cities - список городов, для каждого хранятся настройки.
            townID = App.settings.GetFirstCity().city.cityId.ToString(); //работа с несколькими городами, cities - список городов, для каждого хранятся настройки.

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

        /// <summary>
        /// Обработчик собывать MouseLeave для шестерни
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsImage_MouseLeave(object sender, MouseEventArgs e)
        {
            rotationTimer.Stop();
        }
        /// <summary>
        /// Обработчик собывать MouseEnter для шестерни
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsImage_MouseEnter(object sender, MouseEventArgs e)
        {
            rotationTimer.Start();

        }
        /// <summary>
        /// Обработчик собывать MouseDown для шестерни
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new SettingsWindow(this).Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    Two_Windows tw = new Two_Windows(this, new MainWindow());
                    break;
            }
        }

    }
}
