using System.Collections.Generic;

namespace DataHandlerInterface.Classes
{
    /// <summary>
    /// Класс прогноза дня, содержит температуру за четыре времени суток
    /// </summary>
    public class ForecastDay
    {
        public ForecastDay() { }
        public ForecastDay(int _min, int _max, List<ForecastHour> _hours, string _date, string _icon)
        {
            min = _min;
            max = _max;
            hours = _hours;
            date = _date;
            icon = _icon;
        }

        public ForecastDay(ForecastDay forecastDay)
        {
            min = forecastDay.min;
            max = forecastDay.max;
            hours = forecastDay.hours;
            date = forecastDay.date;
            icon = forecastDay.icon;
        }

        public int max { get; set; }

        public int min { get; set; }

        public List<ForecastHour> hours { get; set; }

        public string date { get; set; }//2014-07-01

        public string icon { get; set; }
    }
}
