using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherInfo.Classes;

namespace WeatherInfoUnitTests
{
    [TestClass]
    public class YandexApiUnitTest
    {
        private readonly YandexWeatherAPI _testedApi = new YandexWeatherAPI("27612");
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
