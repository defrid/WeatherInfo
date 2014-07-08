using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherInfo;
using WeatherInfo.Classes;
using System.Collections.Generic;

namespace WeatherInfoUnitTests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {

        }

        /// <summary>
        /// Тест получения городов в стрене и их перевод
        /// </summary>
        [TestMethod]
        public void TestGetCityAndTranslate()
        {
            List<string> t = getCity.getCities("Россия", true);
            Assert.AreEqual(true, t.Contains("Ульяновск"));
            Assert.AreEqual(true, t.Contains("Москва"));
            Assert.AreEqual(true, t.Contains("Орел"));

            string ul = getCity.cityTranslate("Ульяновск", true);
            Assert.AreEqual("ulyanovsk", ul);
            ul = getCity.cityTranslate("Москва", true);
            Assert.AreEqual("moscow", ul);
            ul = getCity.cityTranslate("Нью-Йорк, шт. Нью-Йорк", true);
            Assert.AreEqual("new york", ul);
        }

        /// <summary>
        /// Тест получения id городов
        /// </summary>
        [TestMethod]
        public void getCitiesIDs()
        {
            int ya = getCity.getCityId("Нью-Йорк, шт. Нью-Йорк", true, true);
            int opW = getCity.getCityId("Нью-Йорк, шт. Нью-Йорк", true, false);
            Assert.AreEqual(72503, ya);
            Assert.AreEqual(5106292, opW);

            ya = getCity.getCityId("moscow", false, true);
            opW = getCity.getCityId("moscow", false, false);

            Assert.AreEqual(27612, ya);
            Assert.AreEqual(524901, opW);
        }

        /// <summary>
        /// Тест перевода страны 
        /// </summary>
        [TestMethod]
        public void translateCountry()
        {
            string eng = getCity.countryTranslate("Бразилия", true);
            string rus = getCity.countryTranslate("Hungary", false);

            Assert.AreEqual(eng, "Brazil");
            Assert.AreEqual(rus, "Венгрия");
        }
    }
}
