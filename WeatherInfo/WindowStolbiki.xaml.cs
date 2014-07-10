using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
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
    /// Interaction logic for WindowStolbiki.xaml
    /// </summary>
    public partial class WindowStolbiki : Window
    {
        public WindowStolbiki()
        {
            InitializeComponent();
        }

        public void makeDiagram(List<ForecastDay> dayList, string cityName)
        {
            ObservableCollection<LineDataPoint> coll = new ObservableCollection<LineDataPoint>();

            foreach (var a in dayList)
            {
                LineDataPoint sp = new LineDataPoint();
                sp.DependentValue = (int)(fromIconToCloud(a.icon));

                string data = a.date;
                if (data.Length > 2) data = data.Substring(data.Length - 2, 2);
                sp.IndependentValue = data;
                coll.Add(sp);
            }

            ColumnSeries NewChart = new ColumnSeries();

            NewChart.ItemsSource = coll;
            NewChart.DependentValuePath = "DependentValue";
            NewChart.IndependentValuePath = "IndependentValue";



            LinearAxis la = new LinearAxis();
            la.Maximum = 6;
            la.Orientation = AxisOrientation.Y;
            la.Minimum = 0;
            la.ShowGridLines = true;
            NewChart.DependentRangeAxis = la;

            CategoryAxis la2 = new CategoryAxis();
            la2.Orientation = AxisOrientation.X;
            la2.Title = "Дата";
            NewChart.IndependentAxis = la2;

           NewChart.Title = "Облачность";


           grafic.Title = cityName + " - Изменение облачности";

            grafic.Series.Add(NewChart);
        }

        clouds fromIconToCloud(string icon)
        {
            icon = icon.Substring(0, 2);
            switch (icon)
            {
                default: return clouds.clear_sky;
                case "02": return clouds.few_clouds;
                case "03": return clouds.scattered_clouds;
                case "04": return clouds.broken_clouds;
                case "09": return clouds.shower;
                case "10": return clouds.rain;
                case "11": return clouds.thunderstorm;
                case "13": return clouds.snow;
                case "50": return clouds.mist;
            }
        }

        enum clouds
        {
            clear_sky = 0,
            few_clouds = 1,
            scattered_clouds = 2,
            broken_clouds = 3,
            rain = 4,
            shower = 4,
            thunderstorm = 4,
            snow = 4,
            mist = 5,
        }
    
    }
}
