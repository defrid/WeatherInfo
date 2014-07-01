using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace WeatherInfo
{
    public class Tray
    {
        public delegate void TrayVoid();
        public static event TrayVoid OnOptionsClick;

        public enum weatherState
        {
            cloud, sun, rain, snow
        }
        static string wayCloud = @"Icons/cloud.png";
        static string waySun = @"Icons/sun.png";
        static string wayRain = @"Icons/rain.png";
        static string waySnow = @"Icons/snow.png";
        static string iDdescr = "Градуcов Цельсия. Синий < 0, Красный > 0.";
        static string iPdescr = "Двойной клик - развернуть программу, ПКМ - открыть меню";
        static Window windowMain;
        static NotifyIcon iconPicture = new NotifyIcon();
        static NotifyIcon iconDigit = new NotifyIcon();
        
        //свернуть окно в трей
        public static void ToTray(Window main, weatherState weather, int temperature)
        {
            windowMain = main;
            main.Hide();

            try
            {
                Icon forPic = Icon.FromHandle(getPicture(weather).GetHicon());
                iconPicture.Icon = forPic;
                Icon forDeg = Icon.FromHandle(getPicture(temperature).GetHicon());
                iconDigit.Icon = forDeg;
            }
            catch { }

            iconPicture.Text = iPdescr;
            iconDigit.Text = iDdescr;

            iconPicture.ContextMenu = new ContextMenu();
            iconPicture.ContextMenu.MenuItems.Add(new MenuItem("Подробный режим", ToWindow));
            iconPicture.ContextMenu.MenuItems.Add(new MenuItem("Настройки", OptionsClick));
            iconPicture.ContextMenu.MenuItems.Add(new MenuItem("Выход", AppExit));

            iconPicture.MouseDoubleClick += trayClick;
            iconDigit.MouseDoubleClick += trayClick;
            

            System.Windows.Application.Current.Exit += Current_Exit;
            iconDigit.Visible = true;
            iconPicture.Visible = true;
        }

        

        //скрыть значки (Если Exception они могут не скрыться автоматом)
        public static void TrayHide()
        {
            iconPicture.Visible = false;
            iconDigit.Visible = false;
        }

        //обновить трей
        public static void Update(weatherState weather, int temperature)
        {
            try
            {
                Icon forPic = Icon.FromHandle(getPicture(weather).GetHicon());
                iconPicture.Icon = forPic;
                Icon forDeg = Icon.FromHandle(getPicture(temperature).GetHicon());
                iconDigit.Icon = forDeg;
            }
            catch { }
        }

        //-------------------------------------------------------------------------------

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
        private static Bitmap getPicture(weatherState ws)
        {
            string way = "";
            switch (ws)
            {
                case weatherState.cloud:
                    way = wayCloud;
                    break;

                case weatherState.rain:
                    way = wayRain;
                    break;

                case weatherState.sun:
                    way = waySun;
                    break;

                case weatherState.snow:
                    way = waySnow;
                    break;
            }
            return new Bitmap(way);
        }

        //получает битмап с цветной цифрой
        private static Bitmap getPicture(int degree)
        {
            Bitmap bitmap = new Bitmap(128, 128);
            Graphics gr = Graphics.FromImage(bitmap);
            Font fontM = new Font(System.Drawing.FontFamily.GenericSansSerif, 80);
            SolidBrush brush;

            if (degree > 0)
                brush = new SolidBrush(Color.Red);
            else
                if (degree < 0)
                {
                    brush = new SolidBrush(Color.Blue);
                    degree = -degree;
                }
                else brush = new SolidBrush(Color.Black);



            gr.DrawString(degree.ToString(), fontM, brush, new PointF(0, 0));

            return bitmap;
        }

        //откроет окно
        private static void ToWindow(object sender, EventArgs e)
        {
            TrayHide();
            windowMain.Show();
        }

        //вызовет событие OnOptionsClick
        private static void OptionsClick(object sender, EventArgs e)
        {
            if (OnOptionsClick != null)
            {
                windowMain.Show();
                TrayHide();
                OnOptionsClick();
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
