using System.Collections;

namespace WeatherInfo.Classes
{
    public class ChoosenCityForForecast
    {
        public string Country { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }

        public override string ToString()
        {
            return string.Format("{1}, {0}", Country, CityName);
        }
    }
}
