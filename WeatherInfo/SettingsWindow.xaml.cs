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

        public string countryId_save = "RU";
        public string countryRusName_save = "";
        public string countryEngName_save = "Country";
        public int regionId_save = 0;
        public string regionName_save = "Region";
        public int cityYaId_save;
        public int cityOWId_save;
        public string cityRusName_save = "";
        public string cityEngName_save = "City";
        public int updatePeriod_save = 10;
        public string format_save = "";
        public bool autostart_save = true;
        public string temperatureUnits_save = "Celsius";
        public string language_save = "Russian";

        private void accept_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                countryRusName_save = listOfCountries_cbx.SelectedItem.ToString();
                cityRusName_save = listOfCitiies_cbx.SelectedItem.ToString();
                cityYaId_save = gC.GetCityNumberYandex(cityRusName_save);
                //cityName_save = translate.toEng(listOfCitiies_cbx.SelectedItem.ToString(), "Location//translit.txt");
                updatePeriod_save = Convert.ToInt32(updatePeriod_slider.Value);
                format_save = Options.GetValueByAttribute(listOfFormatsForecast_cbx.SelectedItem.ToString());
                autostart_save = (bool)autostartFlag_chbx.IsChecked;

                App.settings = new Settings(countryId_save, countryRusName_save, countryEngName_save, regionId_save, regionName_save, cityYaId_save, cityOWId_save, cityRusName_save, cityEngName_save, format_save, updatePeriod_save, autostart_save, temperatureUnits_save, language_save);

                App.settingHandler.SaveSettings(App.settings);

                main.applySettings();

                Autorun();
                Close();
            }
            catch
            {
                MessageBox.Show("Проверьте правильность введенных данных");
            }
        }

        /// <summary>
        /// Метод утановки автозагрузки приложения.
        /// ВАЖНО: просто наткнулся в сэмплах, позже разобраться, сейчас пусть повисит закомменченным
        /// </summary>
        private void Autorun()
        {
            /*if ((bool)autostartFlag_chbx.IsChecked)
            {
                Microsoft.Win32.RegistryKey Key =
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
                Key.SetValue("WeatherInfo", "\\WeatherInfo.exe");
                Key.Close();
            }
            else
            {
                Microsoft.Win32.RegistryKey key =
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.DeleteValue("WeatherInfo", false);
                key.Close();
            }*/
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

            updatePeriod_slider.Value = Convert.ToDouble(App.settings.updatePeriod);

            LoadFormats();
            listOfFormatsForecast_cbx.SelectedItem = Options.GetFormatAttribute(App.settings.format);
            listOfFormatsForecast_cbx.SelectionChanged += listOfFormatsForecast_cbx_SelectionChanged;

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

        void LoadFormats()
        {
            string[] formats = Enum.GetNames(typeof(Options.FormatForecast));
            foreach (var f in formats)
            {
                var value = Options.GetFormatAttribute(f);
                listOfFormatsForecast_cbx.Items.Add(value);
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
    }
}
