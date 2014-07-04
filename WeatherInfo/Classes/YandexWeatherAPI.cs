using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using WeatherInfo.Interfaces;

namespace WeatherInfo.Classes
{
    /// <summary>
    /// Класс организующий связь с Yandex.Weather API
    /// </summary>
    public class YandexWeatherAPI:IYandexWeatherApi
    {
        /// <summary>
        /// Ссылка на Xml документ с городами
        /// </summary>
        public const string CountryCityXmlLink = @"http://weather.yandex.ru/static/cities.xml";
        /// <summary>
        /// Заголовок ссылки на прогноз
        /// </summary>
        public const string ForecastWayLink = @"http://export.yandex.ru/weather-ng/forecasts/";
        /// <summary>
        /// Заголовок ссылки на иконку погоды
        /// </summary>
        public const string ForecastImageLink = @"http://yandex.st/weather/1.2.49/i/icons/48x48/";

        /// <summary>
        /// ИД города
        /// </summary>
        public string CityId { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="cityId">ИД города</param>
        public YandexWeatherAPI(string cityId)
        {
            CityId = cityId;
        }

        /// <summary>
        /// метод для получения списков всех городов
        /// </summary>
        /// <returns></returns>
        public static XDocument GetCities()
        {
            var request = WebRequest.Create(CountryCityXmlLink);
            using (var response = request.GetResponse())
            {
                var stream = response.GetResponseStream();
                return stream != null ? XDocument.Load(stream) : null;
            }
        }
        /// <summary>
        /// Получить Bitmap рисунок погоды
        /// </summary>
        /// <param name="imageId">ИД картинки</param>
        /// <returns></returns>
        public static Bitmap GetImageById(string imageId)
        {
            var link = String.Format("{0}{1}.png",ForecastWayLink , imageId);
            var request = WebRequest.Create(link);
            using (var response = request.GetResponse())
            {
                var stream = response.GetResponseStream();
                return stream != null ? new Bitmap(stream) : null;
            }
        }
        /// <summary>
        /// Получить BitmapImage рисунок погоды
        /// </summary>
        /// <param name="imageId">ИД картинки</param>
        /// <returns></returns>
        public static BitmapImage GetBitmapImageById(string imageId)
        {
            var link=String.Format("{0}{1}.png",ForecastImageLink , imageId);
            var uri = new Uri(link);
            return new BitmapImage(uri);
        }

        /// <summary>
        /// Получить прогноз в виде XML
        /// </summary>
        /// <returns></returns>
        public XDocument GetForecast()
        {
            var link = ForecastWayLink + String.Format("{0}.xml", CityId);
            XDocument doc;
            var request = WebRequest.Create(link);
            using (var response = request.GetResponse())
                doc = XDocument.Load(response.GetResponseStream());
            return doc;
        }
    }
}
