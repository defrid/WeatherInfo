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

        public Country(string _countryId, string _countryRusName, string _countryEngName)
        {
            countryId = _countryId;
            countryRusName = _countryRusName;
            countryEngName = _countryEngName;
        }
    }
}
