using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SettingsHandlerInterface.Classes
{
    /// <summary>
    /// Класс страны
    /// </summary>
    public class Country
    {
        public string countryId { get; set; }
        public string countryRusName { get; set; }
        public string countryEngName { get; set; }

        public Country() { }

        /// <summary>
        /// Конструктор класса, создаёт экземпляр, хранящий информацию о стране
        /// </summary>
        /// <param name="_countryId">ИД страны из Яндекса</param>
        /// <param name="_countryRusName">Наименование страны на руском</param>
        /// <param name="_countryEngName">Наименование страны на английском</param>
        public Country(string _countryId, string _countryRusName, string _countryEngName)
        {
            countryId = _countryId;
            countryRusName = _countryRusName;
            countryEngName = _countryEngName;
        }
    }
}
