using System.Globalization;
using System.IO;
using System.Reflection;
using SettingsHandlerInterface;
using SettingsHandlerInterface.Classes;
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
        public static ISettingsHandler settingHandler = LoadSettingsHandler.GetInstanceSettingsHandler();
        public static Settings settings = settingHandler.LoadSettings();

        protected override void OnStartup(StartupEventArgs e)
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
    }
}
