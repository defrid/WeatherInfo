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

        public Settings(List<CitySettings> _cities, string _format, int _updatePeriod, bool _autostart, TemperatureUnits _temperatureUnits, Language _language)
        {
            cities = new List<CitySettings>(_cities);
            format = _format;
            updatePeriod = _updatePeriod;
            autostart = _autostart;
            temperatureUnits = _temperatureUnits;
            language = _language;
        }

        public Settings(string _countryId, string _countryRusName, string _countryEngName, int _regionId, string _regionName, int _cityYaId, int _cityOWId, string _cityRusName, string _cityEngName, string _format, int _updatePeriod, bool _autostart, TemperatureUnits _temperatureUnits, Language _language)
        {
            cities = new List<CitySettings>();
            cities.Add(new CitySettings(_countryId, _countryRusName, _countryEngName, _regionId, _regionName, _cityYaId, _cityOWId, _cityRusName, _cityEngName));
            format = _format;
            updatePeriod = _updatePeriod;
            autostart = _autostart;
            temperatureUnits = _temperatureUnits;
            language = _language;
        }

        public Settings(List<Country> _countries, List<RegionOfCity> _regions, List<City> _cities, string _format, int _updatePeriod, bool _autostart, TemperatureUnits _temperatureUnits, Language _language)
        {
            cities = new List<CitySettings>();
            format = _format;
            updatePeriod = _updatePeriod;
            autostart = _autostart;
            temperatureUnits = _temperatureUnits;
            language = _language;
        }

        public List<CitySettings> cities { get; set; }
        public string format { get; set; }
        public int updatePeriod { get; set; }
        public bool autostart { get; set; }
        public TemperatureUnits temperatureUnits { get; set; }
        public Language language { get; set; }

        /// <summary>
        /// Настройки по-умолчанию
        /// </summary>
        /// <returns>Объект, хранящий настройки программы.</returns>
        public static Settings GetDefaultSettings()
        {
            string _countryId = "RU";
            string _countryRusName = "Россия";
            string _countryEngName = "Russia";

            int _regionId = 0;
            string _regionName = "Region";

            int _cityYaId = 27786;
            int _cityOWId = 479123;
            string _cityRusName = "Ульяновск";
            string _cityEngName = "Ulyanovsk";

            string _format = Enum.GetName(typeof(Options.FormatForecast), Options.FormatForecast.Days);
            int _updatePeriod = 60;
            bool _autostart = false;
            TemperatureUnits _temperatureUnits = new TemperatureUnits("Цельсии", "Celsius");
            Language _language = new Language("Русский", "Russian");

            var settings = new Settings(_countryId, _countryRusName, _countryEngName, _regionId, _regionName, _cityYaId, _cityOWId, _cityRusName, _cityEngName, _format, _updatePeriod, _autostart, _temperatureUnits, _language);

            return settings;
        }
    }    
}
