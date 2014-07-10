using DataHandlerInterface.Classes;
using DataHandlerInterface.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace WeatherInfoUnitTests
{
    [TestClass]
    public class UnitTestSettings
    {
        [TestMethod]
        public void TestGetFormatAttribute()
        {
            var formatShort = "Short";
            var formatDetailed = "Detailed";
            Assert.AreEqual("Short", Options.GetFormatAttribute(formatShort, "English"));
            Assert.AreEqual("Краткий", Options.GetFormatAttribute(formatShort, "Russian"));
            Assert.AreEqual("Detailed", Options.GetFormatAttribute(formatDetailed, "English"));
            Assert.AreEqual("Подробный", Options.GetFormatAttribute(formatDetailed, "Russian"));
        }

        [TestMethod]
        public void TestGetValueByAttribute()
        {
            Assert.AreEqual("Short", Options.GetValueByAttribute("Краткий", "Russian"));
            Assert.AreNotEqual("Short", Options.GetValueByAttribute("Подробный", "English"));
            Assert.AreEqual("Detailed", Options.GetValueByAttribute("Подробный", "Russian"));
            Assert.AreNotEqual("Detailed", Options.GetValueByAttribute("Краткий", "English"));
        }

        [TestMethod]
        public void TestValidateSettings()
        {
            var validSettings = GetValidSettings();
            var invalidSettings = GetInvalidSettings();

            Assert.AreEqual(true, DataHandler.ValidateSettings(validSettings));
            Assert.AreNotEqual(true, DataHandler.ValidateSettings(invalidSettings));
        }

        [TestMethod]
        public void TestUpperCityName()
        {
            var cityName1 = "moscow";
            var cityName2 = "Ульяновск";            
            //Assert.AreEqual("Moscow", SettingsWindow.upperEngCityName(cityName1));
            //Assert.AreEqual("Ульяновск", SettingsWindow.upperEngCityName(cityName2));
        }

        private static UserSettings GetValidSettings()
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

            string _format = Enum.GetName(typeof(Options.FormatForecast), Options.FormatForecast.Short);
            int _updatePeriod = 60;
            bool _autostart = false;
            TemperatureUnits _temperatureUnits = new TemperatureUnits("Цельсии", "Celsius");
            Language _language = new Language("Русский", "Russian");

            var settings = new UserSettings(_countryId, _countryRusName, _countryEngName, _regionId, _regionName, _cityYaId, _cityOWId, _cityRusName, _cityEngName, _format, _updatePeriod, _autostart, _temperatureUnits, _language);

            return settings;
        }

        private static UserSettings GetInvalidSettings()
        {
            string _countryId = "";
            string _countryRusName = "Россия";
            string _countryEngName = "Russia";

            int _regionId = -2;
            string _regionName = "  ";

            int _cityYaId = 27786;
            int _cityOWId = 479123;
            string _cityRusName = "Ульяновск";
            string _cityEngName = "Ulyanovsk";

            string _format = Enum.GetName(typeof(Options.FormatForecast), Options.FormatForecast.Short);
            int _updatePeriod = 5;
            bool _autostart = false;
            TemperatureUnits _temperatureUnits = new TemperatureUnits("Цельсии", "Celsius");
            Language _language = new Language("Русский", "Russian");

            var settings = new UserSettings(_countryId, _countryRusName, _countryEngName, _regionId, _regionName, _cityYaId, _cityOWId, _cityRusName, _cityEngName, _format, _updatePeriod, _autostart, _temperatureUnits, _language);

            return settings;
        }
    }
}
