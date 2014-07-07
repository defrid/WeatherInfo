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
using System.Windows.Shapes;
using WeatherInfo.Classes;

namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for Two_Windows.xaml
    /// </summary>
    public partial class Two_Windows : Window
    {

        MainWindow forSet;

        public Two_Windows(List<MainWindow> mv)
        {
            InitializeComponent();

            forSet = mv[0];

            List<ContentControl> cll = new List<ContentControl>();
            foreach (var a in mv)
            {
                ContentControl t = new ContentControl();
                t.Content = a.Content;
                cll.Add(t);
            }

            foreach(var a in mv)
            {
              //  a.Close();
            }

            int h = (int)this.Height/cll.Count;


            foreach(var a in cll)
            {
                this.stackP.Children.Add(a);
            }

            Tray.SetupTray(this, options, expandShort);
            //Tray.windowMain = this;

            this.Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.F1:

                    //MainWindow main = new MainWindow();
                    //main.Show();
                    //this.Close();

                    break;
            }
        }


        void expandShort()
        {
            this.WindowState = System.Windows.WindowState.Normal;
            this.Show();
        }



        void options()
        {
            try
            {
                MainWindow.wasLoaded = false;
                if (MainWindow.SettingWindow != null) MainWindow.SettingWindow.Close();
                MainWindow.SettingWindow = new SettingsWindow(forSet);
                MainWindow.SettingWindow.Show();
                
            }
            catch { }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized) this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
         
            //this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (MainWindow.SettingWindow != null) MainWindow.SettingWindow.Close();
            if (MainWindow.GraphicsWindow != null) MainWindow.GraphicsWindow.Close();
        }
    }
}
