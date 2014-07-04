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
        public static void Update(ForecastHour newFore, float scale=1.8f, bool WriteDigits=true)
        {
            Icon forPic = Icon.FromHandle(getPicture(newFore, scale, WriteDigits).GetHicon());
            iconPicture.Text = newFore.temp + "°С";
            makeTray(forPic);
        }

        //-------------------------------------------------------------------------------


        string[] cities = { "Забыли задать город!" };

        public void setCities(string city1, string city2 = "")
        {
            if (city2 == "")
            {
                cities = new string[1];
                cities[0] = city1;
            }
            else
            {
                cities = new string[2];
                cities[0] = city1;
                cities[1] = city2;
            }


            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add("Развернуть кратко");
            menu.MenuItems.Add("Развернуть подробно");
            menu.MenuItems.Add("Настройки", OptionsClick);
            menu.MenuItems.Add("Выход", AppExit);

            foreach (var a in cities)
            {
                menu.MenuItems[0].MenuItems.Add(a, ToShort);
                menu.MenuItems[1].MenuItems.Add(a, ToFull);
            }

            iconPicture.ContextMenu = menu;
        }



        private static void ToShort(object sender, EventArgs e)
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
      
        //получает битмап из картинки.
        private static Bitmap getPicture(ForecastHour fore, float scale, bool needDigits)
        {
            Bitmap res = WeatherInfo.Classes.OpenWeatherAPI.GetImageById(fore.icon);

            Bitmap bm = new Bitmap(100, 100);
            Graphics gr = Graphics.FromImage(bm);
            gr.TranslateTransform(bm.Width/2, bm.Height/2);
            gr.ScaleTransform(scale, scale);
            gr.TranslateTransform(-bm.Width/2, -bm.Height/2);
            gr.DrawImage(res, 0, 0, 100, 100);

            if (needDigits)
            {
                gr.ResetTransform();
                using (Font font1 = new Font("Lucida Console", 65, System.Drawing.FontStyle.Regular, GraphicsUnit.Point))
                {
                    int degree = fore.temp;
                    if (degree < 0) degree = -degree;
                    string text = degree.ToString();
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    Brush colorBr=Brushes.Black;
                    if (fore.temp > 0) colorBr = Brushes.Red;
                    if (fore.temp < 0) colorBr = Brushes.Blue;

                    gr.DrawString(text, font1, colorBr, new System.Drawing.Point(50, 50), stringFormat);

                }
            }

            return bm;
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
