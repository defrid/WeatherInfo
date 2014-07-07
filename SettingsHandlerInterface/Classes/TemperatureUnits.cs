using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SettingsHandlerInterface.Classes
{
    /// <summary>
    /// Класс формата единиц измерения температуры
    /// </summary>
    public class TemperatureUnits
    {
        public TemperatureUnits() { }

        /// <summary>
        /// Конструктор класса, создаёт экземпляр класса, хранящий информацию о единицах измерения температуры.
        /// </summary>
        /// <param name="_rusName">Наименование на русском</param>
        /// <param name="_engName">Наименование на английском</param>
        public TemperatureUnits(string _rusName, string _engName)
        {
            rusName = _rusName;
            engName = _engName;
        }

        public string rusName { get; set; }
        public string engName { get; set; }

        public override string ToString()
        {
            return string.Format("{1} | {0}", rusName, engName);
        }
    }
}
