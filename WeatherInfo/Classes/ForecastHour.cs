using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherInfo.Classes
{
    public class ForecastHour
    {
        public ForecastHour(int _temp, string _clouds, string _time, string _icon)
        {
            temp = _temp;
            clouds = _clouds;
            time = _time;
            icon = _icon;
        }

        public int temp { get; set; }

        public string clouds { get; protected set; }

        public string time { get; set; }

        public string icon { get; protected set; }
    }
}
