using System;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherInfo.Classes;
using System.Linq;

namespace WeatherInfoUnitTests
{
    [TestClass]
    public class OpenApiUnitTest
    {
        readonly OpenWeatherAPI _testedApi = new OpenWeatherAPI("Moscow");

        [TestMethod]
        public void GettingCityTest()
        {
            Assert.AreEqual("Moscow",_testedApi.City);
        }

        [TestMethod]
        public void GetCurrentForecastTest()
        {
            var result = _testedApi.GetCurrentForecast();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result,typeof(XDocument));
            Assert.AreEqual("524901",result.Element("current").Element("city").Attribute("id").Value);
            Assert.AreEqual("Moscow", result.Element("current").Element("city").Attribute("name").Value);
            Assert.AreEqual("RU",result.Element("current").Element("city").Element("country").Value);
        }

        [TestMethod]
        public void GetDetailedWeekTest()
        {
            var result = _testedApi.GetDetailedWeek();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(XDocument));
            Assert.AreEqual("Moscow", result.Element("weatherdata").Element("location").Element("name").Value);
            Assert.AreEqual("RU", result.Element("weatherdata").Element("location").Element("country").Value);
            //Assert.AreEqual(41, result.Element("weatherdata").Element("forecast").Elements().Count());
        }

        [TestMethod]
        public void GetBigForecastTest()
        {
            var result = _testedApi.GetBigForecast();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(XDocument));
            Assert.AreEqual("Moscow", result.Element("weatherdata").Element("location").Element("name").Value);
            Assert.AreEqual("RU", result.Element("weatherdata").Element("location").Element("country").Value);
            Assert.AreEqual(14, result.Element("weatherdata").Element("forecast").Elements().Count());
        }

        [TestMethod]
        public void GetResponseStreamTest()
        {
            string url = @"http://api.openweathermap.org/data/2.5/weather?mode=xml&lang=ru&units=metric&q=Moscow";
            var method = typeof (OpenWeatherAPI).GetRuntimeMethods().Single(m=>m.Name=="GetResponseStream");
            var result = method.Invoke(null, new object[] {url});
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(XDocument));
        }

        [TestMethod]
        public void GetImageByIdTest()
        {
            var result = typeof (OpenWeatherAPI).GetMethod("GetImageById").Invoke(null, new object[] {"09n"});
            Assert.IsNotNull(result);
        }
    }
}
