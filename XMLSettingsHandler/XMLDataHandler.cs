using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;
using DataHandlerInterface.Interfaces;
using DataHandlerInterface.Classes;
using System.Reflection;
using Tomers.WPF.Localization;

namespace XMLDataHandler
{
    public class XmlDataHandler : DataHandler
    {
        public static String XmlFileName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + /*Application.StartupPath + */@"\Config\settings.xml";

        public override void SaveSettings(UserSettings settings)
        {
            WriteXml(settings);
        }

        public override void SaveForecastDay(ForecastDayModel forecastDay)
        {
            //throw new NotImplementedException();
        }

        public override void SaveForecastHour(ForecastHourModel forecastHour)
        {
            //throw new NotImplementedException();
        }

        public override List<ForecastDayModel> LoadForecastDays()
        {
            return null;
            //throw new NotImplementedException();
        }

        public override List<ForecastHourModel> LoadForecastHours()
        {
            return null;
            //throw new NotImplementedException();
        }

        public override UserSettings LoadSettings()
        {
            var settings = ReadXml();

            return settings;
        }

        //Запись настроек в файл
        private static void WriteXml(UserSettings settings)
        {
            try
            {
                var isValid = ValidateSettings(settings);
                if (!isValid)
                {
                    throw new Exception(LanguageDictionary.Current.Translate<string>("writeXmlValidateSettings_SttsHandler", "Content"));
                }

                var ser = new XmlSerializer(typeof(UserSettings));
                using (TextWriter writer = new StreamWriter(XmlFileName))
                {
                    ser.Serialize(writer, settings);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("XmlDataHandler.WriteXml(): " + LanguageDictionary.Current.Translate<string>("writeXml_SttsHandler", "Content") + ex.Message);
                throw new Exception();
                //throw new Exception(LanguageDictionary.Current.Translate<string>("writeXml_SttsHandler", "Content"));
                //MessageBox.Show("Непредвиденная ошибка. Не удалось сохранить настройки. Текст ошибки: " + ex.Message);
            }
        }

        //Чтение настроек из файла
        private static UserSettings ReadXml()
        {
            try
            {
                //WriteXml(Settings.GetDefaultSettings());
                if (!File.Exists(XmlFileName))
                {
                    return UserSettings.GetDefaultSettings();
                }

                var settings = new UserSettings();

                var ser = new XmlSerializer(typeof(UserSettings));
                using (TextReader reader = new StreamReader(XmlFileName))
                {
                    settings = ser.Deserialize(reader) as UserSettings;
                }

                var isValid = ValidateSettings(settings);
                if (isValid)
                {
                    return settings;
                }
                else
                {
                    throw new Exception(LanguageDictionary.Current.Translate<string>("readXmlValidateSettings_SttsHandler", "Content"));
                }
                return UserSettings.GetDefaultSettings();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("XmlDataHandler.ReadXml(): " + LanguageDictionary.Current.Translate<string>("readXml_SttsHandler", "Content") + ex.Message);
                //throw new Exception(LanguageDictionary.Current.Translate<string>("readXml_SttsHandler", "Content"));
                return UserSettings.GetDefaultSettings();
            }
        }
    }
}
