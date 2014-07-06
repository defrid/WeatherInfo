using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;
using SettingsHandlerInterface;
using SettingsHandlerInterface.Classes;
using System.Reflection;

namespace XMLSettingsHandler
{
    public class XMLSettingsHandler : SettingsHandler
    {
        /// <summary>
        /// Хранит путь к файлу настроек
        /// </summary>
        public static String XMLFileName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Config\settings.xml";

        /// <summary>
        /// Сохраняет настройки в XML-файл.
        /// </summary>
        /// <param name="settings">Объект, содержащий настройки приложения</param>
        public override void SaveSettings(Settings settings)
        {
            WriteXml(settings);
        }

        /// <summary>
        /// Загружает настрйки из XML-файла и возвращает объект их содержащий.
        /// </summary>
        /// <returns></returns>
        public override Settings LoadSettings()
        {
            var settings = ReadXml();

            return settings;
        }

        /// <summary>
        /// Сохраняет настройки в XML-файл.
        /// </summary>
        /// <param name="settings">Объект, содержащий настройки приложения</param>
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
                //MessageBox.Show("Непредвиденная ошибка. Не удалось сохранить настройки. Текст ошибки: " + ex.Message);
            }
        }

        /// <summary>
        /// Загружает настрйки из XML-файла и возвращает объект их содержащий.
        /// </summary>
        /// <returns></returns>
        private static Settings ReadXml()
        {
            try
            {
                //WriteXml(Settings.GetDefaultSettings());
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
                //MessageBox.Show("Непредвиденная ошибка. Будут загружены настройки по-умолчанию, если это возможно. Текст ошибки: " + ex.Message);
            }
            return Settings.GetDefaultSettings();
        }
    }
}
