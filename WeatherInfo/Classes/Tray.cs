using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using WeatherInfo.Classes;

namespace WeatherInfo
{
    public class Tray
    {
        public delegate void TrayVoid();

        public static event TrayVoid OnOptionsClick;
        public static event TrayVoid onToWindow;

        public ForecastHour curFore;

        static string iPdescr = "Двойной клик - развернуть программу, ПКМ - открыть меню";
        static Window windowMain;
        static NotifyIcon iconPicture = new NotifyIcon();
        
        //Устанавливает трей (сюда главное окно и событие на клик опций)
        public static void SetupTray(Window main, TrayVoid onOptionsClick, TrayVoid toWindow)
        {
            OnOptionsClick += onOptionsClick;
            onToWindow += toWindow;

            windowMain = main;

            iconPicture.Text = iPdescr;

            ContextMenu newMenu = new ContextMenu();
            newMenu.MenuItems.Add(new MenuItem("Развернуть кратко", ToWindow));
            newMenu.MenuItems.Add(new MenuItem("Настройки", OptionsClick));
            newMenu.MenuItems.Add(new MenuItem("Выход", AppExit));

            iconPicture.ContextMenu = newMenu;

            iconPicture.MouseDoubleClick += trayClick;
            
            System.Windows.Application.Current.Exit += Current_Exit;
        }

       
        //скрыть значки (Если Exception треи могут не скрыться автоматом)
        public static void TrayHide()
        {
            iconPicture.Visible = false;
        }

        //обновить трей
        public static void Update(ForecastHour newFore)
        {
            Icon forPic = Icon.FromHandle(getPicture(newFore).GetHicon());
            iconPicture.Text = newFore.temp + "°С";
            makeTray(forPic);
        }

        //-------------------------------------------------------------------------------


        private static void expand(object sender, EventArgs e)
        {
            ToWindow(sender, e);
        }


        //задает иконки скрывает окно
        private static void makeTray(Icon forPic) //, Icon forDeg
        {
            windowMain.WindowState = System.Windows.WindowState.Normal;
            windowMain.Hide();

            iconPicture.Icon = forPic;

            iconPicture.Visible = true;
        }

        //при нажатии на выход
        private static void AppExit(object sender, EventArgs e)
        {
            TrayHide();
            windowMain.Close();
        }

        //при двойном клике открываем окно
        static void trayClick(object sender, EventArgs e)
        {
            ToWindow(sender, e);
        }
      
        //получает битмап из картинки
        private static Bitmap getPicture(ForecastHour fore)
        {
            Bitmap res = WeatherInfo.Classes.OpenWeatherAPI.GetImageById(fore.icon);
            return res;
        }

        //откроет окно
        private static void ToWindow(object sender, EventArgs e)
        {
            TrayHide();
            windowMain.Show();
            windowMain.Focus();
            windowMain.Activate();
        }

        //вызовет событие OnOptionsClick
        private static void OptionsClick(object sender, EventArgs e)
        {
            if (OnOptionsClick != null)
            {
                SettingsWindow settingsWindow = new SettingsWindow(windowMain);
                settingsWindow.Show();
                settingsWindow.Focus();
                settingsWindow.Activate();
                //ToWindow(sender, e);
                //OnOptionsClick();
            }
            else { throw new Exception("Задайте событие на нажатие опций в трее - OnOptionsClick"); }
        }

        //событие на закрытие всего приложения
        private static void Current_Exit(object sender, ExitEventArgs e)
        {
            TrayHide();
        }
    }
}
