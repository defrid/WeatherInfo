using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SettingsHandlerInterface.Classes
{
    /// <summary>
    /// Класс города
    /// </summary>
    public class City
    {
        public int cityYaId { get; set; }
        public int cityOWId { get; set; }
        public string cityRusName { get; set; }
        public string cityEngName { get; set; }

        public City() { }

        /// <summary>
        /// Конструктор класса, создаёт экземпляр, хранящий информацию о городе.
        /// </summary>
        /// <param name="_cityYaId">ИД города из Яндекса</param>
        /// <param name="_cityOWId">ИД города из OpenWeather</param>
        /// <param name="_cityRusName">Название города на русском</param>
        /// <param name="_cityEngName">Название города на английском</param>
        public City(int _cityYaId, int _cityOWId, string _cityRusName, string _cityEngName)
        {
            cityYaId = _cityYaId;
            cityOWId = _cityOWId;
            cityRusName = _cityRusName;
            cityEngName = _cityEngName;
        }
    }
}
