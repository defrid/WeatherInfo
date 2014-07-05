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
        public CitySettings(string _countryId, string _countryName, int _regionId, string _regionName, int _cityId, string _cityName)
        {
            country = new Country(_countryId, _countryName);
            region = new RegionOfCity(_regionId, _regionName);
            city = new City(_cityId, _cityName);
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
            string _countryName = "Россия";
            int _regionId = 73;
            string _regionName = "Ульяновск";
            int _cityId = 27786;
            string _cityName = "Ульяновск";

            var citySettings = new CitySettings(_countryId, _countryName, _regionId, _regionName, _cityId, _cityName);

            return citySettings;
        }
    }
}
