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
            if (temperature > 0)
            {
                temp.Content = "+" + temperature.ToString();
                temp.Foreground = Brushes.Red;
            }
            if (temperature == 0) temp.Content = "0";
            if (temperature < 0)
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
        /// когда хотим в опции
        /// </summary>
        public static event TrayVoid OnOptionsClick;

        /// <summary>
        /// когда хотим обратно в окно :)
        /// </summary>
        public static event TrayVoid onToWindow;

        static TaskbarIcon notifyIcon;
        static Window windowMain;
        
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

            TextBlock FullWindow = new TextBlock();
            TextBlock Options = new TextBlock();
            TextBlock Exit = new TextBlock();
            FullWindow.Text = "Развернуть";
            Options.Text = "Настройки";
            Exit.Text = "Выход";

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

        static void notifyIcon_TrayRightMouseDown(object sender, RoutedEventArgs e)
        {
            notifyIcon.ContextMenu.Visibility = Visibility.Visible;
        }

        static void notifyIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            notifyIcon.TrayPopup.Visibility = Visibility.Visible;
        }

        static void Exit_MouseDown(object sender, RoutedEventArgs e)
        {
            windowMain.Close();
        }

        static void Options_MouseDown(object sender, RoutedEventArgs e)
        {
            //notifyIcon.Visibility = Visibility.Hidden;
            notifyIcon.ContextMenu.Visibility = Visibility.Hidden;
            notifyIcon.TrayPopup.Visibility = Visibility.Hidden;

            windowMain.Show();
            windowMain.Focus();
            windowMain.Activate();

            OnOptionsClick();
        }

        static void FullWindow_MouseDown(object sender, RoutedEventArgs e)
        {
            //notifyIcon.Visibility = Visibility.Hidden;
            notifyIcon.ContextMenu.Visibility = Visibility.Hidden;
            notifyIcon.TrayPopup.Visibility = Visibility.Hidden;

            windowMain.Show();
            windowMain.Activate();
            windowMain.Focus();

            onToWindow();
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
            notifyIcon.Visibility = Visibility.Visible;
            notifyIcon.ToolTipText = "WeatherInfo загружает данные";
            preLoadGraphics.DrawImage(preLoadImage, 0, 0, 100, 100);
            timer.Start();
        }

        private static void rotationTimer_Tick(object sender, EventArgs e)
        {
            preLoadGraphics.Clear(System.Drawing.Color.Transparent);
            preLoadGraphics.TranslateTransform(50, 50);
            preLoadGraphics.RotateTransform(1f);
            preLoadGraphics.TranslateTransform(-50, -50);
            preLoadGraphics.DrawImage(preLoadImage, 0, 0, 100, 100);
            //notifyIcon.Icon = System.Drawing.Icon.FromHandle(preLoadCanvas.GetHicon());
        }

        /// <summary>
        /// скроет главное окно если открыто, запишет данные в трей, покажет его, сам трей будет с иконкой 1го города
        /// </summary>
        /// <param name="cities">список городов</param>
        /// <param name="needHide">если главное окно не надо прятать послать false</param>
        public static void Update(List<TrayCityData> cities, bool needHide=true)
        {
            timer.Stop();
            notifyIcon.ToolTipText = "Левая кнопка мыши - краткий прогноз, Правая кнопка мыши - открыть меню";
            notifyIcon.Icon = System.Drawing.Icon.FromHandle(cities[0].icon.GetHicon());

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

        static void leaveWindowTray()
        {
            notifyIcon.TrayPopup.Visibility = Visibility.Hidden;
            
        }

        private static void ApplicationExit(object sender, ExitEventArgs e)
        {
            notifyIcon.Dispose();
        }
    }
}
