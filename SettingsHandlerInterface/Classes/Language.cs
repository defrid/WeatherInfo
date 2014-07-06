using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SettingsHandlerInterface.Classes
{
    /// <summary>
    /// Класс языка системы
    /// </summary>
    public class Language
    {
        public Language() { }

        /// <summary>
        /// Конструктор класса, создаёт экземпляр, хранящий информацию о языке локализации программы.
        /// </summary>
        /// <param name="_rusName">Наименование на русском</param>
        /// <param name="_engName">Наименование на английском</param>
        public Language(string _rusName, string _engName)
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
