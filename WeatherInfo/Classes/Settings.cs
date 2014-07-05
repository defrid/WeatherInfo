using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WeatherInfo.Classes
{
    /// <summary>
    /// Перечисление форматов прогноза погоды (по неделям, по дням, по часам)
    /// </summary>
    public enum FormatForecast
    {
        [FormatAttribute("По неделям")]
        Weeks,
        [FormatAttribute("По дням")]
        Days
    }

    [AttributeUsage(AttributeTargets.All)]
    internal class FormatAttribute : Attribute
    {
        public string name = string.Empty;
        public FormatAttribute(string _name)
        {
            name = _name;
        }
    }

    /// <summary>
    /// Класс настроек приложения
    /// </summary>
    public class Settings
    {
        public Settings() { }

        public Settings(string _country, int cityId, string _cityName, string _format, int _updatePeriod, bool _autostart)
        {
            country = _country;
            city = new City(cityId, _cityName);
            format = _format;
            updatePeriod = _updatePeriod;
            autostart = _autostart;
        }

        /// <summary>
        /// Класс города
        /// </summary>
        public class City
        {
            public int cityId { get; set; }
            public string cityName { get; set; }

            public City() { }

            public City(int _cityId, string _cityName)
            {
                cityId = _cityId;
                cityName = _cityName;
            }
        }

        public string country { get; set; }
        public City city { get; set; }
        public string format { get; set; }
        public int updatePeriod { get; set; }
        public bool autostart { get; set; }
    }
}