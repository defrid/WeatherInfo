using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Net;
using WeatherInfo.Interfaces;

namespace WeatherInfo.Classes
{
    public class WeatherAPI:IWeatherAPI
    {

        public string City { get; protected set; }

        private const string CurentRequestString = @"http://api.openweathermap.org/data/2.5/weather?mode=xml&lang=ru&units=metric";
        private const string DetainedRequestString = @"http://api.openweathermap.org/data/2.5/forecast?&mode=xml&units=metric&lang=ru";
        private const string ShortRequestString = @"http://api.openweathermap.org/data/2.5/forecast//daily?&mode=xml&units=metric&lang=ru";
        private const string ImageRequestString = @"http://openweathermap.org/img/w/";
        public WeatherAPI(string city)
        {
            City = city;
        }

        /// <summary>
        /// Получение ответа от сервера по URI
        /// </summary>
        /// <param name="uriString">Строка uri запроса</param>
        /// <returns></returns>
        private static XDocument GetResponseStream(string uriString)
        {
            var request = WebRequest.Create(uriString);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
                return stream!=null?XDocument.Load(stream):null;
        }
        /// <summary>
        /// Метод для получения рисунка погоды по имени
        /// </summary>
        /// <param name="id">значение облачности</param>
        /// <returns></returns>
        public static Bitmap GetImageById(string id)
        {
            var uriString = ImageRequestString + String.Format("{0}.png", id);
            var request = WebRequest.Create(uriString);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
                return stream!=null?new Bitmap(stream): null;
        }


        /// <summary>
        /// Получение текущего прогноза
        /// </summary>
        /// <returns></returns>
        public XDocument GetCurrentForecast()
        {
            var uriString = CurentRequestString + String.Format("&q={0}", City);
            return GetResponseStream(uriString);
        }

        /// <summary>
        /// Подробный прогноз на 5 дней с интервалом в 3 часа
        /// </summary>
        /// <returns></returns>
        public XDocument GetDetailedWeek()
        {
            var uriString = DetainedRequestString + String.Format("&q={0}", City);
            return GetResponseStream(uriString);
        }

        /// <summary>
        /// Полный прогноз на 14 дней
        /// </summary>
        /// <returns></returns>
        public XDocument GetBigForecast()
        {
            var uriString = ShortRequestString + String.Format("&q={0}&cnt=14", City);
            return GetResponseStream(uriString);
        }
    }
}
