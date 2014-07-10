using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Tomers.WPF.Localization;

namespace DataHandlerInterface.Classes
{
    public class Options
    {
        #region Единицы измерения температуры

        /// <summary>
        /// Список, хранящий единицы измерения температуры
        /// </summary>
        private static readonly List<TemperatureUnits> temperatureUnits = new List<TemperatureUnits> { new TemperatureUnits("Цельсии", "Celsius"), 
                                                                                              new TemperatureUnits("Кельвины", "Kelvin"),
                                                                                              new TemperatureUnits("Фаренгейты", "Fahrenheit") };

        public static List<TemperatureUnits> GetTemperatureUnits()
        {
            return temperatureUnits;
        }
        #endregion

        #region Языки для системы

        /// <summary>
        /// Список, хранящий языки для системы (Расский, Английкский, ...)
        /// </summary>
        private static readonly List<Language> languages = new List<Language> { new Language("Русский", "Russian"), new Language("Английский", "English") };

        /// <summary>
        /// Возвращает список языков для системы
        /// </summary>
        /// <returns>Список языков для системы</returns>
        public static List<Language> GetLanguages()
        {
            return languages;
        }
        #endregion

        /// <summary>
        /// Перечисление форматов прогноза погоды (по неделям, по дням)
        /// </summary>
        public enum FormatForecast
        {
            [FormatAttribute("Краткий", "Short")]
            Short,
            [FormatAttribute("Подробный", "Detailed")]
            Detailed
        }

        /// <summary>
        /// Аттрибут для формата прогноза погоды, предоставляет локализацию
        /// </summary>
        [AttributeUsage(AttributeTargets.All)]
        internal class FormatAttribute : Attribute
        {
            public string nameRus = string.Empty;
            public string nameEng = string.Empty;
            public FormatAttribute(string _nameRus, string _nameEng)
            {
                nameRus = _nameRus;
                nameEng = _nameEng;
            }
        }

        /// <summary>
        /// Для формата прогноза возвращает аттрибут (по сути русская локализация для combobox)
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetFormatAttribute(string format, string language)
        {
            try
            {
                var fieldInfo = typeof(FormatForecast).GetField(format);
                var attributes = (FormatAttribute[])fieldInfo.GetCustomAttributes(typeof(FormatAttribute), false);

                return attributes.Length == 0 ? String.Empty : (language == "English" ? attributes[0].nameEng : attributes[0].nameRus);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DataHandler.GetFormatAttribute(): " + LanguageDictionary.Current.Translate<string>("getFormatAttributeFailed_Option", "Content"));
                return String.Empty;
            }
        }

        //
        /// <summary>
        /// Для формата прогноза возвращает формат по атрибуту.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string GetValueByAttribute(string attribute, string language)
        {
            try
            {
                var ff = Enum.GetNames(typeof(FormatForecast));
                foreach (var format in ff.Where(format => attribute == GetFormatAttribute(format, language)))
                {
                    return format;
                }
                return String.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DataHandler.GetValueByAttribute(): " + LanguageDictionary.Current.Translate<string>("getValueByAttributeFailed_Option", "Content"));
                return String.Empty;
            }
        }
    }
}
