using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Tomers.WPF.Localization;

namespace SettingsHandlerInterface.Classes
{
    public class Options
    {
        #region Единицы измерения температуры

        /// <summary>
        /// Список, хранящий единицы измерения температуры
        /// </summary>
        private static List<TemperatureUnits> temperatureUnits = new List<TemperatureUnits> { new TemperatureUnits("Цельсии", "Celsius"), 
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
        private static List<Language> languages = new List<Language> { new Language("Русский", "Russian"), new Language("Английский", "English") };

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
            [FormatAttribute("По неделям")]
            Weeks,
            [FormatAttribute("По дням")]
            Days
        }

        /// <summary>
        /// Аттрибут для формата прогноза погоды, предоставляет локализацию
        /// </summary>
        [AttributeUsage(AttributeTargets.All)]
        internal class FormatAttribute : Attribute
        {
            public string name = string.Empty;
            public FormatAttribute(string _name)
            {
                name = _name;
            }
        }

        /// <summary>
        /// Для формата прогноза возвращает аттрибут (по сути русская локализация для combobox)
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetFormatAttribute(string format)
        {
            try
            {
                FieldInfo fieldInfo = typeof(FormatForecast).GetField(format);
                FormatAttribute[] attributes = (FormatAttribute[])fieldInfo.GetCustomAttributes(typeof(FormatAttribute), false);

                return attributes.Length == 0 ? String.Empty : attributes[0].name;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SettingsHandler.GetFormatAttribute(): " + LanguageDictionary.Current.Translate<string>("getFormatAttributeFailed_Option", "Content"));
                return String.Empty;
            }
        }

        //
        /// <summary>
        /// Для формата прогноза возвращает формат по атрибуту.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string GetValueByAttribute(string attribute)
        {
            try
            {
                var ff = Enum.GetNames(typeof(FormatForecast));
                foreach (var format in ff)
                {
                    if (attribute == GetFormatAttribute(format))
                    {
                        return format;
                    }
                }
                return String.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SettingsHandler.GetValueByAttribute(): " + LanguageDictionary.Current.Translate<string>("getValueByAttributeFailed_Option", "Content"));
                return String.Empty;
            }
        }
    }
}
