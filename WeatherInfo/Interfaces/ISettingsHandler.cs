using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using WeatherInfo.Classes;

namespace WeatherInfo.Interfaces
{
    public interface ISettingsHandler
    {
        /// <summary>
        /// Метод загржуает настройки из хранилища.
        /// </summary>
        /// <returns></returns>
        Settings LoadSettings();

        /// <summary>
        /// Метод сохраняет настройки в хранилище.
        /// </summary>
        /// <param name="settings"></param>
        void SaveSettings(Settings settings);
    }

    public abstract class SettingsHandler : ISettingsHandler
    {
        public abstract Settings LoadSettings();

        public abstract void SaveSettings(Settings settings);

        /// <summary>
        /// Метод проверяет переданные ему настройки на корректность.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        internal static bool ValidateSettings(Settings settings)
        {

            try
            {
                var isValid = true;

                //FieldInfo[] fields = typeof(FormatForecast).GetFields(BindingFlags.Public | BindingFlags.Instance);
                //foreach (var field in fields)
                //{
                //    if (!ValidSettingField(field, settings))
                //    {
                //        isValid = false;
                //        break;
                //    }
                //}

                if (string.IsNullOrWhiteSpace(settings.country))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(settings.format))
                {
                    return false;
                }

                if (settings.updatePeriod < 10 || settings.updatePeriod > 180)
                {
                    return false;
                }

                if (settings.city.cityId <= 0)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(settings.city.cityName))
                {
                    return false;
                }

                return isValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //private static bool ValidSettingField(FieldInfo field, Settings settings)
        //{
        //    switch (field.GetType().ToString())
        //    {
        //        case "int":
        //            if ((int)field.GetValue(settings) <= 0 || (int)field.GetValue(settings) == null)
        //            {
        //                return false;
        //            }
        //            break;
        //        case "string":
        //            if (string.IsNullOrWhiteSpace((string)field.GetValue(settings)))
        //            {
        //                return false;
        //            }
        //            break;
        //        case "bool":
        //            if ((bool)field.GetValue(settings) == null)
        //            {
        //                return false;
        //            }
        //            break;
        //        case "City":
        //            FieldInfo[] cityFields = typeof(Settings.City).GetFields(BindingFlags.Public | BindingFlags.Instance);
        //            foreach (var cityField in cityFields)
        //            {
        //                if (!ValidSettingField(cityField, settings))
        //                {
        //                    return false;
        //                }
        //            }
        //            break;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Для формата прогноза возвращает аттрибут (по сути русская локализация для combobox) для формата
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        internal static string GetFormatAttribute(string format)
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
        internal static string GetValueByAttribute(string attribute)
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

        /// <summary>
        /// Настройки по-умолчанию
        /// </summary>
        /// <returns></returns>
        internal static Settings GetDefaultSettings()
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
