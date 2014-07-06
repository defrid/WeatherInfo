using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using WeatherInfo.Classes;

namespace WeatherInfo
{
    public partial class WindowGraphics : Window
    {
        public WindowGraphics()
        {
            InitializeComponent();
        }

       
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// Строит график температуры в течении дня (Внутри дня записи часовых показаний должны быть в хронолгически верном порядке)
        /// </summary>
        /// <param name="Day">День</param>
        public void makeGraphic(ForecastDay Day)
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
            NewChart.Title = "Температура";
            
            grafic.Series.Add(NewChart);

            var a1 = from p1 in Day.hours select p1.temp;
            int max = a1.Max();
            int min = a1.Min();

            LinearAxis yA = new LinearAxis();
            yA.Orientation = AxisOrientation.Y;
            yA.Maximum = max+1;
            yA.Minimum = min-1;
            yA.ShowGridLines = true;
            yA.Title = "Температура";
            grafic.Axes.Add(yA);

            CategoryAxis xA = new CategoryAxis();
            xA.Orientation = AxisOrientation.X;
            xA.Title="Время суток";
            xA.ShowGridLines = true;
            grafic.Axes.Add(xA);

            grafic.Title = "Изменение температуры за день";
        }

        /// <summary>
        /// Строит график для выборки дней (Внимание! Необходимо чтобы приходил список отсортированный по дате!)
        /// </summary>
        /// <param name="dayList">Выборка дней</param>
        public void makeGraphic(List<ForecastDay> dayList)
        {
            ObservableCollection<dataCharHour> coll = new ObservableCollection<dataCharHour>();

            foreach (var a in dayList)
            {
                var a1 = from p1 in a.hours select p1.temp;
                int av = (int)a1.Average();

                coll.Add(new dataCharHour { temp = av, id = a.date });
            }

            LineSeries NewChart = new LineSeries();
            NewChart.ItemsSource = coll;
            NewChart.DependentValuePath = "temp";
            NewChart.IndependentValuePath = "id";
            NewChart.Title = "Температура";

            grafic.Series.Add(NewChart);

            var a2 = from p1 in coll select p1.temp;
            int min = a2.Min();
            int max = a2.Max();

            LinearAxis yA = new LinearAxis();
            yA.Orientation = AxisOrientation.Y;
            yA.Maximum = max + 1;
            yA.Minimum = min - 1;
            yA.ShowGridLines = true;
            yA.Title = "Средняя температура";
            grafic.Axes.Add(yA);

            CategoryAxis xA = new CategoryAxis();
            xA.Orientation = AxisOrientation.X;
            xA.Title = "Дни";
            xA.ShowGridLines = true;
            grafic.Axes.Add(xA);

            grafic.Title = "Изменение температуры по дням";
        }

        /// <summary>
        /// Строит диаграмму облачности
        /// </summary>
        /// <param name="dayList">Список дней</param>
        public void makeDiagram(List<ForecastDay> dayList)
        {
            ObservableCollection<dataCharHour> coll = new ObservableCollection<dataCharHour>();
            Dictionary<string, int> oblaka = new Dictionary<string, int>();

            foreach (var a in dayList)
            {
                foreach(var b in a.hours)
                {
                    if (oblaka.ContainsKey(b.clouds)) oblaka[b.clouds]++;
                    else oblaka.Add(b.clouds, 1);
                }
            }

            foreach(var a in oblaka)
            {
                coll.Add(new dataCharHour { id = a.Key, temp = a.Value });
            }

            PieSeries NewChart = new PieSeries();
            NewChart.ItemsSource = coll;
            NewChart.DependentValuePath = "temp";
            NewChart.IndependentValuePath = "id";
            grafic.Title = "Диаграмма облачности";
            grafic.Series.Add(NewChart);

        }

        //Служебный класс
        class dataCharHour
        {
            public int temp { get; set; }
            public string id { get; set; }
        }


        /*
           
         *  Пример тестового заполнения ---------------------------------------------------
         
           Random rand = new Random();

            List<ForecastDay> days = new List<ForecastDay>();
            for(int i=0; i<17; i++)
            {
                ForecastHour fh1 = new ForecastHour(rand.Next(-30, 30), "облачно", "утро", "");
                ForecastHour fh2 = new ForecastHour(rand.Next(-30, 30), "дождь", "день", "");
                ForecastHour fh3 = new ForecastHour(rand.Next(-30, 30), "солнечно", "вечер", "");
                ForecastHour fh4 = new ForecastHour(rand.Next(-30, 30), "облачно", "ночь", "");

                List<ForecastHour> fhL = new List<ForecastHour>();
                fhL.Add(fh1);
                fhL.Add(fh2);
                fhL.Add(fh3);
                fhL.Add(fh4);

                days.Add(new ForecastDay(-100, 100, fhL, i.ToString()+"-02-14", ""));
            }

            //makeGraphic(days[0]);
            //makeGraphic(days);
            //makeDiagram(days);
         */
    }
}
