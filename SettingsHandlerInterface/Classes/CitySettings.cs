namespace DataHandlerInterface.Classes
{
    public class CitySettings
    {
        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public CitySettings() { }

        /// <summary>
        /// Конструктор класса, создаёт экземпляр, хранящий информацию о параметрах города.
        /// </summary>
        /// <param name="_countryId">ИД страны (буквенный код)</param>
        /// <param name="_countryRusName">Наименование страны на русском</param>
        /// <param name="_countryEngName">Наименование страны на английском</param>
        /// <param name="_regionId">ИД региона</param>
        /// <param name="_regionName">Наименование региона</param>
        /// <param name="_cityYaId">ИД города из Яндекса</param>
        /// <param name="_cityOWId">ИД города из OpenWeather</param>
        /// <param name="_cityRusName">Наименование города на русском</param>
        /// <param name="_cityEngName">Наименование города на английском</param>
        public CitySettings(string _countryId, string _countryRusName, string _countryEngName, int _regionId, string _regionName, int _cityYaId, int _cityOWId, string _cityRusName, string _cityEngName)
        {
            country = new Country(_countryId, _countryRusName, _countryEngName);
            region = new RegionOfCity(_regionId, _regionName);
            city = new City(_cityYaId, _cityOWId, _cityRusName, _cityEngName);
        }

        /// <summary>
        /// Конструктор класса, создаёт экземпляр, хранящий информацию о параметрах города.
        /// </summary>
        /// <param name="_country">Объект, представляющий информацию о стране</param>
        /// <param name="_region">Объект, представляющий информацию о регионе</param>
        /// <param name="_city">Объект, представляющий информацию о городе</param>
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
        /// <returns>Объект, хранящий информацию о параметрах города.</returns>
        public static CitySettings GetDefaultSettings()
        {
            string _countryId = "RU";
            string _countryRusName = "Россия";
            string _countryEngName = "Russian Federation";
            int _regionId = 0;
            string _regionName = "Region";

            int _cityYaId = 27786;
            int _cityOWId = 479123;
            string _cityRusName = "Ульяновск";
            string _cityEngName = "Ulyanovsk";

            var citySettings = new CitySettings(_countryId, _countryRusName, _countryEngName, _regionId, _regionName, _cityYaId, _cityOWId, _cityRusName, _cityEngName);

            return citySettings;
        }

        public override string ToString()
        {
            return string.Format("{1}, {0}", country.countryRusName, city.cityRusName);
        }
    }
}
