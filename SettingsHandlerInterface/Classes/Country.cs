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
        public string countryName { get; set; }

        public Country() { }

        public Country(string _countryId, string _countryName)
        {
            countryId = _countryId;
            countryName = _countryName;
        }
    }
}
