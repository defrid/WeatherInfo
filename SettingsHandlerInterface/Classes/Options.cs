using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SettingsHandlerInterface.Classes
{
    public class Options
    {
        /// <summary>
        /// Перечисление форматов единиц измерения температуры
        /// </summary>
        public enum TemperatureUnits
        {
            Celsius,
            Kelvin,
            Fahrenheit
        }

        /// <summary>
        /// Языки
        /// </summary>
        public enum Languages
        {
            Russian,
            English
        }

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
        /// Для формата прогноза возвращает аттрибут (по сути русская локализация для combobox) для формата
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
                Debug.WriteLine("SettingsHandler.GetFormatAttribute(): Ошибка при получении атрибута формата прогноза погоды. Текст ошибки: " + ex.Message);
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SettingsHandler.GetValueByAttribute(): Ошибка при получении формата прогноза погоды по заданному атрибуту. Текст ошибки: " + ex.Message);
            }
            return String.Empty;
        }
    }
}
