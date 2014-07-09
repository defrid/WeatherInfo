using System;
using System.Windows;
using System.Windows.Media.Imaging;
using WeatherInfo.Classes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Hardcodet.Wpf.TaskbarNotification;
using System.Threading;
using System.Windows.Threading;
using WeatherInfo.Properties;
using Tomers.WPF.Localization;

namespace WeatherInfo
{
    public class TrayCityData
    {
        public Label name;
        public Label temp;
        public System.Drawing.Bitmap icon;

        /// <summary>
        /// Класс для города в трее
        /// </summary>
        /// <param name="name">Название города.</param>
        /// <param name="temp">Температура</param>
        /// <param name="icon">Иконка погоды</param>
        /// <param name="format">Формат градусов (по умолчанию градусы цельсия) - это просто будет приписано к градусам</param>
        public TrayCityData(string name, int temperature, System.Drawing.Bitmap icon, string format="°С" )
        {
            this.name = new Label();
            this.name.Content = name;

            this.icon = icon;

            this.temp = new Label();
            if (temperature > 0 && !format.Equals("K"))
            {
                temp.Content = "+" + temperature.ToString();
                temp.Foreground = Brushes.Red;
            }
            if (temperature == 0) temp.Content = "0";
            if (temperature < 0 || format.Equals("K"))
            {
                temp.Content = temperature.ToString();
                temp.Foreground = Brushes.Blue;
            }
            temp.Content += format;
        }
    }

    public class Tray
    {
        public delegate void TrayVoid();

        /// <summary>
        /// Когда хотим в опции
        /// </summary>
        public static event TrayVoid OnOptionsClick;

        /// <summary>
        /// Когда хотим обратно в окно
        /// </summary>
        public static event TrayVoid onToWindow;

        static TaskbarIcon notifyIcon;
        static Window windowMain;

        static TextBlock FullWindow;
        static TextBlock Options;
        static TextBlock Exit;
        
        /// <summary>
        /// Необходимо вызвать этот метод в самом начале работы программы
        /// </summary>
        /// <param name="main">окно, которое трей сочтет главным и будет его скрывать</param>
        /// <param name="onOptionsClick">сюда послать метод, который будет обрабатывать нажатие не опции</param>
        /// <param name="toWindow">сюда послать метод, совершающий действия, при развертывании трея</param>
        public static void SetupTray(Window main, TrayVoid onOptionsClick, TrayVoid toWindow)
        {
            OnOptionsClick += onOptionsClick;
            onToWindow += toWindow;
            windowMain = main;
            System.Windows.Application.Current.Exit += ApplicationExit;

            notifyIcon = new TaskbarIcon();
            //notifyIcon.Visibility = Visibility.Visible;

            FullWindow = new TextBlock();
            Options = new TextBlock();
            Exit = new TextBlock();

            SetTrayMenu();
            //FullWindow.Text = LanguageDictionary.Current.Translate<string>("menuFullWindow_Tray", "Content");//"Развернуть";
            //Options.Text = LanguageDictionary.Current.Translate<string>("menuOptions_Tray", "Content"); //"Настройки";
            //Exit.Text = LanguageDictionary.Current.Translate<string>("menuExit_Tray", "Content"); //"Выход";

            notifyIcon.ContextMenu = new ContextMenu();
            notifyIcon.ContextMenu.Items.Add(FullWindow);
            notifyIcon.ContextMenu.Items.Add(Options);
            notifyIcon.ContextMenu.Items.Add(Exit);

            FullWindow.MouseLeftButtonDown += FullWindow_MouseDown;
            Options.MouseLeftButtonDown += Options_MouseDown;
            Exit.MouseLeftButtonDown += Exit_MouseDown;
            notifyIcon.TrayMouseDoubleClick += FullWindow_MouseDown;
            notifyIcon.TrayLeftMouseDown += notifyIcon_TrayLeftMouseDown;
            notifyIcon.TrayRightMouseDown += notifyIcon_TrayRightMouseDown;

            timer.Tick += rotationTimer_Tick;
            preLoadCanvas = new System.Drawing.Bitmap(100, 100);
            preLoadGraphics = System.Drawing.Graphics.FromImage(preLoadCanvas);
            preLoadImage = Properties.Resources.Gear;
        }

        private static void SetTrayMenu()
        {
            try
            {
                FullWindow.Text = LanguageDictionary.Current.Translate<string>("menuFullWindow_Tray", "Content");//"Развернуть";
                Options.Text = LanguageDictionary.Current.Translate<string>("menuOptions_Tray", "Content"); //"Настройки";
                Exit.Text = LanguageDictionary.Current.Translate<string>("menuExit_Tray", "Content"); //"Выход";
            }
            catch { }
        }

