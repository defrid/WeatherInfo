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

                coll.Add(new dataCharHour { temp = av, id = a.date });
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

        /// <summary>
        /// Строит диаграмму облачности (Внимание! Необходимо чтобы приходил список отсортированный по дате!)
        /// </summary>
        /// <param name="dayList">Список дней</param>
        public void makeDiagram(List<ForecastDay> dayList, string cityName)
        {
            ObservableCollection<PieDataPoint> coll = new ObservableCollection<PieDataPoint>();
            Dictionary<string, int> oblaka = new Dictionary<string, int>();
            Dictionary<string, List<string>> oblakaDays = new Dictionary<string, List<string>>();

            foreach (var a in dayList)
            {
                foreach(var b in a.hours)
                {
                    string category = b.clouds;
                    if (Lang == lang.rus) category = getRusClouds(category);

                    if (oblaka.ContainsKey(category))
                    {
                        if (!oblakaDays[category].Contains(a.date))
                        {
                            oblaka[category]++;
                            oblakaDays[category].Add(a.date);
                        }
                    }

                    else
                    {
                        oblaka.Add(category, 1);
                        oblakaDays.Add(category, new List<string> { a.date });
                    }
                }
            }

            foreach(var a in oblaka)
            {
                PieDataPoint t = new PieDataPoint();
                t.IndependentValue = a.Key;
                t.DependentValue = a.Value;

                string res = "Числа с такой погодой";
                if (Lang != lang.rus) res = "Days with such weather";

                foreach(var b in oblakaDays[a.Key])
                {
                    res += "\n";
                    res += b;
                }
                t.DataContext = res;

                coll.Add(t);
            }

            PieSeries NewChart = new PieSeries();
            NewChart.ItemsSource = coll;
            NewChart.DependentValuePath = "DependentValue";
            NewChart.IndependentValuePath = "IndependentValue";



            if (Lang == lang.rus) grafic.Title = cityName+" - Диаграмма облачности";
            else grafic.Title = cityName+" - Overcast chart";
            grafic.Series.Add(NewChart);
           
        }

        //Служебный класс
        class dataCharHour
        {
            public int temp { get; set; }
            public string id { get; set; }
        }

        string getRusClouds(string engClouds)
        {
            engClouds = engClouds.Replace("overcast", "пасмурно");
            engClouds = engClouds.Replace("and", "и");
            engClouds = engClouds.Replace("light", "лёгкий");
            engClouds = engClouds.Replace("rain", "дождь");
            engClouds = engClouds.Replace("partly", "местами");
            engClouds = engClouds.Replace("cloudy", "облачно");
            engClouds = engClouds.Replace("clear", "ясно");
            engClouds = engClouds.Replace("mostly", "в большей части");
            engClouds = engClouds.Replace("thunderstorms", "гроза");
            engClouds = engClouds.Replace("possible", "возможно");
            engClouds = engClouds.Replace("with", "с");
            engClouds = engClouds.Replace("showers", "ливни");
            engClouds = engClouds.Replace("possibility", "возможно");
            engClouds = engClouds.Replace("of", "");
            engClouds = engClouds.Replace("snow", "снег");
            engClouds = engClouds.Replace("hail", "град");
            return engClouds;
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
