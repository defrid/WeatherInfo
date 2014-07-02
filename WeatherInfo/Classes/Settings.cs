using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
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
        Days,
        [FormatAttribute("По часам")]
        Hours
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
    /// Перечисление задержек для всплывающего сообщения в трее
    /// </summary>
    public enum Delay
    {    
        lowDelay = 10,
        slightDelay = 15,
        avarageDelay = 20,
        longDelay = 30
    }

    /// <summary>
    /// Класс настроек приложения
    /// </summary>
    public class Settings
    {
        public Settings()
        {
            country = "Россия";
            city = "Москва";
            format = Enum.GetName(typeof(FormatForecast), FormatForecast.Days);
            delay = Enum.GetName(typeof(Delay), Delay.slightDelay);//Delay.slightDelay;
            autostart = true;
        }

        public Settings(string _country, string _city, string _format, string _delay, bool _autostart)
        {
            country = _country;
            city = _city;
            format = _format;
            delay = _delay;
            autostart = _autostart;
        }

        public string country { get; set; }
        public string city { get; set; }
        public string format { get; set; }
        public string delay { get; set; }
        public bool autostart { get; set; }
    }

    public class SettingsHandler
    {
        public static String XMLFileName = Application.StartupPath + @"\Config\settings.xml";
        //public Settings settings;

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
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                //TextReader reader = new StreamReader(XMLFileName);
                Settings settings = new Settings();
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

        public static string GetFormatAttribute(string format)
        {
            FieldInfo fieldInfo = typeof(FormatForecast).GetField(format);
            FormatAttribute[] attributes = (FormatAttribute[])fieldInfo.GetCustomAttributes(typeof(FormatAttribute), false);

            return attributes.Length == 0 ? String.Empty : attributes[0].name;
        }

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
