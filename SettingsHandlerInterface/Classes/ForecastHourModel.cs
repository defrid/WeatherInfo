namespace DataHandlerInterface.Classes
{
    public class ForecastHour
    {
        public ForecastHour() { }
        public ForecastHour(int _temp, string _clouds, string _time, string _icon)
        {
            temp = _temp;
            clouds = _clouds;
            time = _time;
            icon = _icon;
        }

        public ForecastHour(ForecastHour forecastHour)
        {
            temp = forecastHour.temp;
            clouds = forecastHour.clouds;
            time = forecastHour.time;
            icon = forecastHour.icon;
        }

        public int temp { get; set; }

        public string clouds { get; set; }

        public string time { get; set; }

        public string icon { get; set; }
    }
}
