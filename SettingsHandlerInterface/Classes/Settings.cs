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

        /// <summary>
        /// Конструктор класса, создаёт экземпляр, хранящий информацию о настройках программы
        /// </summary>
        /// <param name="_cities">Список городов (со всей хранимой информацией о городе)</param>
        /// <param name="_format">Формат представления прогноза погоды</param>
        /// <param name="_updatePeriod">Период обновления прогноза</param>
        /// <param name="_autostart">Флаг автозагрузки (true - разрешить, false - запретить)</param>
        /// <param name="_temperatureUnits">Единицы измерения температуры</param>
        /// <param name="_language">Язык локализации системы</param>
        public Settings(List<CitySettings> _cities, string _format, int _updatePeriod, bool _autostart, TemperatureUnits _temperatureUnits, Language _language)
        {
            cities = new List<CitySettings>(_cities);
            format = _format;
            updatePeriod = _updatePeriod;
            autostart = _autostart;
            temperatureUnits = _temperatureUnits;
            language = _language;
        }

        /// <summary>
        /// Конструктор класса, создаёт экземпляр, хранящий информацию о настройках программы
        /// </summary>
        /// <param name="_countryId">ИД страны</param>
        /// <param name="_countryRusName">Наименование страны на русском</param>
        /// <param name="_countryEngName">Наименование страны на английском</param>
        /// <param name="_regionId">ИД региона страны</param>
        /// <param name="_regionName">Наименование региона страны</param>
        /// <param name="_cityYaId">ИД города из Яндекса</param>
        /// <param name="_cityOWId">ИД города из OpenWeather</param>
        /// <param name="_cityRusName">Название города на русском</param>
        /// <param name="_cityEngName">Название города на английском</param>
        /// <param name="_format">Формат представления прогноза погоды</param>
        /// <param name="_updatePeriod">Период обновления прогноза</param>
        /// <param name="_autostart">Флаг автозагрузки (true - разрешить, false - запретить)</param>
        /// <param name="_temperatureUnits">Единицы измерения температуры</param>
        /// <param name="_language">Язык локализации системы</param>
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

        /// <summary>
        /// Конструктор класса, создаёт экземпляр, хранящий информацию о настройках программы
        /// </summary>
        /// <param name="_countries">Список стран (со всей хранимой информацией о стране)</param>
        /// <param name="_regions">Список регионов (со всей хранимой информацией о городе)</param>
        /// <param name="_cities">Список городов (со всей хранимой информацией о городе)</param>
        /// <param name="_format">Формат представления прогноза погоды</param>
        /// <param name="_updatePeriod">Период обновления прогноза</param>
        /// <param name="_autostart">Флаг автозагрузки (true - разрешить, false - запретить)</param>
        /// <param name="_temperatureUnits">Единицы измерения температуры</param>
        /// <param name="_language">Язык локализации системы</param>
        public Settings(List<Country> _countries, List<RegionOfCity> _regions, List<City> _cities, string _format, int _updatePeriod, bool _autostart, TemperatureUnits _temperatureUnits, Language _language)
        {
            cities = new List<CitySettings>();
            format = _format;
            updatePeriod = _updatePeriod;
            autostart = _autostart;
            temperatureUnits = _temperatureUnits;
            language = _language;
        }

        /// <summary>
        /// Список городов
        /// </summary>
        public List<CitySettings> cities { get; set; }

        /// <summary>
        /// Формат представления прогноза погоды
        /// </summary>
        public string format { get; set; }

        /// <summary>
        /// Период обновления прогноза
        /// </summary>
        public int updatePeriod { get; set; }

        /// <summary>
        /// Флаг автозагрузки (true - разрешить, false - запретить)
        /// </summary>
        public bool autostart { get; set; }

        /// <summary>
        /// Единицы измерения температуры
        /// </summary>
        public TemperatureUnits temperatureUnits { get; set; }

        /// <summary>
        /// Язык локализации системы
        /// </summary>
        public Language language { get; set; }

        /// <summary>
        /// Возвращает первый в списке город
        /// </summary>
        /// <returns></returns>
        public CitySettings GetFirstCity()
        {
            return cities.ElementAt(0);
        }

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