        static void notifyIcon_TrayRightMouseDown(object sender, RoutedEventArgs e)
        {
            try
            {
                notifyIcon.ContextMenu.Visibility = Visibility.Visible;
            }
            catch { }
        }

        static void notifyIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            try
            {
                notifyIcon.TrayPopup.Visibility = Visibility.Visible;
            }
            catch { }
        }

        static void Exit_MouseDown(object sender, RoutedEventArgs e)
        {
            try
            {
                windowMain.Close();
            }
            catch { }
        }

        static void Options_MouseDown(object sender, RoutedEventArgs e)
        {
            try
            {
                //notifyIcon.Visibility = Visibility.Hidden;
                notifyIcon.ContextMenu.Visibility = Visibility.Hidden;
                notifyIcon.TrayPopup.Visibility = Visibility.Hidden;

                windowMain.Show();
                windowMain.Focus();
                windowMain.Activate();

                OnOptionsClick();
            }
            catch { }
        }

        static void FullWindow_MouseDown(object sender, RoutedEventArgs e)
        {
            try
            {
                //notifyIcon.Visibility = Visibility.Hidden;
                notifyIcon.ContextMenu.Visibility = Visibility.Hidden;
                notifyIcon.TrayPopup.Visibility = Visibility.Hidden;

                windowMain.Show();
                windowMain.Activate();
                windowMain.Focus();

                onToWindow();
            }
            catch { }
        }

        static System.Drawing.Bitmap preLoadCanvas;
        static System.Drawing.Bitmap preLoadImage;
        static System.Drawing.Graphics preLoadGraphics;
        static DispatcherTimer timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 10) };

        /// <summary>
        /// Этот метод следует вызвать если программу скрыли, а данные ещё долго будут грузиться
        /// </summary>
        public static void PreLoad()
        {
            try
            {
                notifyIcon.Visibility = Visibility.Visible;
                notifyIcon.ToolTipText = LanguageDictionary.Current.Translate<string>("toolTipTextPreLoad_Tray", "Content");//"WeatherInfo загружает данные";
                preLoadGraphics.DrawImage(preLoadImage, 0, 0, 100, 100);
                timer.Start();
            }
            catch { }
        }

        private static void rotationTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                preLoadGraphics.Clear(System.Drawing.Color.Transparent);
                preLoadGraphics.TranslateTransform(50, 50);
                preLoadGraphics.RotateTransform(1f);
                preLoadGraphics.TranslateTransform(-50, -50);
                preLoadGraphics.DrawImage(preLoadImage, 0, 0, 100, 100);
                notifyIcon.Icon = System.Drawing.Icon.FromHandle(preLoadCanvas.GetHicon());
            }
            catch
            { 
                // Трей! Я запрещаю тебе крашиться! Держись до последней строчки кода!
            }
        }

        /// <summary>
        /// Cкроет главное окно если открыто, запишет данные в трей, покажет его, сам трей будет с иконкой 1го города
        /// </summary>
        /// <param name="cities">Список городов</param>
        /// <param name="needHide">Если главное окно не надо прятать послать false</param>
        public static void Update(List<TrayCityData> cities, bool needHide=true)
        {
            try
            {
                timer.Stop();
                notifyIcon.ToolTipText = LanguageDictionary.Current.Translate<string>("toolTipText_Tray", "Content");//"Левая кнопка мыши - краткий прогноз, Правая кнопка мыши - открыть меню";
                SetTrayMenu();
                try
                {
                    notifyIcon.Icon = System.Drawing.Icon.FromHandle(cities[0].icon.GetHicon());
                }
                catch { }

                if (needHide)
                {
                    windowMain.WindowState = WindowState.Normal;
                    windowMain.Hide();
                }

                WindowTray wt = new WindowTray(leaveWindowTray, cities);
                notifyIcon.TrayPopup = wt;
                notifyIcon.TrayPopup.Visibility = Visibility.Visible;
                notifyIcon.ContextMenu.Visibility = Visibility.Visible;
            }
            catch { }
        }

        static void leaveWindowTray()
        {
            try
            {
                notifyIcon.TrayPopup.Visibility = Visibility.Hidden;
            }
            catch { }
        }

        private static void ApplicationExit(object sender, ExitEventArgs e)
        {
            notifyIcon.Dispose();
        }
    }
}
