using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SettingsHandlerInterface.Classes
{    
    /// <summary>
    /// Класс настроек приложения
    /// </summary>
    public class Settings
    {
        public Settings() { }

        public Settings(List<CitySettings> _cities, string _format, int _updatePeriod, bool _autostart, string _temperatureUnits, string _language)
        {
            cities = new List<CitySettings>(_cities);
            format = _format;
            updatePeriod = _updatePeriod;
            autostart = _autostart;
        }

        public Settings(string _countryId, string _countryRusName, string _countryEngName, int _regionId, string _regionName, int _cityYaId, int _cityOWId, string _cityRusName, string _cityEngName, string _format, int _updatePeriod, bool _autostart, string _temperatureUnits, string _language)
        {
            cities = new List<CitySettings>();
            cities.Add(new CitySettings(_countryId, _countryRusName, _countryEngName, _regionId, _regionName, _cityYaId, _cityOWId, _cityRusName, _cityEngName));
            format = _format;
            updatePeriod = _updatePeriod;
            autostart = _autostart;
        }

        public Settings(List<Country> _countries, List<RegionOfCity> _regions, List<City> _cities, string _format, int _updatePeriod, bool _autostart, string _temperatureUnits, string _language)
        {
            cities = new List<CitySettings>();
            format = _format;
            updatePeriod = _updatePeriod;
            autostart = _autostart;
        }

        public List<CitySettings> cities { get; set; }
        public string format { get; set; }
        public int updatePeriod { get; set; }
        public bool autostart { get; set; }
        public string temperatureUnits { get; set; }
        public string language { get; set; }

        public CitySettings GetFirstCity()
        {
            return cities.ElementAt(0);
        }

        /// <summary>
        /// Настройки по-умолчанию
        /// </summary>
        /// <returns></returns>
        public static Settings GetDefaultSettings()
        {
            string _countryId = "RU";
            string _countryRusName = "Россия";
            string _countryEngName = "Russia";

            int _regionId = 73;
            string _regionName = "Ульяновск";

            int _cityYaId = 27786;
            int _cityOWId = 479123;
            string _cityRusName = "Ульяновск";
            string _cityEngName = "Ulyanovsk";

            string _format = Enum.GetName(typeof(Options.FormatForecast), Options.FormatForecast.Days);
            int _updatePeriod = 60;
            bool _autostart = true;
            string _temperatureUnits = "Celsius";
            string _language = "Russian";

            var settings = new Settings(_countryId, _countryRusName, _countryEngName, _regionId, _regionName, _cityYaId, _cityOWId, _cityRusName, _cityEngName, _format, _updatePeriod, _autostart, _temperatureUnits, _language);

            return settings;
        }
    }    
}
