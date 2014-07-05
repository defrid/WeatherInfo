using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowTray
    {
        public delegate void myD();
        public event myD onLeave;

        public WindowTray(myD leaveEvent, List<TrayCityData> listData)
        {
            InitializeComponent();
            onLeave = leaveEvent;
            foreach(var a in listData)
            {
                AddItem(a);
            }
        }


        public void AddItem(TrayCityData item)
        {
            DockPanel dp = new DockPanel();
            dp.Height = 50;

            //dp.Name = item.name.Content.ToString();
            //-------------------------
            item.name.FontSize = 15;
            item.name.VerticalAlignment = VerticalAlignment.Center;
            item.name.HorizontalAlignment = HorizontalAlignment.Center;
            dp.Children.Add(item.name);
            //-------------------------
            System.Windows.Controls.Image im = new System.Windows.Controls.Image();
            string s = AppDomain.CurrentDomain.BaseDirectory.ToString();

            //Создал System.Drawing.Image
            System.Drawing.Image i = item.icon;
            //Создал поток в памяти и записал туда иконку
            System.IO.MemoryStream stre = new System.IO.MemoryStream();
            i.Save(stre, System.Drawing.Imaging.ImageFormat.Png);
            //Создал System.Windows.Media.ImageSource и передал в него поток
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = stre;
            bi.EndInit();
            //Присвоил элементу Image с именем myimg
            //В результате получаем преобразованнй System.Drawing.Image в System.Windows.Media.ImageSourc
            im.Source = bi;
            im.Width = 50;
            im.Height = 50;
            im.VerticalAlignment = VerticalAlignment.Center;
            DockPanel.SetDock(im, Dock.Right);
            dp.Children.Add(im);

            //-------------------------
            item.temp.FontSize = 15;
            DockPanel.SetDock(item.temp, Dock.Right);
            item.temp.HorizontalAlignment = HorizontalAlignment.Right;
            item.temp.VerticalAlignment = VerticalAlignment.Center;
            dp.Children.Add(item.temp);
            
            StackP.Children.Add(dp);
            this.Height = StackP.Children.Count * 50;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            onLeave();
        }
    }
}
