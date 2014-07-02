using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WeatherInfo.Classes
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

    /// <summary>
    /// Класс настроек приложения
    /// </summary>
    public class Settings
    {
        public Settings()
        {
            country = "Россия";
            city = new City(524901, "Moscow");
            format = Enum.GetName(typeof(FormatForecast), FormatForecast.Days);
            updatePeriod = 10;
            autostart = true;
        }

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
            public int id { get; set; }
            public string name { get; set; }

            public City()
            {

            }

            public City(int _id, string _name)
            {
                id = _id;
                name = _name;
            }
        }

        public string country { get; set; }
        public City city { get; set; }
        public string format { get; set; }
        public int updatePeriod { get; set; }
        public bool autostart { get; set; }
    }

    public class SettingsHandler
    {
        public static String XMLFileName = Application.StartupPath + @"\Config\settings.xml";

        //Запись настроек в файл
        public static void WriteXml(Settings settings)
        {          

            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            using (TextWriter writer = new StreamWriter(XMLFileName))
            {
                ser.Serialize(writer, settings);
                writer.Close();
            }           
        }

        //Чтение настроек из файла
        public static Settings ReadXml()
        {
            if (File.Exists(XMLFileName))
            {
                Settings settings = new Settings();

                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                using (TextReader reader = new StreamReader(XMLFileName))
                {
                    settings = ser.Deserialize(reader) as Settings;
                    reader.Close();

                }
                return settings;
            }
            else
            {
                return new Settings();
            }
        }

        /// <summary>
        /// Для формата прогноза возвращает аттрибут (по сути русская локализация для combobox) для формата
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetFormatAttribute(string format)
        {
            FieldInfo fieldInfo = typeof(FormatForecast).GetField(format);
            FormatAttribute[] attributes = (FormatAttribute[])fieldInfo.GetCustomAttributes(typeof(FormatAttribute), false);

            return attributes.Length == 0 ? String.Empty : attributes[0].name;
        }

        //
        /// <summary>
        /// Для формата прогноза возвращает формат по атрибуту.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string GetValueByAttribute(string attribute)
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
    }
}
