using System.Globalization;
using System.IO;
using System.Reflection;
using DataHandlerInterface.Classes;
using DataHandlerInterface.Interfaces;
using DataHandlerInterface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Tomers.WPF.Localization;
using WeatherInfo.Classes;
using WeatherInfo.Interfaces;

namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IDataHandler settingHandler = LoadDataHandler.GetInstanceSettingsHandler();
        public static UserSettings settings = settingHandler.LoadSettings();

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                LanguageDictionary.RegisterDictionary(
                CultureInfo.GetCultureInfo("en-US"),
                new XmlLanguageDictionary(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Languages\en-US.xml"));

                LanguageDictionary.RegisterDictionary(
                    CultureInfo.GetCultureInfo("ru-RU"),
                    new XmlLanguageDictionary(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Languages\ru-RU.xml"));

                LanguageContext.Instance.Culture = CultureInfo.GetCultureInfo(settings.language.engName == "English" ? "en-US" : "ru-RU");

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                var message = "Ошибка приложения. Приложение будет завершено.\r\nApplication Error. The application will be completed.";
                MessageBox.Show(message);
                Environment.Exit(1);
            }
        }
    }
}
