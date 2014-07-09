using Microsoft.VisualStudio.TestTools.UnitTesting;
using SettingsHandlerInterface;
using SettingsHandlerInterface.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherInfo;

namespace WeatherInfoUnitTests
{
    [TestClass]
    public class UnitTestSettings
    {
        [TestMethod]
        public void TestGetFormatAttribute()
        {
            var formatWeeks = "Weeks";
            var formatDays = "Days";
            Assert.AreEqual("По неделям", Options.GetFormatAttribute(formatWeeks));
            Assert.AreNotEqual("По неделям", Options.GetFormatAttribute(formatDays));
        }

        [TestMethod]
        public void TestGetValueByAttribute()
        {
            var attrWeeks = "По неделям";
            var attrDays = "По дням";
            Assert.AreEqual("Weeks", Options.GetValueByAttribute(attrWeeks));
            Assert.AreNotEqual("Weeks", Options.GetFormatAttribute(attrDays));
        }

        [TestMethod]
        public void TestValidateSettings()
        {
            var validSettings = GetValidSettings();
            var invalidSettings = GetInvalidSettings();

            Assert.AreEqual(true, SettingsHandler.ValidateSettings(validSettings));
            Assert.AreNotEqual(true, SettingsHandler.ValidateSettings(invalidSettings));
        }

        [TestMethod]
        public void TestUpperCityName()
        {
            var cityName1 = "moscow";
            var cityName2 = "Ульяновск";            
            //Assert.AreEqual("Moscow", SettingsWindow.upperEngCityName(cityName1));
            //Assert.AreEqual("Ульяновск", SettingsWindow.upperEngCityName(cityName2));
        }

        private static Settings GetValidSettings()
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

        private static Settings GetInvalidSettings()
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

            string _format = Enum.GetName(typeof(Options.FormatForecast), Options.FormatForecast.Days);
            int _updatePeriod = 5;
            bool _autostart = false;
            TemperatureUnits _temperatureUnits = new TemperatureUnits("Цельсии", "Celsius");
            Language _language = new Language("Русский", "Russian");

            var settings = new Settings(_countryId, _countryRusName, _countryEngName, _regionId, _regionName, _cityYaId, _cityOWId, _cityRusName, _cityEngName, _format, _updatePeriod, _autostart, _temperatureUnits, _language);

            return settings;
        }
    }
}
