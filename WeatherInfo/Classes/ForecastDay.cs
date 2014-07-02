using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherInfo.Classes
{
    /// <summary>
    /// Класс прогноза, в него будем складывать информацию за отрезок времени
    /// </summary>
    public class ForecastDay
    {
        public ForecastDay(int _mor, int _day, int _evn, int _ngt, string _clouds, string _date, string _icon)
        {
            mor = _mor;
            day = _day;
            evn = _evn;
            ngt = _ngt;
            clouds = _clouds;
            date = _date;
            icon = _icon;
        }


        public int mor { get; protected set; }

        public int day { get; protected set; }

        public int evn { get; protected set; }

        public int ngt { get; protected set; }

        public string clouds { get; protected set; }

        public string date { get; protected set; }//2014-07-01T12:00:00, для неподробного 2014-07-01

        public string icon { get; protected set; }
    }
}
