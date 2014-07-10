using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    #region Для расширяемости с помощью инструмента MEF
    [Export(typeof(IDataHandler))]
    #endregion
    public class XmlDataHandler : DataHandler
    {
        private static String XmlSettingsDirName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Config";
        private static String XmlSettingsFileName = XmlSettingsDirName + @"\UserSettings.xml";
        private static String XmlForecastDetailedDirName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Data";
        private static String XmlForecastDetailedFileName = XmlForecastDetailedDirName + @"\ForecastDetailed.xml";


        #region public Save
        /// <summary>
        /// Метод сохраняет настройки в хранилище.
        /// </summary>
        /// <param name="settings">Объект, содержащий сохраняемые настройки</param>
        public override void SaveSettings(UserSettings settings)
        {
            WriteSettingsToXml(settings);
        }

        /// <summary>
        /// Метод сохраняет прогноз погоды в хранилище.
        /// </summary>
        /// <param name="forecastDetailed">Объект, содержащий информацию о погоде для выбранных городов</param>
        public override void SaveForecasrDetailed(ForecastDetailed forecastDetailed)
        {
            WriteForecastDetailedToXml(forecastDetailed);
        }

        #endregion

        #region public Load
        /// <summary>
        /// Метод загружает хранящийся прогноз погоды из хранилища.
        /// </summary>
        /// <returns>Объект, содержащий информацию о погоде для выбранных городов</returns>
        public override ForecastDetailed LoadForecastDetailed()
        {
            var forecastDetailed = ReadForecastDetailedFromXml();

            return forecastDetailed;
        }

        /// <summary>
        /// Метод загржуает настройки из хранилища.
        /// </summary>
        /// <returns>Объект, содержащий загруженные настройки.</returns>
        public override UserSettings LoadSettings()
        {
            var settings = ReadSettingsFromXml();

            return settings;
        }

        #endregion

        #region private WriteToXml
        /// <summary>
        /// Метод сохраняет настройки в хранилище.
        /// </summary>
        /// <param name="settings">Объект, содержащий сохраняемые настройки</param>
        private static void WriteSettingsToXml(UserSettings settings)
        {
            try
            {
                if (!Directory.Exists(XmlSettingsDirName))
                {
                    Directory.CreateDirectory(XmlSettingsDirName);
                }

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
                //throw new Exception();
                throw new Exception(LanguageDictionary.Current.Translate<string>("writeXml_SttsHandler", "Content"));
                //MessageBox.Show("Непредвиденная ошибка. Не удалось сохранить настройки. Текст ошибки: " + ex.Message);
            }
        }

        /// <summary>
        /// Метод сохраняет прогноз погоды в хранилище.
        /// </summary>
        /// <param name="forecastDetailed">Объект, содержащий информацию о погоде для выбранных городов</param>
        private static void WriteForecastDetailedToXml(ForecastDetailed forecastDetailed)
        {
            try
            {
                if (!Directory.Exists(XmlForecastDetailedDirName))
                {
                    Directory.CreateDirectory(XmlForecastDetailedDirName);
                }

                var ser = new XmlSerializer(typeof(ForecastDetailed));
                using (TextWriter writer = new StreamWriter(XmlForecastDetailedFileName))
                {
                    ser.Serialize(writer, forecastDetailed);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WriteForecastDetailed.WriteXml(): " + LanguageDictionary.Current.Translate<string>("writeForecastToXml_SttsHandler", "Content") + ex.Message);
                //throw new Exception();
                throw new Exception(LanguageDictionary.Current.Translate<string>("writeForecastToXml_SttsHandler", "Content"));
                //MessageBox.Show("Непредвиденная ошибка. Не удалось сохранить настройки. Текст ошибки: " + ex.Message);
            }
        }

        #endregion
        
        #region private ReadFromXml

        /// <summary>
        /// Метод загржуает настройки из хранилища.
        /// </summary>
        /// <returns>Объект, содержащий загруженные настройки.</returns>
        private static UserSettings ReadSettingsFromXml()
        {
            try
            {
                if (!Directory.Exists(XmlSettingsDirName))
                {
                    return UserSettings.GetDefaultSettings();
                }
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

        /// <summary>
        /// Метод загружает хранящийся прогноз погоды из хранилища.
        /// </summary>
        /// <returns>Объект, содержащий информацию о погоде для выбранных городов</returns>
        private static ForecastDetailed ReadForecastDetailedFromXml()
        {
            try
            {
                if (!Directory.Exists(XmlForecastDetailedDirName))
                {
                    return null;
                }
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
                Debug.WriteLine("XmlDataHandler.ReadForecastDetailedFromXml(): " + LanguageDictionary.Current.Translate<string>("readForecastFromXml_SttsHandler", "Content") + ex.Message);
                //throw new Exception(LanguageDictionary.Current.Translate<string>("readXml_SttsHandler", "Content"));
                return null;
            }
        }

        #endregion
    }
}
