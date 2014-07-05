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
        public int cityId { get; set; }
        public string cityName { get; set; }

        public City() { }

        public City(int _cityId, string _cityName)
        {
            cityId = _cityId;
            cityName = _cityName;
        }
    }
}
