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
using WeatherInfo.Classes;
using System.Windows.Threading;
using Microsoft.Win32;
using IWshRuntimeLibrary;
using System.Diagnostics;

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

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void about_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                About aboutWindow = new About();
                aboutWindow.Show();
                aboutWindow.Focus();
                aboutWindow.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("About Window Show(): Непредвиденная ошибка! Ошибка: " + ex.Message);
            }            
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
                format_save = Options.GetValueByAttribute(listOfFormatsForecast_cbx.SelectedItem.ToString());
                autostart_save = (bool)autostartFlag_chbx.IsChecked;
                temperatureUnits_save = (TemperatureUnits)listOfTemperatureUnits_cbx.SelectedItem;
                language_save = (Language)listOfLanguages_cbx.SelectedItem;

                App.settings = new Settings(ChoosenCities, format_save, updatePeriod_save, autostart_save, temperatureUnits_save, language_save);

                App.settingHandler.SaveSettings(App.settings);

                main.applySettings();

                Autorun();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проверьте правильность введенных данных. Ошибка: " + ex.Message);
            }
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
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения состояния автозагрузки. Ошибка: " + ex.Message);
                Debug.WriteLine("Autorun(): Ошибка сохранения состояния автозагрузки. Ошибка: " + ex.Message);
            }
                        
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
                Debug.WriteLine("CreateShortCut(): Ошибка добвления программы в автозагрузку! Ошибка: " + ex.Message);
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
                Debug.WriteLine("DeleteShortCut(): Ошибка удаления программы из автозагрузки! Ошибка: " + ex.Message);
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
            LoadCountries();
            listOfCountries_cbx.SelectedItem = App.settings.cities[0].country.countryRusName;
            listOfCountries_cbx.SelectionChanged += listOfCountries_cbx_SelectionChanged;

            LoadCities();
            listOfCitiies_cbx.SelectedItem = App.settings.cities[0].city.cityRusName;
            listOfCitiies_cbx.SelectionChanged += listOfCitiies_cbx_SelectionChanged;

            LoadChoosenCities();
            ChoosenCitiesComboBox.SelectedIndex = 0;

            updatePeriod_slider.Value = Convert.ToDouble(App.settings.updatePeriod);

            LoadFormats();
            listOfFormatsForecast_cbx.SelectedItem = Options.GetFormatAttribute(App.settings.format);
            listOfFormatsForecast_cbx.SelectionChanged += listOfFormatsForecast_cbx_SelectionChanged;

            LoadTemperatureUnits();
            listOfTemperatureUnits_cbx.SelectedIndex = 0;

            LoadLanguages();
            listOfLanguages_cbx.SelectedIndex = 0;

            autostartFlag_chbx.IsChecked = App.settings.autostart;
        }

        /// <summary>
        /// Подгружает список стран
        /// </summary>
        void LoadCountries()
        {
            List<string> countries = getCity.getCountryNames();
            listOfCountries_cbx.ItemsSource = countries;
        }

        /// <summary>
        /// Подгружает список городов для выбранной страны
        /// </summary>
        /// <param name="country"></param>
        void LoadCities()
        {
            List<string> allCities = getCity.getCities(listOfCountries_cbx.SelectedItem.ToString(), true);
            listOfCitiies_cbx.ItemsSource = allCities;
        }

        /// <summary>
        /// Подгружает список выбранных для отображения городов
        /// </summary>
        void LoadChoosenCities()
        {
            try
            {
                ChoosenCities = new List<CitySettings>(App.settings.cities);
                foreach (var city in ChoosenCities)
                {
                    var item = new ComboBoxItem();
                    item.Tag = city;
                    item.Content = city.ToString();
                    ChoosenCitiesComboBox.Items.Add(item);//(city.ToString());
                    //ChoosenCitiesComboBox.Items.GetItemAt(ChoosenCitiesComboBox.Items.Count - 1).Tag = city;                
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки списка выбранных для отображения городов. Ошибка: " + ex.Message);
                Debug.WriteLine("LoadChoosenCities(): Ошибка загрузки списка выбранных для отображения городов. Ошибка: " + ex.Message);
            }            
        }

        /// <summary>
        /// Подгружает список форматов отображения
        /// </summary>
        void LoadFormats()
        {
            try
            {
                string[] formats = Enum.GetNames(typeof(Options.FormatForecast));
                foreach (var f in formats)
                {
                    var value = Options.GetFormatAttribute(f);
                    listOfFormatsForecast_cbx.Items.Add(value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки списка форматов отображения. Ошибка: " + ex.Message);
                Debug.WriteLine("LoadFormats(): Ошибка загрузки списка форматов отображения. Ошибка: " + ex.Message);
            } 
        }

        /// <summary>
        /// Подгружает список единиц измерения температуры
        /// </summary>
        void LoadTemperatureUnits()
        {
            try
            {
                var temperatureUnits = Options.GetTemperatureUnits();
                foreach (var tempUn in temperatureUnits)
                {
                    listOfTemperatureUnits_cbx.Items.Add(tempUn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки списка единиц измерения температуры. Ошибка: " + ex.Message);
                Debug.WriteLine("LoadTemperatureUnits(): Ошибка загрузки списка единиц измерения температуры. Ошибка: " + ex.Message);
            } 
        }

        /// <summary>
        /// Подгружает список языков локализации программы
        /// </summary>
        void LoadLanguages()
        {
            try
            {
                var languages = Options.GetLanguages();
                foreach (var lang in languages)
                {
                    //var item = new ComboBoxItem();
                    //item.Tag = lang;
                    //item.Content = lang.ToString();
                    listOfLanguages_cbx.Items.Add(lang);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки списка языков локализации программы. Ошибка: " + ex.Message);
                Debug.WriteLine("LoadLanguages(): Ошибка загрузки списка языков локализации программы. Ошибка: " + ex.Message);
            }
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
            format_save = Options.GetValueByAttribute(listOfFormatsForecast_cbx.SelectedItem.ToString());
        }

        private void updatePeriod_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void AddCityButtonClick(object sender, RoutedEventArgs e)
        {
            var curCity = listOfCitiies_cbx.SelectedItem.ToString();
            var curCountry = listOfCountries_cbx.SelectedItem.ToString();

            var newCity = new CitySettings
            {
                country = new Country {
                    countryId = "RU",
                    countryRusName = listOfCountries_cbx.SelectedItem.ToString(),
                    countryEngName = "Russia"
                },
                region = new RegionOfCity {
                    regionId = 0,
                    regionName = "Region"
                },
                city = new City {
                    cityYaId = getCity.getCityId(curCity, true, true, curCountry),
                    cityOWId = getCity.getCityId(curCity, true, false, curCountry),
                    cityRusName = listOfCitiies_cbx.SelectedItem.ToString(),
                    cityEngName = getCity.cityTranslate(curCity, true, curCountry)
                }                
            };

            var repeateSearch = ChoosenCities.SingleOrDefault(el => (el.city.cityYaId == newCity.city.cityYaId   &&
                                                               el.city.cityRusName == newCity.city.cityRusName   &&
                                                               el.country.countryId == newCity.country.countryId &&
                                                               el.country.countryRusName == newCity.country.countryRusName));
            if (repeateSearch != null)
                return;
            ChoosenCities.Add(newCity);
            var item = new ComboBoxItem();
            item.Content = newCity.ToString();
            item.Tag = newCity;
            //ChoosenCitiesComboBox.Items.Add(newCity.ToString());
            //ChoosenCitiesComboBox.Tag = newCity;
            ChoosenCitiesComboBox.Items.Add(item);
            ChoosenCitiesComboBox.SelectedIndex = ChoosenCitiesComboBox.Items.Count - 1;
        }

        private void DeleteCityButtonClick(object sender, RoutedEventArgs e)
        {
            //var countryCity = (ChoosenCitiesComboBox.SelectedItem as String)
            //    .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            //var removeCity = countryCity[0];
            //var removeCountry = countryCity[1];
            //ChoosenCities.Remove(
            //    ChoosenCities.SingleOrDefault(el => (el.city.cityRusName == removeCity && el.country.countryRusName == removeCountry)));
            //ChoosenCitiesComboBox.Items.RemoveAt(ChoosenCitiesComboBox.SelectedIndex);
            //ChoosenCitiesComboBox.SelectedIndex = ChoosenCitiesComboBox.Items.Count - 1;
            var countryCity = (ChoosenCitiesComboBox.SelectedItem as ComboBoxItem).Tag as CitySettings;
            //var removeCity = countryCity[0];
            //var removeCountry = countryCity[1];
            ChoosenCities.Remove(
                ChoosenCities.SingleOrDefault(el => (el.city.cityRusName == countryCity.city.cityRusName && el.country.countryRusName == countryCity.country.countryRusName)));
            ChoosenCitiesComboBox.Items.RemoveAt(ChoosenCitiesComboBox.SelectedIndex);
            ChoosenCitiesComboBox.SelectedIndex = ChoosenCitiesComboBox.Items.Count - 1;

        }

        private void LockUnlockButtons(object sender, SelectionChangedEventArgs e)
        {
            var comboItemsCount = ChoosenCitiesComboBox.Items.Count;
            AddButton.IsEnabled = comboItemsCount < 10;
            DeleteButton.IsEnabled = comboItemsCount > 1;
        }
    }
}
