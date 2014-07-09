using System;

namespace DataHandlerInterface.Classes
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

        public override bool Equals(object _langCompare)
        {
            if (_langCompare == null)
            {
                return false;
            }
            var langCompare = (Language)_langCompare;
            var isEqual = ((rusName == langCompare.rusName) && (engName == langCompare.engName));

            return isEqual;
        }
    }
}
