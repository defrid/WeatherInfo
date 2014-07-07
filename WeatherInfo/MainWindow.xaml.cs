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
using System.ComponentModel;
using System.Threading;
using System.Net.NetworkInformation;
using WeatherInfo.Classes;
using Entity_base;
using Tomers.WPF.Localization;

namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ForecastDay[] shrtForecast;
        private ForecastDay[] dtldForecast;
        private ForecastHour curForecast;
        private XMLParser forecasts;
        private string town;
        private string townID;
        private const int BaseRowCount = 2;
        private const int BaseColumnCount = 2;
        private string HourTitle = LanguageDictionary.Current.Translate<string>("hourTitle_mainWin", "Content");//"Почасовой прогноз";
        private const string HoutTimeEnd = ":00";
        private bool hasConnection = false;
        private bool connectedToYaAPI = false;
        private bool connectedToOpAPI = false;

        private Dictionary<string, string> dayParts;
        DispatcherTimer timer;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        private DispatcherTimer rotationTimer;
        private int rotationAngle = 0;

        public MainWindow()
        {
            //Пример создания окна графиков
            //WindowGraphics wg = new WindowGraphics();
            //wg.makeDiagram .makeGraphic 
            //wg.Show();
            if (!OneInstance.EnsureSingleInstance())
            {
                this.Close();
            }


            town = App.settings.GetFirstCity().city.cityRusName;
            townID = App.settings.GetFirstCity().city.cityYaId.ToString();

            forecasts = new XMLParser(town, townID);

            InitializeComponent();

            City.Content = town;
            MonthYear.Content = DateTime.Now.ToString("y");

            SettingsImage.Source = ConvertBitmabToImage(Properties.Resources.Gear);
            rotationTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 10) };
            rotationTimer.Tick += rotationTimer_Tick;

            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(4);//FromMinutes(App.settings.updatePeriod);

            worker.DoWork += worker_reload;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            Tray.SetupTray(this, options, expandShort);

            dayParts = new Dictionary<string, string>();
            dayParts.Add("morning", LanguageDictionary.Current.Translate<string>("morning_mainWin", "Content"));//"Утро"
            dayParts.Add("day", LanguageDictionary.Current.Translate<string>("day_mainWin", "Content"));//"День"
            dayParts.Add("evening", LanguageDictionary.Current.Translate<string>("evening_mainWin", "Content"));//"Вечер"
            dayParts.Add("night", LanguageDictionary.Current.Translate<string>("night_mainWin", "Content"));//"Ночь"

            hasConnection = IsNetworkAvailable();
            this.IsEnabled = false;
            Tray.PreLoad();
            Icon = ConvertBitmabToImage(Properties.Resources.weather.ToBitmap());
            var message = (hasConnection) ? LanguageDictionary.Current.Translate<string>("messUpdateStatusInProcess_mainWin", "Content")
                                          : LanguageDictionary.Current.Translate<string>("messUpdateStatusFaildConnection_mainWin", "Content");
            City.Content = message;//(hasConnection) ? "Обновление" : "Нет соединения";
            worker.RunWorkerAsync();
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
        public static BitmapImage ConvertBitmabToImage(System.Drawing.Bitmap bitmapImage)
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

        /// <summary>
        /// Проверяет есть ли доступ к сети
        /// </summary>
        /// <returns>Истина, если подключение есть</returns>
        public static bool IsNetworkAvailable()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // discard because of standard reasons
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;


                // discard virtual cards (virtual box, virtual pc, etc.)
                if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }
            return false;
        }

        /// <summary>
        /// Подключается к серверу в фоновом режиме
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_reload(object sender, DoWorkEventArgs e)
        {
            timer.Stop();
            while (!(hasConnection = IsNetworkAvailable())) ;
            forecasts = new XMLParser(town, townID);
            try
            {
                curForecast = forecasts.getCurHour();
                shrtForecast = forecasts.getBigForecast();
                connectedToOpAPI = true;
            }
            catch { connectedToOpAPI = false; }
            try
            {
                dtldForecast = forecasts.getDetailedWeek();
                connectedToYaAPI = true;
            }
            catch { connectedToYaAPI = false; }
        }

        /// <summary>
        /// Обновляет интерфейс, если подключение успешно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            if (!(connectedToOpAPI || connectedToYaAPI))
            {
                City.Content = LanguageDictionary.Current.Translate<string>("messUpdateStatusFaildConnection_mainWin", "Content");//"Нет соединения";
                timer.Start();
                return;
            }
            City.Content = town;
            fillTable();
            this.IsEnabled = true;
            MonthYear.Content = DateTime.Now.ToString("y");

            List<TrayCityData> listfortray = new List<TrayCityData>();
            //Это надо делать в потоке! ------
            listfortray.Add(new TrayCityData(
                town, curForecast.temp, WeatherInfo.Classes.OpenWeatherAPI.GetImageById(curForecast.icon))); //просто пример
            Tray.Update(listfortray, false);

            timer.Start();
        }

        private DockPanel GetContainerForCity(string cityName, string monthYear, int cityIndex, bool addSettingsIcon = false)
        {
            var docResult = new DockPanel { Margin = new Thickness(10) };

            var docPanelCityYear = new DockPanel();
            DockPanel.SetDock(docPanelCityYear, Dock.Top);

            var cityLabel = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(50, 5, 50, 5),
                Content = cityName
            };
            docPanelCityYear.Children.Add(cityLabel);

            var monthYearLabel = new Label
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(50, 5, 50, 5),
                Content = monthYear
            };
            docPanelCityYear.Children.Add(monthYearLabel);

            var gridBorder = new Border { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };

            var weatherGrid = new Grid() { ShowGridLines = true, MinWidth = 580, MinHeight = 170 };
            var dayStyle = new Style() { TargetType = typeof(Label) };
            dayStyle.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));
            weatherGrid.Resources.Add("CenterAlligment", dayStyle);
            for (var i = 0; i < 7; i++)
                weatherGrid.ColumnDefinitions.Add(new ColumnDefinition());
            weatherGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            weatherGrid.RowDefinitions.Add(new RowDefinition());
            weatherGrid.RowDefinitions.Add(new RowDefinition());

            DateTime curDay = DateTime.Now;
            for (int i = 0; i < 7; i++)
            {
                Label day = new Label()
                {
                    Content = curDay.ToString("ddd")
                };
                Grid.SetRow(day, 0);
                Grid.SetColumn(day, i);
                weatherGrid.Children.Add(day);
                curDay = curDay.AddDays(1);
            }
            var loadCity = App.settings.cities.ElementAt(cityIndex);
            var townRusName = loadCity.city.cityRusName;
            var townYaId = loadCity.city.cityYaId.ToString();
            var xmlParser = new XMLParser(townRusName, townYaId);

            shrtForecast = xmlParser.getBigForecast();
            dtldForecast = xmlParser.getDetailedWeek();

            int index = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    weatherGrid.Children.Add(GetWeaterElement(j, i + 1, index));
                    index++;
                }
            }

            gridBorder.Child = weatherGrid;

            docResult.Children.Add(gridBorder);
            return docResult;
        }

        /// <summary>
        /// Заполнение таблицы погоды
        /// </summary>
        private void fillTable()
        {
            WeatherTable.Children.Clear();

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
            int limit = connectedToOpAPI ? 14 : 10;
            if (!connectedToOpAPI)
                convertDtldToShrt();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 7 && index < limit; j++)
                {
                    WeatherTable.Children.Add(GetWeaterElement(j, i + 1, index));
                    index++;
                }
            }
        }

        /// <summary>
        /// Превращает прогноз яндекса в 10 дневный прогноз, на случай отсутствия соединения с opAPI
        /// </summary>
        private void convertDtldToShrt()
        {
            shrtForecast = new ForecastDay[10];
            for (int i = 0; i < 10; i++)
            {
                ForecastHour fore = dtldForecast[i].hours[2];
                int min = dtldForecast[i].hours[0].temp;
                int max = fore.temp;
                string date = dtldForecast[i].date;
                string icon = fore.icon;
                shrtForecast[i] = new ForecastDay(min, max, null, date, icon);
            }
        }

        /// <summary>
        /// Возвращает табличку текущего дня
        /// </summary>
        /// <param name="column"></param>
        /// <param name="day"></param>
        /// <returns></returns>
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

            var dayLabel = new Label { FontWeight = FontWeights.Bold };
            gridResult.Children.Add(dayLabel);

            ForecastDay fore = shrtForecast[index];
            string day = fore.date.Substring(8, 2);
            dayLabel.Content = day;

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
                    Source = connectedToOpAPI ? new BitmapImage(
                        new Uri(OpenWeatherAPI.ImageRequestString + String.Format("{0}.png", fore.icon)))
                        : YandexWeatherAPI.GetBitmapImageById(fore.icon)
                };
            image.SetValue(Grid.RowProperty, 1);
            image.SetValue(Grid.RowSpanProperty, 2);
            image.SetValue(Grid.ColumnSpanProperty, 2);
            gridResult.Children.Add(image);
            ToolTipService.SetShowDuration(gridResult, 15000);

            
            if (index == 0)
            {
                gridResult.MouseDown += gridResult_MouseDown;
            }
            if (!connectedToYaAPI && index < 10)
            {
                gridResult.ToolTip = LanguageDictionary.Current.Translate<string>("messConnectedToYaAPIResult_mainWin", "Content");//"Нет соединения с одним из серверов погоды";
                return gridResult;
            }
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
                HourTitle = LanguageDictionary.Current.Translate<string>("dailyTitle_mainWin", "Content");//"Почасовой прогноз"
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
                var titleToolTip = LanguageDictionary.Current.Translate<string>("dailyTitle_mainWin", "Content");//"Суточный прогноз"
                gridResult.ToolTip = GetTooltipForecast(BaseRowCount, BaseColumnCount, titleToolTip, fors, "");
            }
            return gridResult;
        }

        void gridResult_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new curWeather(forecasts.getCurHour()).Show();
        }

        /// <summary>
        /// Метод для определения размера сетки Тултипа
        /// </summary>
        /// <param name="len">количество прогнозов в тултипе</param>
        /// <returns>количество строк[0] и стобцов[1]</returns>
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
            if (!connectedToYaAPI)
            {
                titleLabel.Content = LanguageDictionary.Current.Translate<string>("messConnectedToYaAPIResult_mainWin", "Content");//"Нет соединения с сервером погоды";
                return docResult;
            }

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


        void expandShort()
        {
            this.WindowState = System.Windows.WindowState.Normal;
        }


        /// <summary>
        /// Применение настроек и по совместительству обновление таблицы
        /// </summary>
        public void applySettings()
        {
            while (worker.IsBusy)
            {
                var waitForUpdate = LanguageDictionary.Current.Translate<string>("messWaitForUpdate_mainWin", "Content");
                MessageBox.Show(waitForUpdate);
            }
            hasConnection = IsNetworkAvailable();
            var message = (hasConnection) ? LanguageDictionary.Current.Translate<string>("messUpdateStatusInProcess_mainWin", "Content")
                                          : LanguageDictionary.Current.Translate<string>("messUpdateStatusFaildConnection_mainWin", "Content");
            City.Content = message;//(hasConnection) ? "Обновление" : "Нет соединения";
            this.IsEnabled = false;

            MonthYear.Content = DateTime.Now.ToString("y");

            town = App.settings.GetFirstCity().city.cityRusName; //работа с несколькими городами, cities - список городов, для каждого хранятся настройки.
            townID = App.settings.GetFirstCity().city.cityYaId.ToString(); //работа с несколькими городами, cities - список городов, для каждого хранятся настройки.
            timer.Interval = TimeSpan.FromSeconds(5);//TimeSpan.FromMinutes(App.settings.updatePeriod);

            Tray.PreLoad();
            worker.RunWorkerAsync();

            //
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //DataBase data = new DataBase();
            //data.ADD_BD();
            //data.show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new Days(forecasts.getDetailedWeek()).Show();
        }

    }
}
