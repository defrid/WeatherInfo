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
        public static String XmlSettingsFileName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + /*Application.StartupPath + */@"\Config\UserSettings.xml";
        //public static String XmlForecastDayFileName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + /*Application.StartupPath + */@"\Data\ForecastDay.xml";
        //public static String XmlForecastHourFileName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + /*Application.StartupPath + */@"\Data\ForecastHour.xml";
        public static String XmlForecastDetailedFileName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + /*Application.StartupPath + */@"\Data\ForecastDetailed.xml";

        public override void SaveSettings(UserSettings settings)
        {
            WriteSettingsToXml(settings);
        }

        //public override void SaveForecastDay(ForecastDayModel forecastDay)
        //{
        //    //throw new NotImplementedException();
        //}

        //public override void SaveForecastHour(ForecastHourModel forecastHour)
        //{
        //    //throw new NotImplementedException();
        //}

        public override void SaveForecasrDetailed(ForecastDetailed forecastDetailed)
        {
            WriteForecastDetailedToXml(forecastDetailed);
        }

        //public override List<ForecastDayModel> LoadForecastDays()
        //{
        //    return null;
        //    //throw new NotImplementedException();
        //}

        //public override List<ForecastHourModel> LoadForecastHours()
        //{
        //    return null;
        //    //throw new NotImplementedException();
        //}

        public override ForecastDetailed LoadForecastDetailed()
        {
            var forecastDetailed = ReadForecastDetailedFromXml();

            return forecastDetailed;
        }

        public override UserSettings LoadSettings()
        {
            var settings = ReadSettingsFromXml();

            return settings;
        }

        //Запись настроек в файл
        private static void WriteSettingsToXml(UserSettings settings)
        {
            try
            {
                var isValid = ValidateSettings(settings);
                if (!isValid)
                {
                    throw new Exception(LanguageDictionary.Current.Translate<string>("writeXmlValidateSettings_SttsHandler", "Content"));
                }

                var ser = new XmlSerializer(typeof(UserSettings));
                using (TextWriter writer = new StreamWriter(XmlSettingsFileName))
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
        private static UserSettings ReadSettingsFromXml()
        {
            try
            {
                //WriteXml(Settings.GetDefaultSettings());
                if (!File.Exists(XmlSettingsFileName))
                {
                    return UserSettings.GetDefaultSettings();
                }

                var settings = new UserSettings();

                var ser = new XmlSerializer(typeof(UserSettings));
                using (TextReader reader = new StreamReader(XmlSettingsFileName))
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

        //Запись прогноза погоды в файл
        /*private static void WriteForecastDayToXml(ForecastDayModel forecastDay)
        {
            try
            {
                //var isValid = ValidateSettings(settings);
                //if (!isValid)
                //{
                //    throw new Exception(LanguageDictionary.Current.Translate<string>("writeXmlValidateSettings_SttsHandler", "Content"));
                //}

                var ser = new XmlSerializer(typeof(ForecastDayModel));
                using (TextWriter writer = new StreamWriter(XmlForecastDayFileName))
                {
                    ser.Serialize(writer, forecastDay);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("XmlDataHandler.WriteXml(): " + LanguageDictionary.Current.Translate<string>("writeXml_SttsHandler", "Content") + ex.Message);
                //throw new Exception();
                //throw new Exception(LanguageDictionary.Current.Translate<string>("writeXml_SttsHandler", "Content"));
                //MessageBox.Show("Непредвиденная ошибка. Не удалось сохранить настройки. Текст ошибки: " + ex.Message);
            }
        }

        /// <summary>
        /// Загружает прогноз погоды из XML-файла
        /// </summary>
        /// <returns></returns>
        private static ForecastDayModel ReadForecastDayFromXml()
        {
            try
            {
                //WriteXml(Settings.GetDefaultSettings());
                if (!File.Exists(XmlForecastDayFileName))
                {
                    return null;//UserSettings.GetDefaultSettings();
                }

                var forecastDay = new ForecastDayModel();

                var ser = new XmlSerializer(typeof(ForecastDayModel));
                using (TextReader reader = new StreamReader(XmlForecastDayFileName))
                {
                    forecastDay = ser.Deserialize(reader) as ForecastDayModel;
                }

                //var isValid = ValidateSettings(settings);
                //if (isValid)
                //{
                //    return settings;
                //}
                //else
                //{
                //    throw new Exception(LanguageDictionary.Current.Translate<string>("readXmlValidateSettings_SttsHandler", "Content"));
                //}
                return forecastDay;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("XmlDataHandler.ReadXml(): " + LanguageDictionary.Current.Translate<string>("readXml_SttsHandler", "Content") + ex.Message);
                //throw new Exception(LanguageDictionary.Current.Translate<string>("readXml_SttsHandler", "Content"));
                return null;
            }
        }*/

        private static void WriteForecastDetailedToXml(ForecastDetailed forecastDetailed)
        {
            try
            {
                var ser = new XmlSerializer(typeof(ForecastDetailed));
                using (TextWriter writer = new StreamWriter(XmlForecastDetailedFileName))
                {
                    ser.Serialize(writer, forecastDetailed);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WriteForecastDetailed.WriteXml(): " + LanguageDictionary.Current.Translate<string>("writeXml_SttsHandler", "Content") + ex.Message);
                //throw new Exception();
                //throw new Exception(LanguageDictionary.Current.Translate<string>("writeXml_SttsHandler", "Content"));
                //MessageBox.Show("Непредвиденная ошибка. Не удалось сохранить настройки. Текст ошибки: " + ex.Message);
            }
        }

        private static ForecastDetailed ReadForecastDetailedFromXml()
        {
            try
            {
                //WriteXml(Settings.GetDefaultSettings());
                if (!File.Exists(XmlForecastDetailedFileName))
                {
                    return null;//UserSettings.GetDefaultSettings();
                }

                var forecastDetailed = new ForecastDetailed();

                var ser = new XmlSerializer(typeof(ForecastDetailed));
                using (TextReader reader = new StreamReader(XmlForecastDetailedFileName))
                {
                    forecastDetailed = ser.Deserialize(reader) as ForecastDetailed;
                }

                return forecastDetailed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("XmlDataHandler.ReadForecastDetailedFromXml(): " + LanguageDictionary.Current.Translate<string>("readXml_SttsHandler", "Content") + ex.Message);
                //throw new Exception(LanguageDictionary.Current.Translate<string>("readXml_SttsHandler", "Content"));
                return null;
            }
        }
    }
}
