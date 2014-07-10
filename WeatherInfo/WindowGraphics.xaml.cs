using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Collections.ObjectModel;
using WeatherInfo.Classes;

namespace WeatherInfo
{
    public enum lang { rus, eng };

    public partial class WindowGraphics : Window
    {
        public WindowGraphics(lang language)
        {
            InitializeComponent();
            this.Lang = language;
        }

        public lang Lang;
       
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (Lang != lang.rus) this.Title = "Statistic";
        }

        /// <summary>
        /// Строит график температуры в течении дня (Внимание! Необходимо чтобы приходил список отсортированный по дате!)
        /// </summary>
        /// <param name="Day">День</param>
        public void makeGraphic(ForecastDay Day, string cityName)
        {
            ObservableCollection<dataCharHour> coll = new ObservableCollection<dataCharHour>();

            foreach(var a in Day.hours)
            {
                coll.Add(new dataCharHour { temp = a.temp, id = a.time});
            }

            LineSeries NewChart = new LineSeries();
            NewChart.ItemsSource = coll;
            NewChart.DependentValuePath = "temp";
            NewChart.IndependentValuePath = "id";

            if (Lang == lang.rus)
                NewChart.Title = "Температура";
            else
                NewChart.Title = "Temperature";
            
            grafic.Series.Add(NewChart);

            var a1 = from p1 in Day.hours select p1.temp;
            int max = a1.Max();
            int min = a1.Min();

            LinearAxis yA = new LinearAxis();
            yA.Orientation = AxisOrientation.Y;
            yA.Maximum = max+1;
            yA.Minimum = min-1;
            yA.ShowGridLines = true;
            if (Lang == lang.rus)
                yA.Title = "Температура";
            else
                yA.Title = "Temperature";
            grafic.Axes.Add(yA);

            CategoryAxis xA = new CategoryAxis();
            xA.Orientation = AxisOrientation.X;
            if (Lang == lang.rus)
                xA.Title = "Время суток";
            else
                xA.Title = "Day time";
            xA.ShowGridLines = true;
            grafic.Axes.Add(xA);

            if (Lang == lang.rus)
                grafic.Title = cityName + " - Изменение температуры за день " + Day.date;
            else
                grafic.Title = cityName+" - Temperature changing in the day " + Day.date;
        }

        /// <summary>
        /// Строит график для выборки дней (Внимание! Необходимо чтобы приходил список отсортированный по дате!)
        /// </summary>
        /// <param name="dayList">Выборка дней</param>
        public void makeGraphic(List<ForecastDay> dayList, string cityName)
        {
            ObservableCollection<dataCharHour> coll = new ObservableCollection<dataCharHour>();

            foreach (var a in dayList)
            {
                var a1 = from p1 in a.hours select p1.temp;
                int av = (int)a1.Average();

                string data = a.date;
                if (data.Length > 2) data = data.Substring(data.Length - 2, 2);

                coll.Add(new dataCharHour { temp = av, id = data });
            }

            LineSeries NewChart = new LineSeries();
            NewChart.ItemsSource = coll;
            NewChart.DependentValuePath = "temp";
            NewChart.IndependentValuePath = "id";
            if (Lang == lang.rus)
                NewChart.Title = "Температура";
            else
                NewChart.Title = "Temperature";

            grafic.Series.Add(NewChart);

            var a2 = from p1 in coll select p1.temp;
            int min = a2.Min();
            int max = a2.Max();

            LinearAxis yA = new LinearAxis();
            yA.Orientation = AxisOrientation.Y;
            yA.Maximum = max + 1;
            yA.Minimum = min - 1;
            yA.ShowGridLines = true;
            if (Lang == lang.rus)
                yA.Title = "Средняя температура";
            else
                yA.Title = "Average temperature";
            grafic.Axes.Add(yA);

            CategoryAxis xA = new CategoryAxis();
            xA.Orientation = AxisOrientation.X;
            if (Lang == lang.rus)
                xA.Title = "Дни";
            else
                xA.Title = "Days";
            xA.ShowGridLines = true;
            grafic.Axes.Add(xA);

            if (Lang == lang.rus)
                grafic.Title = cityName+" - Изменение температуры за несколько дней";
            else
                grafic.Title = cityName+" - Temperature change for some days";
        }

        //Служебный класс
        class dataCharHour
        {
            public int temp { get; set; }
            public string id { get; set; }
        }
    }
}
