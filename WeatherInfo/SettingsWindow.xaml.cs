using System.Globalization;
using SettingsHandlerInterface.Classes;
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
            ChoosenCities = new List<CitySettings>();
            updatePeriod_save = 10;
            format_save = Options.GetValueByAttribute(Options.FormatForecast.Days.ToString());
            autostart_save = true;
            temperatureUnits_save = new TemperatureUnits("Цельсии", "Celsius");
            language_save = new Language("Русский", "Russian");
        }

        getCity gC = new getCity();
        string translatePath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Location\translit.txt";

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void about_btn_Click(object sender, RoutedEventArgs e)
        {
            About aboutWindow = new About();
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
                autostart_save = (bool)autostartFlag_chbx.IsChecked;
                temperatureUnits_save = (TemperatureUnits)((ComboBoxItem)listOfTemperatureUnits_cbx.SelectedItem).Tag;
                language_save = (Language)((ComboBoxItem)listOfLanguages_cbx.SelectedItem).Tag;

                //App.settings = new Settings(countryId_save, countryRusName_save, countryEngName_save, regionId_save, regionName_save, cityYaId_save, cityOWId_save, cityRusName_save, cityEngName_save, format_save, updatePeriod_save, autostart_save, temperatureUnits_save, language_save);
                App.settings = new Settings(ChoosenCities, format_save, updatePeriod_save, autostart_save, temperatureUnits_save, language_save);

                App.settingHandler.SaveSettings(App.settings);

                main.applySettings();

                Autorun();
                ChangeLocalization();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проверьте правильность введенных данных. Ошибка: " + ex.Message);
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
            var path = string.Concat(Environment.CurrentDirectory, @"\WeatherInfo.exe");
            var pathToLnk = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\WeatherInfo.lnk";

            

            RegistryKey key =
                    Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            //Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);

            if ((bool)autostartFlag_chbx.IsChecked)
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

        /// <summary>
        /// Создание ярлыка в папке автозагрузки
        /// </summary>
        /// <param name="FilePath">Путь к исполняемому файлу программы</param>
        /// <param name="WorkDir">Рабочая директория приложения</param>
        /// <param name="shortcutPath">Путь к создаваемому ярлыку</param>
        /// <returns>Флаг успешности операции</returns>
        private bool CreateShortCut(string FilePath, string WorkDir, string shortcutPath)
        {
            try
            {
                WshShell shell = new WshShell();

                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.Description = "Запуск";
                shortcut.TargetPath = FilePath;
                shortcut.WorkingDirectory = WorkDir;
                shortcut.Save();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добвления программы в автозагрузку! Ошибка: " + ex.Message);
                return false;
            }            
        }

        /// <summary>
        /// Удаляет ярлык из папки автозагрузки
        /// </summary>
        /// <param name="shortcutPath">Путь к удаляемому ярлыку приложения в папке автозагрузки</param>
        /// <returns>Флаг успешности операции</returns>
        private bool DeleteShortCut(string shortcutPath)
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
                MessageBox.Show("Ошибка удаления программы из автозагрузки! Ошибка: " + ex.Message);
                return false;
            }            
        }

        private void autostartFlag_chbx_Checked(object sender, RoutedEventArgs e)
        {
            autostart_save = (bool)autostartFlag_chbx.IsChecked;//true;
            //MessageBox.Show("Автозапуск");
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var selectedItemCountry = new ComboBoxItem();
            //selectedItemCountry.Tag = App.settings.cities[0].country.countryRusName;
            //selectedItemCountry.Content = App.settings.language.engName == "English" ? translate.toEng((string)selectedItemCountry.Tag, translatePath) : (string)selectedItemCountry.Tag;

            LoadCountries();
            listOfCountries_cbx.SelectedIndex = GetIndCurCountry();
            listOfCountries_cbx.SelectionChanged += listOfCountries_cbx_SelectionChanged;

            LoadCities();
            listOfCitiies_cbx.SelectedIndex = GetIndCurCity();//SelectedItem = App.settings.cities[0].city.cityRusName;
            listOfCitiies_cbx.SelectionChanged += listOfCitiies_cbx_SelectionChanged;

            LoadChoosenCities();
            ChoosenCitiesComboBox.SelectedIndex = 0;

            updatePeriod_slider.Value = Convert.ToDouble(App.settings.updatePeriod);

            LoadFormats();
            listOfFormatsForecast_cbx.SelectedIndex = GetIndCurFormat();//Options.GetFormatAttribute(App.settings.format);
            listOfFormatsForecast_cbx.SelectionChanged += listOfFormatsForecast_cbx_SelectionChanged;

            LoadTemperatureUnits();
            listOfTemperatureUnits_cbx.SelectedIndex = GetIndCurTemperatureUnit();//0;

            LoadLanguages();
            listOfLanguages_cbx.SelectedIndex = GetIndCurLanguage();//0;

            autostartFlag_chbx.IsChecked = App.settings.autostart;
        }

        /// <summary>
        /// Подгружает список стран
        /// </summary>
        void LoadCountries()
        {
            List<string> countries = getCity.getCountryNames();
            foreach (var country in countries)
            {
                var item = new ComboBoxItem();
                item.Tag = country;
                item.Content = App.settings.language.engName == "English" ? translate.toEng(country, translatePath) : country;
                listOfCountries_cbx.Items.Add(item);
            }
            //listOfCountries_cbx.ItemsSource = countries;
        }

        int GetIndCurCountry()
        {
            var ind = 0;
            var items = listOfCountries_cbx.Items;
            foreach (var item in items)
            {
                if ((string)((ComboBoxItem)item).Tag == App.settings.cities[0].country.countryRusName)
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
        /// <param name="country"></param>
        void LoadCities()
        {
            var country = (string)((ComboBoxItem)listOfCountries_cbx.SelectedItem).Tag;
            List<string> allCities = getCity.getCities(country, true);
            //listOfCitiies_cbx.ItemsSource = allCities;
            foreach (var city in allCities)
            {
                var curCity = getCity.cityTranslate(city, true, country);

                var item = new ComboBoxItem();
                item.Tag = city;
                item.Content = App.settings.language.engName == "English" ? upperEngCityName(curCity) : city;
                listOfCitiies_cbx.Items.Add(item);
            }
        }

        string upperEngCityName(string city)
        {
            var curCity = city;
            if (App.settings.language.engName == "English")
            {
                var firstLit = curCity[0].ToString();
                var reg = new Regex(curCity[0].ToString());
                curCity = reg.Replace(curCity, firstLit.ToUpper(), 1);
            }

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
                var item = new ComboBoxItem();
                item.Tag = cityStts;
                item.Content = App.settings.language.engName == "English" ? upperEngCityName(cityStts.city.cityEngName) : cityStts.city.cityRusName;//city.ToString();

                ChoosenCitiesComboBox.Items.Add(item);//(city.ToString());
                //ChoosenCitiesComboBox.Items.GetItemAt(ChoosenCitiesComboBox.Items.Count - 1).Tag = city;                
            }
        }

        /// <summary>
        /// Подгружает список форматов отображения
        /// </summary>
        void LoadFormats()
        {
            string[] formats = Enum.GetNames(typeof(Options.FormatForecast));
            foreach (var f in formats)
            {
                var value = Options.GetFormatAttribute(f);

                var item = new ComboBoxItem();
                item.Tag = f;
                item.Content = App.settings.language.engName == "English" ? f.ToString() : value;

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
            TemperatureUnits t = App.settings.temperatureUnits;
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
                var item = new ComboBoxItem();
                item.Tag = lang;
                item.Content = App.settings.language.engName == "English" ? lang.engName : lang.rusName;

                listOfLanguages_cbx.Items.Add(item);
            }
        }

        int GetIndCurLanguage()
        {
            var ind = 0;
            var items = listOfLanguages_cbx.Items;
            foreach (var item in items)
            {
                if ((Language)(((ComboBoxItem)item).Tag) == App.settings.language)
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
                LoadCities();
                listOfCitiies_cbx.SelectedIndex = 0;
            }
            catch { }
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
            var curCity = (string)((ComboBoxItem)listOfCitiies_cbx.SelectedItem).Tag;
            var curCountry = (string)((ComboBoxItem)listOfCountries_cbx.SelectedItem).Tag; //listOfCountries_cbx.SelectedItem.ToString();            

            var newCity = new CitySettings
            {
                country = new Country
                {
                    countryId = "RU",
                    countryRusName = curCountry,
                    countryEngName = translate.toEng(curCountry, translatePath)
                },
                region = new RegionOfCity
                {
                    regionId = 0,
                    regionName = "Region"
                },
                city = new City
                {
                    cityYaId = getCity.getCityId(curCity, true, true, curCountry),
                    cityOWId = getCity.getCityId(curCity, true, false, curCountry),
                    cityRusName = curCity,
                    cityEngName = upperEngCityName(getCity.cityTranslate(curCity, true, curCountry))
                }
            };

            var repeateSearch = ChoosenCities.SingleOrDefault(el => (el.city.cityYaId == newCity.city.cityYaId &&
                                                               el.city.cityRusName == newCity.city.cityRusName &&
                                                               el.country.countryId == newCity.country.countryId &&
                                                               el.country.countryRusName == newCity.country.countryRusName));
            if (repeateSearch != null)
                return;
            ChoosenCities.Add(newCity);
            var item = new ComboBoxItem();
            item.Tag = newCity;
            item.Content = App.settings.language.engName == "English" ? newCity.city.cityEngName : newCity.city.cityRusName; // newCity.ToString();

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
            var countryCity = (ChoosenCitiesComboBox.SelectedItem as ComboBoxItem).Tag as CitySettings;
            ChoosenCities.Remove(
                ChoosenCities.SingleOrDefault(el => (el.city.cityRusName == countryCity.city.cityRusName && el.country.countryRusName == countryCity.country.countryRusName)));
            ChoosenCitiesComboBox.Items.RemoveAt(ChoosenCitiesComboBox.SelectedIndex);            
        }

        private void AddCityButtonClick(object sender, RoutedEventArgs e)
        {
            AddCityToChoosen();
        }

        private void DeleteCityButtonClick(object sender, RoutedEventArgs e)
        {
            RemoveCityFromChoosen();
            ChoosenCitiesComboBox.SelectedIndex = ChoosenCitiesComboBox.Items.Count - 1;
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
            RemoveCityFromChoosen();
            AddCityToChoosen();
        }
    }
}
