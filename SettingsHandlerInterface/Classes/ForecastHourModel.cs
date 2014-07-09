namespace DataHandlerInterface.Classes
{
    public class ForecastHourModel
    {
        public ForecastHourModel() { }
        public ForecastHourModel(int _temp, string _clouds, string _time, string _icon)
        {
            temp = _temp;
            clouds = _clouds;
            time = _time;
            icon = _icon;
        }

        public ForecastHourModel(ForecastHourModel forecastHour)
        {
            temp = forecastHour.temp;
            clouds = forecastHour.clouds;
            time = forecastHour.time;
            icon = forecastHour.icon;
        }

        public int temp { get; set; }

        public string clouds { get; protected set; }

        public string time { get; set; }

        public string icon { get; protected set; }
    }
}
