using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherInfo.Classes
{
    /// <summary>
    /// Класс прогноза дня, содержит температуру за четыре времени суток
    /// </summary>
    public class ForecastDay
    {
        public ForecastDay(int _min, int _max, List<ForecastHour> _hours, string _date, string _icon)
        {
            min = _min;
            max = _max;
            hours = _hours;
            date = _date;
            icon = _icon;
        }


        public int max { get; protected set; }

        public int min { get; protected set; }

        public List<ForecastHour> hours { get; set; }

        public string date { get; protected set; }//2014-07-01

        public string icon { get; protected set; }
    }
}
