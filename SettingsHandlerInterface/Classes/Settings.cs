using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SettingsHandlerInterface.Classes
{
    /// <summary>
    /// Перечисление форматов прогноза погоды (по неделям, по дням, по часам)
    /// </summary>
    public enum FormatForecast
    {
        [FormatAttribute("По неделям")]
        Weeks,
        [FormatAttribute("По дням")]
        Days
    }

    [AttributeUsage(AttributeTargets.All)]
    internal class FormatAttribute : Attribute
    {
        public string name = string.Empty;
        public FormatAttribute(string _name)
        { 
            name = _name; 
        }
    }

    public class FormatHandler
    {
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

    /// <summary>
    /// Класс настроек приложения
    /// </summary>
    public class Settings
    {
        public Settings() { }

        public Settings(string _country, int cityId, string _cityName, string _format, int _updatePeriod, bool _autostart)
        {
            country = _country;
            city = new City(cityId, _cityName);
            format = _format;
            updatePeriod = _updatePeriod;
            autostart = _autostart;
        }

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

        public string country { get; set; }
        public City city { get; set; }
        public string format { get; set; }
        public int updatePeriod { get; set; }
        public bool autostart { get; set; }

        /// <summary>
        /// Настройки по-умолчанию
        /// </summary>
        /// <returns></returns>
        public static Settings GetDefaultSettings()
        {
            string country = "Россия";
            int cityId = 524901;
            string cityName = "Moscow";
            string format = Enum.GetName(typeof(FormatForecast), FormatForecast.Days);
            int updatePeriod = 10;
            bool autostart = true;

            var settings = new Settings(country, cityId, cityName, format, updatePeriod, autostart);

            return settings;
        }
    }    
}
