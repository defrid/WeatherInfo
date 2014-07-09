using System.Collections.Generic;

namespace DataHandlerInterface.Classes
{
    /// <summary>
    /// Класс прогноза дня, содержит температуру за четыре времени суток
    /// </summary>
    public class ForecastDayModel
    {
        public ForecastDayModel() { }
        public ForecastDayModel(int _min, int _max, List<ForecastHourModel> _hours, string _date, string _icon)
        {
            min = _min;
            max = _max;
            hours = _hours;
            date = _date;
            icon = _icon;
        }

        public ForecastDayModel(ForecastDayModel forecastDay)
        {
            min = forecastDay.min;
            max = forecastDay.max;
            hours = forecastDay.hours;
            date = forecastDay.date;
            icon = forecastDay.icon;
        }

        public int max { get; set; }

        public int min { get; set; }

        public List<ForecastHourModel> hours { get; set; }

        public string date { get; protected set; }//2014-07-01

        public string icon { get; protected set; }
    }
}
