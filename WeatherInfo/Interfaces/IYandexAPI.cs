using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace WeatherInfo.Interfaces
{
    interface IYandexWeatherApi
    {
        /// <summary>
        /// Получить прогноз в виде XML
        /// </summary>
        /// <returns></returns>
        XDocument GetForecast();
        
    }
}
