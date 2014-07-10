using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataHandlerInterface.Classes;
using Tomers.WPF.Localization;
using WeatherInfo.Classes;
using System.Windows.Threading;
using Microsoft.Win32;
using IWshRuntimeLibrary;
using System.Text.RegularExpressions;

namespace WeatherInfo
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        MainWindow main;

        public SettingsWindow(object e)
        {
            main = (MainWindow)e;
            InitializeComponent();

            try //ибо тут падало (кстати у меня в трее тоже в такой строке падало)
            {
                //Icon = MainWindow.ConvertBitmabToImage(Properties.Resources.settingsGear.ToBitmap());
            }
            catch { }

            ChoosenCities = new List<CitySettings>();
            updatePeriod_save = 10;
            format_save = Options.GetValueByAttribute(Options.FormatForecast.Short.ToString(), App.settings.language.engName);
            autostart_save = true;
            temperatureUnits_save = new TemperatureUnits("Цельсии", "Celsius");
            language_save = new Language("Русский", "Russian");
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void about_btn_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new About();
            aboutWindow.Show();
            aboutWindow.Focus();
            aboutWindow.Activate();
        }

        public List<CitySettings> ChoosenCities;
        public int updatePeriod_save;
        public string format_save;
        public bool autostart_save;
        public TemperatureUnits temperatureUnits_save;
        public Language language_save;

        private void accept_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                updatePeriod_save = Convert.ToInt32(updatePeriod_slider.Value);
                format_save = ((ComboBoxItem)listOfFormatsForecast_cbx.SelectedItem).Tag.ToString();//Options.GetValueByAttribute(listOfFormatsForecast_cbx.SelectedItem.ToString());
                if (autostartFlag_chbx.IsChecked != null) autostart_save = (bool)autostartFlag_chbx.IsChecked;
                temperatureUnits_save = (TemperatureUnits)((ComboBoxItem)listOfTemperatureUnits_cbx.SelectedItem).Tag;
                language_save = (Language)((ComboBoxItem)listOfLanguages_cbx.SelectedItem).Tag;

                //App.settings = new Settings(countryId_save, countryRusName_save, countryEngName_save, regionId_save, regionName_save, cityYaId_save, cityOWId_save, cityRusName_save, cityEngName_save, format_save, updatePeriod_save, autostart_save, temperatureUnits_save, language_save);
                App.settings = new UserSettings(ChoosenCities, format_save, updatePeriod_save, autostart_save, temperatureUnits_save, language_save);

                App.settingHandler.SaveSettings(App.settings);                

                Autorun();
                ChangeLocalization();

                main.applySettings();
                Close();
            }
            catch (Exception ex)
            {
                var message = LanguageDictionary.Current.Translate<string>("messCheckDataStts", "Content");
                MessageBox.Show(message);
            }
        }

        void ChangeLocalization()
        {
            //if (LanguageContext.Instance.Culture.Equals(CultureInfo.GetCultureInfo("ru-RU")))
            LanguageContext.Instance.Culture = CultureInfo.GetCultureInfo(language_save.engName == "English" ? "en-US" : "ru-RU");
        }

        #region Автозагрузка
        /// <summary>
        /// Метод утановки автозагрузки приложения.
        /// Добавляет запись в реестр текущего пользователя и ярлык в папку автозагрузки.
        /// </summary>
        private void Autorun()
        {
            try
            {
                var path = string.Concat(Environment.CurrentDirectory, @"\WeatherInfo.exe");
                var pathToLnk = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\WeatherInfo.lnk";



                var key =
                        Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\");
                //Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
                if (key == null) return;

                if (autostartFlag_chbx.IsChecked != null && (bool)autostartFlag_chbx.IsChecked)
                {
                    if (CreateShortCut(path, Environment.CurrentDirectory, pathToLnk))
                    {
                        key.SetValue("WeatherInfo", pathToLnk);
                        //key.SetValue("WeatherInfo", "\\WeatherInfo.exe");
                    }
                }
                else
                {
                    if (DeleteShortCut(pathToLnk))
                    {
                        key.DeleteValue("WeatherInfo", false);
                    }
                }
                
                key.Flush();
                key.Close();
            }
            catch (Exception ex)
            {
                var message = LanguageDictionary.Current.Translate<string>("errorAddInAutostartStts", "Content");
                MessageBox.Show(message);
            }
                     
        }

        /// <summary>
        /// Создание ярлыка в папке автозагрузки
        /// </summary>
        /// <param name="filePath">Путь к исполняемому файлу программы</param>
        /// <param name="workDir">Рабочая директория приложения</param>
        /// <param name="shortcutPath">Путь к создаваемому ярлыку</param>
        /// <returns>Флаг успешности операции</returns>
        private static bool CreateShortCut(string filePath, string workDir, string shortcutPath)
        {
            try
            {
                var shell = new WshShell();

                var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.Description = "Запуск";
                shortcut.TargetPath = filePath;
                shortcut.WorkingDirectory = workDir;
                shortcut.Save();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageDictionary.Current.Translate<string>("errorAddInAutostartStts", "Content"));
                return false;
            }            
        }

        /// <summary>
        /// Удаляет ярлык из папки автозагрузки
        /// </summary>
        /// <param name="shortcutPath">Путь к удаляемому ярлыку приложения в папке автозагрузки</param>
        /// <returns>Флаг успешности операции</returns>
        private static bool DeleteShortCut(string shortcutPath)
        {
            try
            {
                if (System.IO.File.Exists(shortcutPath))
                {
                    System.IO.File.Delete(shortcutPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageDictionary.Current.Translate<string>("errorRemoveFromAutostartStts", "Content"));
                return false;
            }            
        }

        private void autostartFlag_chbx_Checked(object sender, RoutedEventArgs e)
        {
            if (autostartFlag_chbx.IsChecked != null) autostart_save = (bool)autostartFlag_chbx.IsChecked;//true;
            //MessageBox.Show("Автозапуск");
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadCountries();
                listOfCountries_cbx.SelectedIndex = GetIndCurCountry();
                listOfCountries_cbx.SelectionChanged += listOfCountries_cbx_SelectionChanged;
            } catch (Exception ex) 
            {
                var message = LanguageDictionary.Current.Translate<string>("errorLoadCountriesStts", "Content");
                MessageBox.Show(message);
            }

            try
            {
                LoadCities();
                listOfCitiies_cbx.SelectedIndex = GetIndCurCity();
                listOfCitiies_cbx.SelectionChanged += listOfCitiies_cbx_SelectionChanged;
            }
            catch (Exception ex)
            {
                var message = LanguageDictionary.Current.Translate<string>("errorLoadCitiesStts", "Content");
                MessageBox.Show(message);
            }

            try
            {
                LoadChoosenCities();
                ChoosenCitiesComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                var message = LanguageDictionary.Current.Translate<string>("errorLoadChoosenCitiesStts", "Content");
                MessageBox.Show(message);
            }

            try
            {
                LoadFormats();
                listOfFormatsForecast_cbx.SelectedIndex = GetIndCurFormat();
                listOfFormatsForecast_cbx.SelectionChanged += listOfFormatsForecast_cbx_SelectionChanged;
            }
            catch (Exception ex)
            {
                var message = LanguageDictionary.Current.Translate<string>("errorLoadFormatsStts", "Content");
                MessageBox.Show(message);
            }

            try
            {
                LoadTemperatureUnits();
                listOfTemperatureUnits_cbx.SelectedIndex = GetIndCurTemperatureUnit();
            }
            catch (Exception ex)
            {
                var message = LanguageDictionary.Current.Translate<string>("errorLoadTemperatureUnitsStts", "Content");
                MessageBox.Show(message);
            }
            

            try
            {
                LoadLanguages();
                listOfLanguages_cbx.SelectedIndex = GetIndCurLanguage();
            }
            catch (Exception ex)
            {
                var message = LanguageDictionary.Current.Translate<string>("errorLoadLanguagesStts", "Content");
                MessageBox.Show(message);
            }

            try
            {
                updatePeriod_slider.Value = Convert.ToDouble(App.settings.updatePeriod);
                autostartFlag_chbx.IsChecked = App.settings.autostart;
            }
            catch (Exception ex)
            {
                var message = LanguageDictionary.Current.Translate<string>("errorLoadOtherStts", "Content");
                MessageBox.Show(message);
            }            
        }

        /// <summary>
        /// Подгружает список стран
        /// </summary>
        void LoadCountries()
        {
            List<string> countries = App.settings.language.engName == "English" ? getCity.getCountryNames(false) : getCity.getCountryNames(true);
            countries = countries.OrderBy(item => item).ToList();

            foreach (var country in countries)
            {
                //var item = new ComboBoxItem();
                //item.Tag = App.settings.language.engName == "English" ? getCity.countryTranslate(country, false) : country;//country;getCity.countryTranslate(curCity, true)
                //item.Content = country;//App.settings.language.engName == "English" ? country : getCity.countryTranslate(country, false);
                listOfCountries_cbx.Items.Add(country);
            }
            //listOfCountries_cbx.ItemsSource = countries;
        }

        int GetIndCurCountry()
        {
            var ind = 0;
            var items = listOfCountries_cbx.Items;
            var countryName = App.settings.language.engName == "English" ? App.settings.cities[0].country.countryEngName : App.settings.cities[0].country.countryRusName;
            foreach (var item in items)
            {
                var tmpCountry = item.ToString();
                //if ((string)((ComboBoxItem)item).Tag == countryName)
                if (tmpCountry == countryName)
                {
                    ind = listOfCountries_cbx.Items.IndexOf(item);
                    break;
                }
            }

            return ind;
        }

        /// <summary>
        /// Подгружает список городов для выбранной страны
        /// </summary>
        void LoadCities()
        {
            listOfCitiies_cbx.Items.Clear();
            var country = App.settings.language.engName == "English" ? getCity.countryTranslate(listOfCountries_cbx.SelectedItem.ToString(), false) : listOfCountries_cbx.SelectedItem.ToString();//(string)((ComboBoxItem)listOfCountries_cbx.SelectedItem).Tag;
            
            var allCities = App.settings.language.engName == "English" ? getCity.getCities(country, false) : getCity.getCities(country, true);
            allCities = allCities.OrderBy(item => item).ToList();
            //listOfCitiies_cbx.ItemsSource = allCities;
            foreach (var city in allCities)
            {
                var curCity = App.settings.language.engName == "English" ? UpperEngCityName(getCity.cityTranslate(city, false, country)) : getCity.cityTranslate(city, true, country);

                var item = new ComboBoxItem();
                item.Tag = curCity;
                item.Content = App.settings.language.engName == "English" ? UpperEngCityName(city) : city;
                listOfCitiies_cbx.Items.Add(item);
            }
        }

        public static string UpperEngCityName(string city)
        {
            var curCity = city;
            if (App.settings.language.engName != "English")
            {
                return curCity;
            }

            var firstLit = curCity[0].ToString();
            var reg = new Regex(curCity[0].ToString());
            curCity = reg.Replace(curCity, firstLit.ToUpper(), 1);

            return curCity;
        }

        int GetIndCurCity()
        {
            var ind = 0;
            var items = listOfCitiies_cbx.Items;
            foreach (var item in items)
            {
                if ((string)((ComboBoxItem)item).Tag == App.settings.cities[0].city.cityRusName)
                {
                    ind = listOfCitiies_cbx.Items.IndexOf(item);
                    break;
                }
            }

            return ind;
        }

        /// <summary>
        /// Подгружает список выбранных для отображения городов
        /// </summary>
        void LoadChoosenCities()
        {
            ChoosenCities = new List<CitySettings>(App.settings.cities);
            foreach (var cityStts in ChoosenCities)
            {
                var item = new ComboBoxItem
                {
                    Tag = cityStts,
                    Content =
                        App.settings.language.engName == "English"
                            ? UpperEngCityName(cityStts.city.cityEngName)
                            : cityStts.city.cityRusName
                };

                ChoosenCitiesComboBox.Items.Add(item);//(city.ToString());
                //ChoosenCitiesComboBox.Items.GetItemAt(ChoosenCitiesComboBox.Items.Count - 1).Tag = city;                
            }
        }

        /// <summary>
        /// Подгружает список форматов отображения
        /// </summary>
        void LoadFormats()
        {
            var formats = Enum.GetNames(typeof(Options.FormatForecast));
            foreach (var format in formats)
            {
                var value = Options.GetFormatAttribute(format, App.settings.language.engName);

                var item = new ComboBoxItem
                {
                    Tag = format,
                    Content = App.settings.language.engName == "English" ? format : value
                };

                listOfFormatsForecast_cbx.Items.Add(item);
            }
        }

        int GetIndCurFormat()
        {
            var ind = 0;
            var items = listOfFormatsForecast_cbx.Items;
            foreach (var item in items)
            {
                if (((ComboBoxItem)item).Tag.ToString() == App.settings.format)
                {
                    ind = listOfFormatsForecast_cbx.Items.IndexOf(item);
                    break;
                }
            }

            return ind;
        }

        /// <summary>
        /// Подгружает список единиц измерения температуры
        /// </summary>
        void LoadTemperatureUnits()
        {
            var temperatureUnits = Options.GetTemperatureUnits();
            foreach (var tempUn in temperatureUnits)
            {
                var item = new ComboBoxItem();
                item.Tag = tempUn;
                item.Content = App.settings.language.engName == "English" ? tempUn.engName : tempUn.rusName;

                listOfTemperatureUnits_cbx.Items.Add(item);
            }
        }

        int GetIndCurTemperatureUnit()
        {
            var ind = 0;
            var items = listOfTemperatureUnits_cbx.Items;
            var t = App.settings.temperatureUnits;
            foreach (var item in items)
            {
                if (((TemperatureUnits)(((ComboBoxItem)item).Tag)).engName == App.settings.temperatureUnits.engName)
                {
                    ind = listOfTemperatureUnits_cbx.Items.IndexOf(item);
                    break;
                }
            }

            return ind;
        }

        /// <summary>
        /// Подгружает список языков локализации программы
        /// </summary>
        void LoadLanguages()
        {
            var languages = Options.GetLanguages();
            foreach (var lang in languages)
            {
                var item = new ComboBoxItem
                {
                    Tag = lang,
                    Content = App.settings.language.engName == "English" ? lang.engName : lang.rusName
                };

                listOfLanguages_cbx.Items.Add(item);
            }
        }

        int GetIndCurLanguage()
        {
            var ind = 0;
            var items = listOfLanguages_cbx.Items;
            foreach (var item in items)
            {
                if (((Language)(((ComboBoxItem)item).Tag)).Equals(App.settings.language))
                {
                    ind = listOfLanguages_cbx.Items.IndexOf(item);
                    break;
                }
            }

            return ind;
        }

        private void listOfCountries_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CheckInputCountry())
                {
                    LoadCities();
                    listOfCitiies_cbx.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                var message = LanguageDictionary.Current.Translate<string>("errorLoadCitiesStts", "Content");
                MessageBox.Show(message);
            }
        }

        private void listOfCitiies_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          
        }

        private void listOfFormatsForecast_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //format_save = Options.GetValueByAttribute(listOfFormatsForecast_cbx.SelectedItem.ToString());
        }

        private void updatePeriod_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        /// <summary>
        /// Добавить город в список выбранных
        /// </summary>
        private void AddCityToChoosen()
        {
            var curCountryRus = App.settings.language.engName == "English" ? getCity.countryTranslate(listOfCountries_cbx.SelectedItem.ToString(), false) : listOfCountries_cbx.SelectedItem.ToString();
            var curCountryEng = App.settings.language.engName == "English" ? listOfCountries_cbx.SelectedItem.ToString() : getCity.countryTranslate(listOfCountries_cbx.SelectedItem.ToString(), true);
            var curCityRus = App.settings.language.engName == "English" ? (string)((ComboBoxItem)listOfCitiies_cbx.SelectedItem).Tag : ((ComboBoxItem)listOfCitiies_cbx.SelectedItem).Content.ToString();
            var curCityEng = App.settings.language.engName == "English" ? ((ComboBoxItem)listOfCitiies_cbx.SelectedItem).Content.ToString() : (string)((ComboBoxItem)listOfCitiies_cbx.SelectedItem).Tag;
            //var curCountryRus = App.settings.language.engName == "English" ? (string)((ComboBoxItem)listOfCountries_cbx.SelectedItem).Tag : listOfCountries_cbx.SelectedItem.ToString();
            //var curCountryEng = App.settings.language.engName == "English" ? listOfCountries_cbx.SelectedItem.ToString() : listOfCountries_cbx.SelectedItem.ToString();

            var newCity = new CitySettings
            {
                country = new Country
                {
                    countryId = "RU",
                    countryRusName = curCountryRus,
                    countryEngName = curCountryEng
                },
                region = new RegionOfCity
                {
                    regionId = 0,
                    regionName = "Region"
                },
                city = new City
                {
                    cityYaId = getCity.getCityId(curCityRus, true, true, curCountryRus),
                    cityOWId = getCity.getCityId(curCityRus, true, false, curCountryRus),
                    cityRusName = curCityRus,//curCity,
                    cityEngName = UpperEngCityName(curCityEng)
                }
            };

            var repeateSearch = ChoosenCities.SingleOrDefault(el => (el.city.cityYaId == newCity.city.cityYaId &&
                                                               el.city.cityRusName == newCity.city.cityRusName &&
                                                               el.country.countryId == newCity.country.countryId &&
                                                               el.country.countryRusName == newCity.country.countryRusName));
            if (repeateSearch != null)
                return;
            ChoosenCities.Add(newCity);
            var item = new ComboBoxItem
            {
                Tag = newCity,
                Content = App.settings.language.engName == "English" ? newCity.city.cityEngName : newCity.city.cityRusName
            };

            //ChoosenCitiesComboBox.Items.Add(newCity.ToString());
            //ChoosenCitiesComboBox.Tag = newCity;
            ChoosenCitiesComboBox.Items.Add(item);
            ChoosenCitiesComboBox.SelectedIndex = ChoosenCitiesComboBox.Items.Count - 1;
        }

        /// <summary>
        /// Убирает город из списка выбранных
        /// </summary>
        private void RemoveCityFromChoosen()
        {
            var comboBoxItem = ChoosenCitiesComboBox.SelectedItem as ComboBoxItem;
            if (comboBoxItem != null)
            {
                var countryCity = comboBoxItem.Tag as CitySettings;
                ChoosenCities.Remove(
                    ChoosenCities.SingleOrDefault(el => countryCity != null && (el.city.cityRusName == countryCity.city.cityRusName && el.country.countryRusName == countryCity.country.countryRusName)));
            }
            ChoosenCitiesComboBox.Items.RemoveAt(ChoosenCitiesComboBox.SelectedIndex);            
        }

        private bool CheckInputCountry()
        {
            if (listOfCountries_cbx.SelectedItem == null || string.IsNullOrWhiteSpace(listOfCountries_cbx.SelectedItem.ToString()/*(string)((ComboBoxItem)listOfCountries_cbx.SelectedItem).Tag)*/))
            {
                var message = LanguageDictionary.Current.Translate<string>("incorrectCountryStts", "Content");
                MessageBox.Show(message);
                return false;
                //throw new Exception(LanguageDictionary.Current.Translate<string>("incorrectCountryStts", "Content"));
            }

            return true;
        }

        private bool CheckInputCity()
        {
            if (listOfCitiies_cbx.SelectedItem == null || string.IsNullOrWhiteSpace((string)((ComboBoxItem)listOfCitiies_cbx.SelectedItem).Tag))
            {
                var message = LanguageDictionary.Current.Translate<string>("incorrectCityStts", "Content");
                MessageBox.Show(message);
                return false;
                //throw new Exception(LanguageDictionary.Current.Translate<string>("incorrectCityStts", "Content"));
            }

            return true;
        }

        private bool CheckInputData()
        {
            return (CheckInputCountry() && CheckInputCity());
        }

        private void AddCityButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckInputData())
                {
                    AddCityToChoosen();
                }
            }
            catch (Exception ex)
            {
                var message = LanguageDictionary.Current.Translate<string>("messCheckDataStts", "Content");
                MessageBox.Show(message);
            }  
        }

        private void DeleteCityButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                RemoveCityFromChoosen();
                ChoosenCitiesComboBox.SelectedIndex = ChoosenCitiesComboBox.Items.Count - 1;
            }
            catch (Exception ex)
            {
                var message = LanguageDictionary.Current.Translate<string>("errorDelCityStts", "Content");
                MessageBox.Show(message);
            }
        }

        private void LockUnlockButtons(object sender, SelectionChangedEventArgs e)
        {
            var comboItemsCount = ChoosenCitiesComboBox.Items.Count;
            AddButton.IsEnabled = comboItemsCount < 10;

            DeleteButton.IsEnabled = comboItemsCount > 1;
            ReplaceButton.IsEnabled = !(comboItemsCount > 1);
            if (comboItemsCount > 1)
            {
                DeleteButton.Visibility = Visibility.Visible;
                ReplaceButton.Visibility = Visibility.Hidden;
            }
            else
            {
                DeleteButton.Visibility = Visibility.Hidden;
                ReplaceButton.Visibility = Visibility.Visible;
            }            
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckInputData())
                {
                    RemoveCityFromChoosen();
                    AddCityToChoosen();
                }
            }
            catch (Exception ex)
            {
                var message = LanguageDictionary.Current.Translate<string>("messCheckDataStts", "Content");
                MessageBox.Show(message);
            }            
        }
    }
}
