using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SettingsHandlerInterface.Classes
{
    /// <summary>
    /// Класс региона города
    /// </summary>
    public class RegionOfCity
    {
        public int regionId { get; set; }
        public string regionName { get; set; }

        public RegionOfCity() { }

        public RegionOfCity(int _regionId, string _regionName)
        {
            regionId = _regionId;
            regionName = _regionName;
        }
    }
}
