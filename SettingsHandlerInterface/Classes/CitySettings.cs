using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SettingsHandlerInterface.Classes
{
    public class CitySettings
    {
        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public CitySettings() { }

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="_countryId">ИД страны (буквенный код)</param>
        /// <param name="_countryName">Наименование страны</param>
        /// <param name="_cityId">ИД города (числовой код)</param>
        /// <param name="_cityName">Наименование города</param>
        public CitySettings(string _countryId, string _countryRusName, string _countryEngName, int _regionId, string _regionName, int _cityYaId, int _cityOWId, string _cityRusName, string _cityEngName)
        {
            country = new Country(_countryId, _countryRusName, _countryEngName);
            region = new RegionOfCity(_regionId, _regionName);
            city = new City(_cityYaId, _cityOWId, _cityRusName, _cityEngName);
        }

        public CitySettings(Country _country, RegionOfCity _region, City _city)
        {
            country = _country;
            region = _region;
            city = _city;
        }

        public Country country { get; set; }
        public RegionOfCity region { get; set; }
        public City city { get; set; }

        /// <summary>
        /// Настройки по-умолчанию для города
        /// </summary>
        /// <returns></returns>
        public static CitySettings GetDefaultSettings()
        {
            string _countryId = "RU";
            string _countryRusName = "Россия";
            string _countryEngName = "Russia";
            int _regionId = 11153;
            string _regionName = "Ульяновская область";

            int _cityYaId = 27786;
            int _cityOWId = 479123;
            string _cityRusName = "Ульяновск";
            string _cityEngName = "Ulyanovsk";

            var citySettings = new CitySettings(_countryId, _countryRusName, _countryEngName, _regionId, _regionName, _cityYaId, _cityOWId, _cityRusName, _cityEngName);

            return citySettings;
        }
    }
}
