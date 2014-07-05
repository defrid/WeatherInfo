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

        public Settings(string _countryId, string _countryName, int _regionId, string _regionName, int _cityId, string _cityName, string _format, int _updatePeriod, bool _autostart, string _temperatureUnits, string _language)
        {
            cities = new List<CitySettings>();
            cities.Add(new CitySettings(_countryId, _countryName, _regionId, _regionName, _cityId, _cityName));
            format = _format;
            updatePeriod = _updatePeriod;
            autostart = _autostart;
        }

        public Settings(List<KeyValuePair<string, string>> _countries, List<KeyValuePair<int, string>> _regions, List<KeyValuePair<int, string>> _cities, string _format, int _updatePeriod, bool _autostart, string _temperatureUnits, string _language)
        {
            if (_countries.Count != _cities.Count || _countries.Count != _regions.Count)
            {
                throw new Exception("Список стран не соответствует списку городов. Убедитесь, что их количество одинаково.");
            }

            var tmpCountries = new List<Country>();
            foreach (var country in _countries)
            {
                tmpCountries.Add(new Country(country.Key, country.Value));
            }

            var tmpRegions = new List<RegionOfCity>();
            foreach (var region in _regions)
            {
                tmpRegions.Add(new RegionOfCity(region.Key, region.Value));
            }

            var tmpCities = new List<City>();
            foreach (var city in _cities)
            {
                tmpCities.Add(new City(city.Key, city.Value));
            }

            cities = new List<CitySettings>();
            for (var i = 0; i < _countries.Count; i++)
            {
                var curCitySettings = new CitySettings(tmpCountries[i], tmpRegions[i], tmpCities[i]);
                cities.Add(curCitySettings);
            }

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
            string _countryName = "Россия";
            int _regionId = 73;
            string _regionName = "Ульяновск";
            int _cityId = 27786;
            string _cityName = "Ульяновск";
            string _format = Enum.GetName(typeof(Options.FormatForecast), Options.FormatForecast.Days);
            int _updatePeriod = 60;
            bool _autostart = true;
            string _temperatureUnits = "Celsius";
            string _language = "Russian";

            var settings = new Settings(_countryId, _countryName, _regionId, _regionName, _cityId, _cityName, _format, _updatePeriod, _autostart, _temperatureUnits, _language);

            return settings;
        }
    }    
}
