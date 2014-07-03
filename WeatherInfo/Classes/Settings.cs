using System;
using System.Collections.Generic;
using System.ComponentModel;
<<<<<<< HEAD
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
=======
using System.Linq;
using System.Text;
using System.Windows.Forms;
>>>>>>> origin/SettingsWindow

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
<<<<<<< HEAD
        Days,
        [FormatAttribute("По часам")]
        Hours
=======
        Days
>>>>>>> origin/SettingsWindow
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
<<<<<<< HEAD
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
=======
>>>>>>> origin/SettingsWindow
    /// Класс настроек приложения
    /// </summary>
    public class Settings
    {
<<<<<<< HEAD
        public Settings()
        {
            country = "RU";
            city = new City(524901, "Moscow");
            format = Enum.GetName(typeof(FormatForecast), FormatForecast.Days);
            delay = Enum.GetName(typeof(Delay), Delay.slightDelay);//Delay.slightDelay;
            autostart = true;
        }

        public Settings(string _country, int cityId, string _cityName, string _format, string _delay, bool _autostart)
=======
        public Settings() { }

        public Settings(string _country, int cityId, string _cityName, string _format, int _updatePeriod, bool _autostart)
>>>>>>> origin/SettingsWindow
        {
            country = _country;
            city = new City(cityId, _cityName);
            format = _format;
<<<<<<< HEAD
            delay = _delay;
=======
            updatePeriod = _updatePeriod;
>>>>>>> origin/SettingsWindow
            autostart = _autostart;
        }

        /// <summary>
        /// Класс города
        /// </summary>
        public class City
        {
<<<<<<< HEAD
            public int id { get; set; }
            public string name { get; set; }

            public City(int _id, string _name)
            {
                id = _id;
                name = _name;
=======
            public int cityId { get; set; }
            public string cityName { get; set; }

            public City() { }

            public City(int _cityId, string _cityName)
            {
                cityId = _cityId;
                cityName = _cityName;
>>>>>>> origin/SettingsWindow
            }
        }

        public string country { get; set; }
        public City city { get; set; }
        public string format { get; set; }
<<<<<<< HEAD
        public string delay { get; set; }
        public bool autostart { get; set; }
    }

    public class SettingsHandler
    {
        public static String XMLFileName = Application.StartupPath + @"\Config\settings.xml";

        //Запись настроек в файл
        public static void WriteXml(Settings settings)
        {
            XDocument setts = new XDocument();

            XElement root = new XElement("Settings");
            root.Add(new XElement("country", settings.country));

            XElement city = new XElement("city");
            city.Add(new XElement("id", settings.city.id));
            city.Add(new XElement("name", settings.city.name));
            root.Add(city);

            root.Add(new XElement("delay", settings.delay));
            root.Add(new XElement("format", settings.format));
            root.Add(new XElement("autostart", settings.autostart.ToString()));

            setts.Add(root);            

            //XmlSerializer ser = new XmlSerializer(typeof(Settings));
            using (TextWriter writer = new StreamWriter(XMLFileName))
            {
                setts.Save(writer);
                //ser.Serialize(writer, settings);
                writer.Close();
            }           
        }

        //Чтение настроек из файла
        public static Settings ReadXml()
        {
            if (File.Exists(XMLFileName))
            {
                XDocument setts = XDocument.Load(XMLFileName);              

                
                Settings settings = new Settings();
                settings.country = setts.Root.Element("country").Value;
                settings.city.id = (int)float.Parse(setts.Root.Element("city").Element("id").Value);
                settings.city.name = setts.Root.Element("city").Element("name").Value;
                settings.delay = setts.Root.Element("delay").Value;
                settings.format = setts.Root.Element("format").Value;
                settings.autostart = bool.Parse(setts.Root.Element("autostart").Value);

                //XmlSerializer ser = new XmlSerializer(typeof(Settings));
                //using (TextReader reader = new StreamReader(XMLFileName))
                //{
                //    settings = ser.Deserialize(reader) as Settings;
                //    reader.Close();
                    
                //}
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
=======
        public int updatePeriod { get; set; }
        public bool autostart { get; set; }
    }    
>>>>>>> origin/SettingsWindow
}
