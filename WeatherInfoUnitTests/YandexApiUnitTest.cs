using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherInfo.Classes;

namespace WeatherInfoUnitTests
{
    [TestClass]
    public class YandexApiUnitTest
    {
        private readonly YandexWeatherAPI _testedApi = new YandexWeatherAPI("27612");

        [TestMethod]
        public void GetIdTest()
        {
            Assert.AreEqual(_testedApi.CityId,"27612");
        }

        [TestMethod]
        public void GetCityesTest()
        {
            var result = YandexWeatherAPI.GetCities();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result,typeof(XDocument));
            var moscow = result.Root.Elements().Single(e => e.Attribute("name").Value == "Россия")
                               .Elements().Single(e => e.Value == "Москва");
            Assert.AreEqual(moscow.Attribute("id").Value,"27612");
            Assert.AreEqual(moscow.Attribute("part").Value, "Москва");
        }

        [TestMethod]
        public void GetForecast()
        {
            var result = _testedApi.GetForecast();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(XDocument));
            Assert.AreEqual(result.Root.Attribute("city").Value,"Москва");
            Assert.AreEqual(result.Root.Attribute("country").Value, "Россия");
            Console.WriteLine(result);
            XNamespace space = "http://weather.yandex.ru/forecast";
            Assert.IsNotNull(result.Root.Element(space+"fact"));
            Assert.IsNotNull(result.Root.Element(space + "yesterday"));
            Assert.AreEqual(result.Root.Elements(space+"day").Count(), 10);
        }
    }
}
