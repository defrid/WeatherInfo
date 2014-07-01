using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using WeatherInfo.Interfaces;

namespace WeatherInfo.Classes
{
    public class WeatherApi:IWeatherApi
    {

        public string City { get; protected set; }

        private static readonly string CurentRequestString =
            @"http://api.openweathermap.org/data/2.5/weather?mode=xml&lang=ru";
        private static readonly string DetainedRequestString =
            @"http://api.openweathermap.org/data/2.5/forecast?&mode=xml&lang=ru";
        private static readonly string ShortRequestString=
            @"http://api.openweathermap.org/data/2.5/daily?&mode=xml&lang=ru";
        
        public WeatherApi(string city)
        {
            City = city;
        }

        /// <summary>
        /// Получение ответа от сервера по URI
        /// </summary>
        /// <param name="uriString">Строка uri запроса</param>
        /// <returns></returns>
        private string GetResponseData(string uriString)
        {
            string result;
            var request = WebRequest.Create(uriString);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
                if (stream != null)
                    using (var reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                else
                    result = null;
            return result;
        }

        public XmlDocument GetCurrentForecast()
        {
            throw new NotImplementedException();
        }

        public XmlDocument GetDailyForecast(int days)
        {
            throw new NotImplementedException();
        }

        public XmlDocument GetDetailedWeek()
        {
            throw new NotImplementedException();
        }

        public XmlDocument GetBigForecast()
        {
            throw new NotImplementedException();
        }
    }
}
