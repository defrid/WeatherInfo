﻿using SettingsHandlerInterface.Classes;
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
            temperatureUnits_save = new TemperatureUnits("Цельсии", "Celsius"); //"Celsius";
            language_save = new Language("Русский", "Russian");//"Russian";
        }

        getCity gC = new getCity();

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
                format_save = Options.GetValueByAttribute(listOfFormatsForecast_cbx.SelectedItem.ToString());
                autostart_save = (bool)autostartFlag_chbx.IsChecked;
                temperatureUnits_save = (TemperatureUnits)listOfTemperatureUnits_cbx.SelectedItem;
                language_save = (Language)listOfLanguages_cbx.SelectedItem;

                //App.settings = new Settings(countryId_save, countryRusName_save, countryEngName_save, regionId_save, regionName_save, cityYaId_save, cityOWId_save, cityRusName_save, cityEngName_save, format_save, updatePeriod_save, autostart_save, temperatureUnits_save, language_save);
                App.settings = new Settings(ChoosenCities, format_save, updatePeriod_save, autostart_save, temperatureUnits_save, language_save);

                App.settingHandler.SaveSettings(App.settings);

                main.applySettings();

                //Autorun();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проверьте правильность введенных данных. Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Метод утановки автозагрузки приложения.
        /// ВАЖНО: просто наткнулся в сэмплах, позже разобраться, сейчас пусть повисит закомменченным
        /// </summary>
        private void Autorun()
        {
            var path = string.Concat(Environment.CurrentDirectory, @"\WeatherInfo.exe");
            var pathToLnk = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\WeatherInfo.lnk";

            CreateShortCut(path, path.Substring(0, path.LastIndexOf('\\')), pathToLnk);

            RegistryKey key =
                    Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            //Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);

            if ((bool)autostartFlag_chbx.IsChecked)
            {
                key.SetValue("WeatherInfo", pathToLnk);
                //key.SetValue("WeatherInfo", "\\WeatherInfo.exe");

            }
            else
            {
                key.DeleteValue("WeatherInfo", false);
            }
            key.Flush();
            key.Close();            
        }

        private void CreateShortCut(string FilePath, string WorkDir, string shortcutPath)
        {
            WshShell shell = new WshShell();

            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.Description = "Запуск";
            shortcut.TargetPath = FilePath;
            shortcut.WorkingDirectory = WorkDir;
            shortcut.Save();
        }

        private void autostartFlag_chbx_Checked(object sender, RoutedEventArgs e)
        {
            autostart_save = (bool)autostartFlag_chbx.IsChecked;//true;
            //MessageBox.Show("Автозапуск");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCountries();
            listOfCountries_cbx.SelectedItem = App.settings.cities[0].country.countryRusName;
            listOfCountries_cbx.SelectionChanged += listOfCountries_cbx_SelectionChanged;

            LoadCities(App.settings.cities[0].country.countryRusName);
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

        void LoadCountries()
        {
            List<string> countries = gC.CountryNames();
            listOfCountries_cbx.ItemsSource = countries;
        }

        void LoadCities(string country)
        {
            List<string> allCities = gC.CityNamesYandex(country);
            listOfCitiies_cbx.ItemsSource = allCities;
        }

        void LoadChoosenCities()
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

        void LoadFormats()
        {
            string[] formats = Enum.GetNames(typeof(Options.FormatForecast));
            foreach (var f in formats)
            {
                var value = Options.GetFormatAttribute(f);
                listOfFormatsForecast_cbx.Items.Add(value);
            }
        }

        void LoadTemperatureUnits()
        {
            var temperatureUnits = Options.GetTemperatureUnits();
            foreach (var tempUn in temperatureUnits)
            {
                //var item = new ComboBoxItem();
                //item.Tag = tempUn;
                //item.Content = tempUn.ToString();
                listOfTemperatureUnits_cbx.Items.Add(tempUn);
            }
        }

        void LoadLanguages()
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

        private void listOfCountries_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                LoadCities(listOfCountries_cbx.SelectedItem.ToString());
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
                    cityYaId = gC.GetCityNumberYandex(listOfCitiies_cbx.SelectedItem.ToString()),
                    cityOWId = 0,//gC.GetCityNumber(listOfCitiies_cbx.SelectedItem.ToString()),
                    cityRusName = listOfCitiies_cbx.SelectedItem.ToString(),
                    cityEngName = "City"
                }                
            };
            var repeateSearch = ChoosenCities.SingleOrDefault(el => (el.city.cityYaId == newCity.city.cityYaId   &&
                                                               el.city.cityRusName == newCity.city.cityRusName   &&
                                                               el.country.countryId == newCity.country.countryId &&
                                                               el.country.countryRusName == newCity.country.countryRusName));
            if (repeateSearch != null)
                return;
            ChoosenCities.Add(newCity);
            ChoosenCitiesComboBox.Items.Add(newCity.ToString());
            ChoosenCitiesComboBox.Tag = newCity;
            ChoosenCitiesComboBox.SelectedIndex = ChoosenCitiesComboBox.Items.Count - 1;
        }

        private void DeleteCityButtonClick(object sender, RoutedEventArgs e)
        {
            var countryCity = (ChoosenCitiesComboBox.SelectedItem as String)
                .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            var removeCity = countryCity[0];
            var removeCountry = countryCity[1];
            ChoosenCities.Remove(
                ChoosenCities.SingleOrDefault(el => (el.city.cityRusName == removeCity && el.country.countryRusName == removeCountry)));
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
