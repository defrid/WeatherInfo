using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using WeatherInfo.Interfaces;

namespace WeatherInfo.Classes
{
    public class XMLSettingsHandler : ISettingsHandler
    {
        public static String XMLFileName = Application.StartupPath + @"\Config\settings.xml";

        public void SaveSettings(Settings settings)
        {
            WriteXml(settings);
        }

        public Settings LoadSettings()
        {
            var settings = ReadXml();

            return settings;
        }

        //Запись настроек в файл
        private static void WriteXml(Settings settings)
        {
            try
            {
                var isValid = ValidateSettings(settings);
                if (!isValid)
                {
                    throw new Exception("Переданные настройки не прошли проверку на валиндность. Изменения не будут сохранены.");
                }

                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                using (TextWriter writer = new StreamWriter(XMLFileName))
                {
                    ser.Serialize(writer, settings);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SettingsHandler.WriteXml(): Непредвиденная ошибка. Не удалось сохранить настройки. Текст ошибки: " + ex.Message);
                MessageBox.Show("Непредвиденная ошибка. Не удалось сохранить настройки. Текст ошибки: " + ex.Message);
            }
        }

        //Чтение настроек из файла
        private static Settings ReadXml()
        {
            try
            {
                if (File.Exists(XMLFileName))
                {
                    Settings settings = new Settings();

                    XmlSerializer ser = new XmlSerializer(typeof(Settings));
                    using (TextReader reader = new StreamReader(XMLFileName))
                    {
                        settings = ser.Deserialize(reader) as Settings;
                    }
                    var isValid = ValidateSettings(settings);
                    if (isValid)
                    {
                        return settings;
                    }
                    else
                    {
                        throw new Exception("Файл настроек не прошел проверку на валиндность.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SettingsHandler.ReadXml(): Непредвиденная ошибка. Будут загружены настройки по-умолчанию, если это возможно. Текст ошибки: " + ex.Message);
                MessageBox.Show("Непредвиденная ошибка. Будут загружены настройки по-умолчанию, если это возможно. Текст ошибки: " + ex.Message);
            }
            return GetDefaultSettings();
        }

        /// <summary>
        /// Метод проверяет переданные ему настройки на корректность.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private static bool ValidateSettings(Settings settings)
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
