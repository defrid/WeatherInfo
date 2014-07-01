using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherInfo.Classes
{
    /// <summary>
    /// Класс прогноза, в него будем складывать информацию за отрезок времени
    /// </summary>
    public class Forecast
    {
        public Forecast(int _min, int _max, string _clouds, string _date, string _icon)
        {
            min = _min;
            max = _max;
            clouds = _clouds;
            date = _date;
            icon = _icon;
        }


        public int min { get; protected set; }

        public int max { get; protected set; }

        public string clouds { get; protected set; }

        public string date { get; protected set; }//2014-07-01T12:00:00, для неподробного 2014-07-01

        public string icon { get; protected set; }
    }
}
