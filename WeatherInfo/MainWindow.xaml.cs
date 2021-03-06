﻿using System;
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
using DataHandlerInterface.Classes;
using WeatherInfo.Classes;
using Entity_base;
using Tomers.WPF.Localization;
using System.Text.RegularExpressions;

namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ForecastDay[]> shrtForecasts;
        private List<ForecastDay[]> dtldForecasts;
        private List<ForecastHour> curForecasts;
        private List<XMLParser> forecasts;
        //private string town;
        //private string townID;
        private const int BaseRowCount = 2;
        private const int BaseColumnCount = 2;
        private string HourTitle = LanguageDictionary.Current.Translate<string>("hourTitle_mainWin", "Content");//"Почасовой прогноз";
        private string HoutTimeEnd = ":00";
        private bool hasConnection = false;
        private bool connectedToYaAPI = false;
        private bool connectedToOpAPI = false;
        private bool firstLaunch = true;
        private bool isFahr = false;
        private bool isKelv = false;
        private int curIndex = 0;

        private Window[] curForeWindows = new Window[10];

        private Dictionary<string, string> dayParts;
        DispatcherTimer timer;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        private DispatcherTimer rotationTimer;
        private int rotationAngle = 0;

        private List<DockPanel> preloaders;
        private DispatcherTimer preloaderRotationTimer;
        private int preloaderRotationAngle = 0;

        private bool isDailyForecast = false;

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

            InitializeComponent();
            forecasts = new List<XMLParser>();

            isDailyForecast = !App.settings.format.Equals("Short");
            preloaders = new List<DockPanel>();
            var nowMonthYear = DateTime.Now.ToString("y", LanguageContext.Instance.Culture);
            foreach (var city in App.settings.cities)
            {
                var town = (App.settings.language.engName) == "English" ? upperEngCityName(city.city.cityEngName) : city.city.cityRusName;//city.city.cityRusName;
                var townId = city.city.cityYaId.ToString();
                MainContainer.Children.Add(GetContainerForCity(town, nowMonthYear));
                town = city.city.cityEngName;
                forecasts.Add(new XMLParser(town, townId));
            }

            var forecastDetailed = App.dataHandler.LoadForecastDetailed();
            if (forecastDetailed != null)
            {
                shrtForecasts = forecastDetailed.shrtForecasts;
                dtldForecasts = forecastDetailed.dtldForecasts;
                for (curIndex = 0; curIndex < forecasts.Count; curIndex++)
                {
                    if (isDailyForecast)
                    {
                        fillDaily();
                        continue;
                    }
                    fillWeekly();
                }
            }

            firstLaunch = false;

            curIndex = 0;
            isKelv = App.settings.temperatureUnits.rusName.Equals("Кельвины");
            isFahr = App.settings.temperatureUnits.rusName.Equals("Фаренгейты");

            preloaderRotationTimer = new DispatcherTimer();
            preloaderRotationTimer.Interval = TimeSpan.FromMilliseconds(100);
            preloaderRotationTimer.Tick += preloaderRotationTimer_Tick;
            preloaderRotationTimer.Start();

            HelpImage.Source = ConvertBitmabToImage(Properties.Resources.help);

            SettingsImage.Source = ConvertBitmabToImage(Properties.Resources.Gear);
            rotationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            rotationTimer.Tick += rotationTimer_Tick;

            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromMinutes(App.settings.updatePeriod);

            worker.DoWork += worker_reload;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            Tray.SetupTray(this, options, expandShort, help);



            hasConnection = IsNetworkAvailable();
            //Scroll.IsEnabled = false;
            Tray.PreLoad();
            Icon = ConvertBitmabToImage(Properties.Resources.weather.ToBitmap());
            var message = (hasConnection) ? LanguageDictionary.Current.Translate<string>("messUpdateStatusInProcess_mainWin", "Content")
                                          : LanguageDictionary.Current.Translate<string>("messUpdateStatusFaildConnection_mainWin", "Content");

            Connection.Content = message;

            shrtForecasts = new List<ForecastDay[]>();
            dtldForecasts = new List<ForecastDay[]>();
            curForecasts = new List<ForecastHour>();

            worker.RunWorkerAsync();
        }

        void preloaderRotationTimer_Tick(object sender, EventArgs e)
        {
            preloaderRotationAngle += 30;
            if (preloaderRotationAngle >= 360)
                preloaderRotationAngle = 0;
            foreach (var preloader in preloaders)
            {
                var image = (preloader.Children[0] as StackPanel).Children[0] as Image;
                image.RenderTransform = new RotateTransform(preloaderRotationAngle);
            }
        }

        private void SetWindowHeight()
        {
            var screenHeight = SystemParameters.FullPrimaryScreenHeight;
            var likelyHeight = (App.settings.cities.Count - 1) * 270 + 340;
            Height = likelyHeight < screenHeight ? likelyHeight : screenHeight;
            Width = isDailyForecast ? 720 : 640;
        }

        private DockPanel GetPreloadPanel()
        {
            var resultPanel = new DockPanel();
            Grid.SetRowSpan(resultPanel, 2);
            Grid.SetColumnSpan(resultPanel, 7);
            var stackPanel = new StackPanel()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            var image = new Image()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Source = ConvertBitmabToImage(Properties.Resources.Preloader),
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    Height = 32,
                    Width = 32
                };
            stackPanel.Children.Add(image);
            var label = new Label()
            {
                Content = LanguageDictionary.Current.Translate<string>("loadingMess_mainWin", "Content"),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(label);
            resultPanel.Children.Add(stackPanel);
            return resultPanel;
        }

        private Dictionary<string, string> InitDaysDictionary()
        {
            var days = new Dictionary<string, string>();
            days.Add("morning", LanguageDictionary.Current.Translate<string>("morning_mainWin", "Content"));//"Утро"
            days.Add("day", LanguageDictionary.Current.Translate<string>("day_mainWin", "Content"));//"День"
            days.Add("evening", LanguageDictionary.Current.Translate<string>("evening_mainWin", "Content"));//"Вечер"
            days.Add("night", LanguageDictionary.Current.Translate<string>("night_mainWin", "Content"));//"Ночь"
            days.Add("Утро", LanguageDictionary.Current.Translate<string>("morning_mainWin", "Content"));//"Утро"
            days.Add("День", LanguageDictionary.Current.Translate<string>("day_mainWin", "Content"));//"День"
            days.Add("Вечер", LanguageDictionary.Current.Translate<string>("evening_mainWin", "Content"));//"Вечер"
            days.Add("Ночь", LanguageDictionary.Current.Translate<string>("night_mainWin", "Content"));//"Ночь"
            days.Add("Morning", LanguageDictionary.Current.Translate<string>("morning_mainWin", "Content"));//"Утро"
            days.Add("Day", LanguageDictionary.Current.Translate<string>("day_mainWin", "Content"));//"День"
            days.Add("Evening", LanguageDictionary.Current.Translate<string>("evening_mainWin", "Content"));//"Вечер"
            days.Add("Night", LanguageDictionary.Current.Translate<string>("night_mainWin", "Content"));

            return days;
        }

        private string upperEngCityName(string city)
        {
            var curCity = city;
            if (App.settings.language.engName == "English")
            {
                var firstLit = curCity[0].ToString();
                var reg = new Regex(curCity[0].ToString());
                curCity = reg.Replace(curCity, firstLit.ToUpper(), 1);
            }

            return curCity;
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


        private void toKelv(ref ForecastDay[] week)
        {
            foreach (var day in week)
            {
                day.max += 273;
                day.min += 273;
                foreach (var hour in day.hours)
                {
                    hour.temp += 273;
                }
            }
        }

        private void toFahr(ref ForecastDay[] week)
        {
            foreach (var day in week)
            {
                day.max = (int)(day.max * 1.8 + 32);
                day.min = (int)(day.min * 1.8 + 32);
                foreach (var hour in day.hours)
                {
                    hour.temp = (int)(hour.temp * 1.8 + 32);
                }
            }
        }

        private void toKelv()
        {
            var city = shrtForecasts[curIndex];
            foreach (var day in city)
            {
                day.max += 273;
                day.min += 273;
            }

            city = dtldForecasts[curIndex];
            foreach (var day in city)
            {
                day.max += 273;
                day.min += 273;
                foreach (var hour in day.hours)
                {
                    hour.temp += 273;
                }
            }

            curForecasts[curIndex].temp += 273;
        }

        //F = 1.8 C + 32
        private void toFahr()
        {
            var city = shrtForecasts[curIndex];
            foreach (var day in city)
            {
                day.max = (int)(day.max * 1.8 + 32);
                day.min = (int)(day.min * 1.8 + 32);
            }

            city = dtldForecasts[curIndex];
            foreach (var day in city)
            {
                day.max = (int)(day.max * 1.8 + 32);
                day.min = (int)(day.min * 1.8 + 32);
                foreach (var hour in day.hours)
                {
                    hour.temp = (int)(hour.temp * 1.8 + 32);
                }
            }

            curForecasts[curIndex].temp = (int)(curForecasts[curIndex].temp * 1.8 + 32);
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


        private void getRealMaxMin()
        {
            var shrt = shrtForecasts[curIndex];
            var dtld = dtldForecasts[curIndex];

            for (int i = 0; i < dtld.Length; i++)
            {
                var hours = dtld[i].hours;
                int max = hours[0].temp;
                int min = hours[0].temp;
                foreach (var hour in hours)
                {
                    max = (hour.temp > max) ? hour.temp : max;
                    min = (hour.temp < min) ? hour.temp : min;
                }
                shrt[i].max = (shrt[i].max > max) ? shrt[i].max : max;
                shrt[i].min = (shrt[i].min < min) ? shrt[i].min : min;
            }
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
            dayParts = InitDaysDictionary();
            try
            {
                var xmlParser = forecasts[curIndex];
                shrtForecasts.Add(xmlParser.getBigForecast());
                Thread.Sleep(1000);//Время между запросами должно быть не меньше секунды
                curForecasts.Add(xmlParser.getCurHour());
                Thread.Sleep(1000);
                connectedToOpAPI = true;
            }
            catch
            {
                connectedToOpAPI = false;
            }
            try
            {
                var xmlParser = forecasts[curIndex];
                dtldForecasts.Add(xmlParser.getDetailedWeek());
                Thread.Sleep(1000);//Время между запросами должно быть не меньше секунды
                connectedToYaAPI = true;

                //App.settingHandler.SaveForecasrDetailed(new ForecastDetailed(dtldForecasts));
                //dtldForecasts.Clear();
                //dtldForecasts = App.settingHandler.LoadForecastDetailed().forecasts;

            }
            catch
            {
                connectedToYaAPI = false;
            }

            if (connectedToYaAPI && connectedToOpAPI)
            {
                getRealMaxMin();
            }
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
                Connection.Content = LanguageDictionary.Current.Translate<string>("messUpdateStatusFaildConnection_mainWin", "Content");
                worker.RunWorkerAsync();
                return;
            }

            //на тот случай, если прогноз получился криво, ибо через некоторое время сервер может нас послать и дать пустой прогноз
            if ((shrtForecasts.Count == 0 && connectedToOpAPI) || (dtldForecasts.Count == 0 && connectedToYaAPI))
            {
                Connection.Content = LanguageDictionary.Current.Translate<string>("messUpdateStatusFaildConnection_mainWin", "Content");
                worker.RunWorkerAsync();
                return;
            }

            Connection.Content = LanguageDictionary.Current.Translate<string>("messUpdateStatusSuccessConnection_mainWin", "Content");//"Соединение установлено";


            SetWindowHeight();

            if (isKelv)
            {
                toKelv();
            }

            if (isFahr)
            {
                toFahr();
            }

            if (!isDailyForecast)
                fillWeekly();
            else
                fillDaily();

            Scroll.IsEnabled = true;


            if (!connectedToOpAPI)
            {
                return;
            }

            curIndex++;
            if (curIndex == forecasts.Count)
            {
                fillTray();
                preloaderRotationTimer.Stop();
                try
                {
                    ///Пример сохранения погоды в XML
                    var citiesForXml = new List<City>();
                    foreach (var city in App.settings.cities)
                    {
                        citiesForXml.Add(city.city);
                    }
                    App.dataHandler.SaveForecasrDetailed(new ForecastDetailed(dtldForecasts, shrtForecasts, citiesForXml));
                }
                catch (Exception ex)
                {
                    var message = LanguageDictionary.Current.Translate<string>("writeForecastToXml_SttsHandler", "Content");
                    MessageBox.Show(message);
                }
                timer.Start();
            }
            else
            {
                worker.RunWorkerAsync();
            }
        }

        private void fillTray(bool needHide = false)
        {
            List<TrayCityData> listfortray = new List<TrayCityData>();
            for (int i = 0; i < App.settings.cities.Count; i++)
            {
                var name = (App.settings.language.engName) == "English" ? upperEngCityName(App.settings.cities[i].city.cityEngName) : App.settings.cities[i].city.cityRusName;
                var temp = curForecasts[i].temp;
                var icon = WeatherInfo.Classes.OpenWeatherAPI.GetImageById(curForecasts[i].icon);

                string format = "";
                if (isKelv) format = "K";
                else
                    if (isFahr) format = "F";
                    else format = "°С";

                listfortray.Add(new TrayCityData(name, temp, icon, format));
            }
            Tray.Update(listfortray, needHide);
        }

        private StackPanel GetContainerForCity(string cityName, string monthYear)
        {
            var nowMonthYear = DateTime.Now.ToString("y", LanguageContext.Instance.Culture);
            var lang = App.settings.language.engName;

            var stackResult = new StackPanel() { Margin = new Thickness(10) };

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
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(50, 5, 50, 5),
                Content = monthYear
            };
            docPanelCityYear.Children.Add(monthYearLabel);
            stackResult.Children.Add(docPanelCityYear);

            var gridBorder = new Border { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };

            var weatherGrid = new Grid() { ShowGridLines = false, MinWidth = 580, MinHeight = 170 };

            if (isDailyForecast)
            {
                for (var i = 0; i < 5; i++)
                {
                    weatherGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }
                weatherGrid.RowDefinitions.Add(new RowDefinition());
                weatherGrid.RowDefinitions.Add(new RowDefinition());
            }
            else
            {
                for (var i = 0; i < 7; i++)
                    weatherGrid.ColumnDefinitions.Add(new ColumnDefinition());
                weatherGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
                weatherGrid.RowDefinitions.Add(new RowDefinition());
                weatherGrid.RowDefinitions.Add(new RowDefinition());
            }

            var preloadPanel = GetPreloadPanel();
            weatherGrid.Children.Add(preloadPanel);
            preloaders.Add(preloadPanel);

            gridBorder.Child = weatherGrid;

            stackResult.Children.Add(gridBorder);
            var stackButton = new StackPanel()
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Orientation = Orientation.Horizontal,
                };
            DockPanel.SetDock(stackButton, Dock.Bottom);
            var button1 = new Button()
                {
                    Tag = cityName,
                    MinWidth = 100,
                    Margin = new Thickness(50, 3, 30, 3),
                    Content = LanguageDictionary.Current.Translate<string>("oneDayBtn_mainWin", "Content")
                };
            button1.Click += OneDay_Click;
            stackButton.Children.Add(button1);
            var button2 = new Button()
            {
                Tag = cityName,
                MinWidth = 100,
                Margin = new Thickness(30, 3, 30, 3),
                Content = LanguageDictionary.Current.Translate<string>("manyDayBtn_mainWin", "Content")
            };
            button2.Click += ManyDays_Click;
            stackButton.Children.Add(button2);
            var button3 = new Button()
            {
                Tag = cityName,
                MinWidth = 100,
                Margin = new Thickness(30, 3, 50, 3),
                Content = LanguageDictionary.Current.Translate<string>("cloudyBtn_mainWin", "Content")
            };
            button3.Click += Cloudly_Click;
            stackButton.Children.Add(button3);
            stackResult.Children.Add(stackButton);
            return stackResult;
        }

        void OneDay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = (sender as Button).Tag;

                CitySettings engName = null;
                lang wL = lang.eng;

                if (App.settings.language.engName == "English")
                {
                    engName = App.settings.cities.Where(el => el.city.cityEngName.ToLower() == name.ToString().ToLower()).FirstOrDefault();
                }
                else
                {
                    engName = App.settings.cities.Where(el => el.city.cityRusName.ToLower() == name.ToString().ToLower()).FirstOrDefault();
                    wL = lang.rus;
                }

                string Ename = engName.city.cityEngName;
                var parser = forecasts.Where(el => el.town == Ename).FirstOrDefault();
                var graphic = new WindowGraphics(wL);
                ForecastDay[] days = parser.getDetailedWeek();
                if (isKelv)
                {
                    toKelv(ref days);
                }
                if (isFahr)
                {
                    toFahr(ref days);
                }
                var day = days[0];
                day.hours = day.hours.Take(24).ToList();
                graphic.makeGraphic(day, name.ToString());
                graphic.Show();
            }
            catch { }
        }


        void ManyDays_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = (sender as Button).Tag;

                CitySettings engName = null;
                lang wL = lang.eng;

                if (App.settings.language.engName == "English")
                {
                    engName = App.settings.cities.Where(el => el.city.cityEngName.ToLower() == name.ToString().ToLower()).FirstOrDefault();
                }
                else
                {
                    engName = App.settings.cities.Where(el => el.city.cityRusName.ToLower() == name.ToString().ToLower()).FirstOrDefault();
                    wL = lang.rus;
                }

                string Ename = engName.city.cityEngName;

                var parser = forecasts.Where(el => el.town == Ename).FirstOrDefault();
                var graphic = new WindowGraphics(wL);
                ForecastDay[] days = parser.getDetailedWeek();
                if (isKelv)
                {
                    toKelv(ref days);
                }
                if (isFahr)
                {
                    toFahr(ref days);
                }
                graphic.makeGraphic(days.ToList(), name.ToString());
                graphic.Show();
            }
            catch { }
        }

        void Cloudly_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = (sender as Button).Tag;

                CitySettings engName = null;
                lang wL = lang.eng;

                if (App.settings.language.engName == "English")
                {
                    engName = App.settings.cities.Where(el => el.city.cityEngName.ToLower() == name.ToString().ToLower()).FirstOrDefault();
                }
                else
                {
                    engName = App.settings.cities.Where(el => el.city.cityRusName.ToLower() == name.ToString().ToLower()).FirstOrDefault();
                    wL = lang.rus;
                }

                string Ename = engName.city.cityEngName;

                var parser = forecasts.Where(el => el.town == Ename).FirstOrDefault();

                int index = forecasts.FindIndex(el => el == parser);

                var graphic = new WindowStolbiki();
                List<ForecastDay> days = shrtForecasts[index].ToList();

                graphic.Show();
                graphic.makeDiagram(days, name.ToString());

            }
            catch { }
        }


        private void fillDaily()
        {
            var weatherresults = MainContainer.Children.Cast<Panel>();
            var weatherTable = weatherresults.Select(weatherContainer => (weatherContainer.Children[1] as Border).Child as Grid).ToList()[curIndex];
            weatherTable.Children.Clear();

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var index = 5 * i + j;
                    int temp = 0;
                    ForecastHour[] parts = dtldForecasts[curIndex][index].hours.Where(el => !Int32.TryParse(el.time, out temp)).ToArray();

                    dayParts = InitDaysDictionary();
                    foreach (var el in parts)
                    {
                        el.time = dayParts[el.time];
                    }

                    DateTime date = DateTime.Parse(dtldForecasts[curIndex][index].date);
                    var dateDay = date.ToString("m", LanguageContext.Instance.Culture) + " " + date.ToString("ddd", LanguageContext.Instance.Culture);
                    var weatherElement = GetDailyWeatherElement(i, j, dateDay, parts);
                    weatherTable.Children.Add(weatherElement);
                }
            }

        }

        /// <summary>
        /// Заполнение таблицы погоды кратким
        /// </summary>
        private void fillWeekly()
        {
            //preloaderRotationTimer.Stop();

            var weatherresults = MainContainer.Children.Cast<Panel>();
            var weatherTable = weatherresults.Select(weatherContainer => (weatherContainer.Children[1] as Border).Child as Grid).ToList()[curIndex];
            weatherTable.Children.Clear();
            weatherTable.ShowGridLines = true;

            DateTime curDay = DateTime.Now;

            for (int i = 0; i < 7; i++)
            {
                Label day = new Label()
                    {
                        Content = curDay.ToString("ddd", LanguageContext.Instance.Culture)
                    };
                Grid.SetRow(day, 0);
                Grid.SetColumn(day, i);
                weatherTable.Children.Add(day);
                curDay = curDay.AddDays(1);
            }

            //if (!connectedToOpAPI)
            //    ConvertDtldToShrt();

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    var index = 7 * i + j;
                    if (index >= shrtForecasts[curIndex].Length)
                        break;
                    var weatherElement = GetWeaterElement(j, i + 1, shrtForecasts[curIndex][index]);
                    if (index == 0 && !firstLaunch)
                    {
                        weatherElement.MouseUp += gridResult_MouseUp;
                        weatherElement.Name = "_" + curIndex.ToString();
                    }
                    if (index < 10)
                    {
                        var today = index == 0;
                        if (!connectedToYaAPI && !firstLaunch)
                        {
                            weatherElement.ToolTip = LanguageDictionary.Current.Translate<string>("messConnectedToYaAPIResult_mainWin", "Content");
                        }
                        else
                        {
                            var tooltip = GetSpecialTooltip(dtldForecasts[curIndex][index], today);
                            weatherElement.ToolTip = tooltip;
                        }
                    }
                    weatherTable.Children.Add(weatherElement);
                }
            }
        }

        /// <summary>
        /// Превращает прогноз яндекса в 10 дневный прогноз, на случай отсутствия соединения с opAPI
        /// </summary>
        private void ConvertDtldToShrt()
        {
            shrtForecasts = new List<ForecastDay[]>();
            foreach (var dtldForecast in dtldForecasts)
            {
                var shrtForecast = new ForecastDay[10];
                for (int i = 0; i < 10; i++)
                {
                    ForecastHour fore = dtldForecast[i].hours[2];
                    int min = dtldForecast[i].hours[0].temp;
                    int max = fore.temp;
                    string date = dtldForecast[i].date;
                    string icon = fore.icon;
                    shrtForecast[i] = new ForecastDay(min, max, null, date, icon);
                }
                shrtForecasts.Add(shrtForecast);
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
        /// <param name="shortForecast"></param>
        /// <param name="longForecast"></param>
        /// <returns></returns>
        private Grid GetWeaterElement(int column, int row, ForecastDay shortForecast)
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
            var specColumnDef = new ColumnDefinition { Width = new GridLength(1.6, GridUnitType.Star) };
            gridResult.ColumnDefinitions.Add(specColumnDef);

            var dayLabel = new Label { FontWeight = FontWeights.Bold };
            gridResult.Children.Add(dayLabel);

            string day = shortForecast.date.Substring(8, 2);
            dayLabel.Content = day;

            var maxTempLabel = new Label()
                {
                    Content = getTempString(shortForecast.max),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    FontSize = 15,
                    FontWeight = FontWeights.Bold
                };
            maxTempLabel.SetValue(Grid.ColumnProperty, 1);
            maxTempLabel.SetValue(Grid.ColumnSpanProperty, 2);
            gridResult.Children.Add(maxTempLabel);
            var minTempLabel = new Label()
                {
                    Content = getTempString(shortForecast.min),
                    HorizontalAlignment = HorizontalAlignment.Right
                };
            minTempLabel.SetValue(Grid.RowProperty, 1);
            minTempLabel.SetValue(Grid.ColumnProperty, 2);
            gridResult.Children.Add(minTempLabel);
            if (!firstLaunch)
            {
                var image = new Image
                    {
                        Source = connectedToOpAPI ? new BitmapImage(
                            new Uri(OpenWeatherAPI.ImageRequestString + String.Format("{0}.png", shortForecast.icon)))
                            : YandexWeatherAPI.GetBitmapImageById(shortForecast.icon)
                    };
                image.SetValue(Grid.RowProperty, 1);
                image.SetValue(Grid.RowSpanProperty, 2);
                image.SetValue(Grid.ColumnSpanProperty, 2);
                gridResult.Children.Add(image);
            }
            ToolTipService.SetShowDuration(gridResult, 15000);

            return gridResult;
        }

        private object GetSpecialTooltip(ForecastDay dayForecast, bool today)
        {
            if (!connectedToYaAPI && !firstLaunch)
            {
                return LanguageDictionary.Current.Translate<string>("messConnectedToYaAPIResult_mainWin", "Content");
            }

            if (dayForecast.hours.Count > 24)
            {
                dayParts = InitDaysDictionary();
                int temp = 0;
                ForecastHour[] fors = dayForecast.hours.Where(el => Int32.TryParse(el.time, out temp)).ToArray();
                if (today)
                {
                    int curHour = DateTime.Now.Hour;
                    fors = fors.Where(el => Int32.Parse(el.time) >= curHour).ToArray();
                }
                int rows = fors.Length != 24 ? rowsAndColumns(fors.Length)[0] : 8;
                int cols = fors.Length != 24 ? rowsAndColumns(fors.Length)[1] : 3;
                HourTitle = LanguageDictionary.Current.Translate<string>("dailyTitle_mainWin", "Content");
                return GetTooltipForecast(rows, cols, HourTitle, fors, HoutTimeEnd);
            }
            else
            {
                //ForecastHour[] fors = dtldForecast[index].hours.ToArray();
                //dayParts = InitDaysDictionary();

                dayParts = InitDaysDictionary();

                int temp = 0;
                ForecastHour[] fors = dayForecast.hours.Where(el => !Int32.TryParse(el.time, out temp)).ToArray();
                foreach (var el in fors)
                {
                    el.time = dayParts[el.time];
                }
                var titleToolTip = LanguageDictionary.Current.Translate<string>("dailyTitle_mainWin", "Content");
                return GetTooltipForecast(BaseRowCount, BaseColumnCount, titleToolTip, fors, "");
            }
        }

        private Border GetDailyWeatherElement(int row, int column, string dateDay, ForecastHour[] hourForecasts)
        {
            var borderResult = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(column == 0 ? 1 : 0, 1, column == 4 ? 1 : 0, 1)
            };
            Grid.SetColumn(borderResult, column);
            Grid.SetRow(borderResult, row);
            var gridElement = new Grid();
            for (var i = 0; i < 4; i++)
            {
                gridElement.RowDefinitions.Add(new RowDefinition());
                gridElement.ColumnDefinitions.Add(new ColumnDefinition());
            }

            var borderDay = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Child = new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = dateDay
                }
            };
            Grid.SetColumnSpan(borderDay, 4);
            gridElement.Children.Add(borderDay);

            for (var i = 0; i < 4; i++)
            {
                var rowForecast = i / 2 == 0 ? 1 : 2;
                var columnForecast = i % 2 == 0 ? 0 : 2;
                var borderForecast = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
                Grid.SetColumn(borderForecast, columnForecast);
                Grid.SetRow(borderForecast, rowForecast);
                Grid.SetColumnSpan(borderForecast, 2);

                var stackForecast = new StackPanel { Orientation = Orientation.Horizontal };
                var dayLabel = new Label
                {
                    Padding = new Thickness(0),
                    VerticalAlignment = VerticalAlignment.Center,
                    Content = hourForecasts[i].time[0] + ": " //Первый символ времени
                };
                stackForecast.Children.Add(dayLabel);
                if (!firstLaunch)
                {
                    var imageForecast = new Image
                    {
                        Height = 24,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Source = YandexWeatherAPI.GetBitmapImageById(hourForecasts[i].icon)
                    };
                    stackForecast.Children.Add(imageForecast);
                }
                var tempLabel = new Label
                {
                    Padding = new Thickness(0, 0, 3, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    Content = getTempString(hourForecasts[i].temp)//Температура
                };
                stackForecast.Children.Add(tempLabel);
                borderForecast.Child = stackForecast;
                gridElement.Children.Add(borderForecast);
            }

            borderResult.Child = gridElement;
            return borderResult;
        }

        void gridResult_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!connectedToOpAPI)
            {
                return;
            }
            var index = Int32.Parse((sender as FrameworkElement).Name.Remove(0, 1));
            if (curForeWindows[index] == null || !curForeWindows[index].IsVisible)
            {
                curForeWindows[index] = new curWeather(curForecasts[index]);
            }
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

                    if (!firstLaunch)
                    {
                        var bitmapImage = YandexWeatherAPI.GetBitmapImageById(adding.icon);

                        var icon = new Image
                        {
                            Source = bitmapImage,
                            Width = 32,
                            Height = 32
                        };
                        container.Children.Add(icon);
                    }

                    var stringTemp = getTempString(adding.temp);
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
            Connection.Content = message;
            Scroll.IsEnabled = false;

            var mainElementsCount = MainContainer.Children.Cast<Panel>().Count();
            MainContainer.Children.RemoveRange(0, mainElementsCount);

            isDailyForecast = !App.settings.format.Equals("Short");

            isKelv = App.settings.temperatureUnits.rusName.Equals("Кельвины");
            isFahr = App.settings.temperatureUnits.rusName.Equals("Фаренгейты");

            forecasts = new List<XMLParser>();

            preloaders = new List<DockPanel>();
            var nowMonthYear = DateTime.Now.ToString("y", LanguageContext.Instance.Culture);
            foreach (var city in App.settings.cities)
            {
                var town = (App.settings.language.engName) == "English" ? upperEngCityName(city.city.cityEngName) : city.city.cityRusName;//city.city.cityRusName;
                var townId = city.city.cityYaId.ToString();
                MainContainer.Children.Add(GetContainerForCity(town, nowMonthYear));
                town = city.city.cityEngName;
                forecasts.Add(new XMLParser(town, townId));
            }

            preloaderRotationTimer.Start();

            timer.Interval = TimeSpan.FromMinutes(App.settings.updatePeriod);

            Tray.PreLoad();

            shrtForecasts.Clear();
            dtldForecasts.Clear();
            curForecasts.Clear();
            curIndex = 0;

            worker.RunWorkerAsync();

            //
        }

        private string getTempString(int temp)
        {
            return (isKelv ? temp + "K" : (((temp > 0) ? "+" : "") + (isFahr ? temp + "F" : temp.ToString())));
        }

        void options()
        {
            new SettingsWindow(this).Show();
        }

        void help()
        {
            if(!Directory.Exists(@"Config/help"))
            {
                System.Windows.Forms.MessageBox.Show(@"Не найдена папка Config/help");
                return;
            }

            if (!File.Exists(@"Config/help/help.chm"))
            {
                System.Windows.Forms.MessageBox.Show("Не найден файл справки по пути Config/help/help.chm");
                return;
            }

            try
            {
                System.Windows.Forms.Help.ShowHelp(null, @"Config/help/help.chm");
            }
            catch { }
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
        private void SettingsImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            new SettingsWindow(this).Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    //WindowGraphics wg = new WindowGraphics(lang.eng); //пример создания окна графиков и построения диаграммы
                    //wg.makeDiagram(dtldForecast.ToList());
                    //wg.ShowDialog();
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                fillTray(true);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        /// <summary>
        /// Нажатие на "???"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            help();
        }

    }
}
