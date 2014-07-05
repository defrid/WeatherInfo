using SettingsHandlerInterface.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SettingsHandlerInterface
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
        public static bool ValidateSettings(Settings settings)
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

                if (settings == null || settings.cities.Count == 0)
                {
                    return false;
                }

                foreach (var city in settings.cities)
                {
                    if (string.IsNullOrWhiteSpace(city.country.countryId))
                    {
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(city.country.countryName))
                    {
                        return false;
                    }

                    if (city.city.cityId < 0)
                    {
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(city.city.cityName))
                    {
                        return false;
                    }
                }

                

                if (string.IsNullOrWhiteSpace(settings.format))
                {
                    return false;
                }

                if (settings.updatePeriod < 10 || settings.updatePeriod > 180)
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
    }
}
