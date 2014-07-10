using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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
        public static IDataHandler dataHandler;// = LoadDataHandler.GetInstanceSettingsHandler();
        public static UserSettings settings;// = settingHandler.LoadSettings();

        #region Для расширяемости с помощью инструмента MEF
        [ImportMany]
        protected IDataHandler[] DataHandlers { get; set; }

        private void Compose()
        {
            try
            {
                var catalog = new DirectoryCatalog(Environment.CurrentDirectory);
                var container = new CompositionContainer(catalog);
                container.ComposeParts(this);
            }
            catch (Exception ex)
            {
                var message = "Ошибка загрузки обработчика хранения настроек и прогноза погоды! Обработчик отсутствует или поврежден. \r\n Error loading handler store settings and the weather forecast! Handler is missing or damaged.";
                MessageBox.Show(message);
                throw new Exception();
            }

        }
        #endregion
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                #region Для расширяемости с помощью инструмента MEF
                Compose();
                if (DataHandlers == null || DataHandlers.Length == 0)
                {
                    var message = "Не удалось найти ни одного обработчика! Unable to find a single handler!";
                    MessageBox.Show(message);
                    throw new Exception();
                }
                dataHandler = DataHandlers[0];
                settings = dataHandler.LoadSettings();
                #endregion

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
