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

namespace WeatherInfo
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
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

        public string country_save = "";
        public int cityId_save;
        public string cityName_save = "";
        public string delay_save = "";
        public string format_save = "";
        public bool autostart_save = true;

        private void accept_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                country_save = listOfCountries_cbx.SelectedItem.ToString();
                cityName_save = listOfCitiies_cbx.SelectedItem.ToString();
                cityId_save = gC.GetCityNumber(cityName_save);
                delay_save = Enum.GetName(typeof(Delay), int.Parse(listOfVariablesDelay_cbx.SelectedItem.ToString()));
                format_save = SettingsHandler.GetValueByAttribute(listOfFormatsForecast_cbx.SelectedItem.ToString());
                autostart_save = (bool)autostartFlag_chbx.IsChecked;

                App.settings = new Settings(country_save, cityId_save, cityName_save, format_save, delay_save, autostart_save);

                SettingsHandler.WriteXml(App.settings);
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
            listOfCountries_cbx.SelectedItem = App.settings.country;
            listOfCountries_cbx.SelectionChanged += listOfCountries_cbx_SelectionChanged;

            LoadCities(App.settings.country);
            listOfCitiies_cbx.SelectedItem = App.settings.city.name;
            listOfCitiies_cbx.SelectionChanged += listOfCitiies_cbx_SelectionChanged;

            LoadDelays();
            listOfVariablesDelay_cbx.SelectedItem = (int)Enum.Parse(typeof(Delay), App.settings.delay);
            listOfVariablesDelay_cbx.SelectionChanged += listOfVariablesDelay_cbx_SelectionChanged;

            LoadFormats();
            listOfFormatsForecast_cbx.SelectedItem = SettingsHandler.GetFormatAttribute(App.settings.format);
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
            List<string> allCities = gC.CityNames(country);
            listOfCitiies_cbx.ItemsSource = allCities;            
        }

        void LoadDelays()
        {
            string[] delays = Enum.GetNames(typeof(Delay));
            foreach (var d in delays)
            {
                int value = (int)Enum.Parse(typeof(Delay), d);
                listOfVariablesDelay_cbx.Items.Add(value);
            }
        }

        void LoadFormats()
        {
            string[] formats = Enum.GetNames(typeof(FormatForecast));
            foreach (var f in formats)
            {
                var value = SettingsHandler.GetFormatAttribute(f);
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
        
        private void listOfVariablesDelay_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            delay_save = Enum.GetName(typeof(Delay), int.Parse(listOfVariablesDelay_cbx.SelectedItem.ToString()));
        }

        private void listOfFormatsForecast_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            format_save = SettingsHandler.GetValueByAttribute(listOfFormatsForecast_cbx.SelectedItem.ToString());//Enum.GetName(typeof(FormatForecast), int.Parse(listOfFormatsForecast_cbx.SelectedItem.ToString()));
        }

        private void listOfCitiies_cbx_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Enter:
                    listOfCitiies_cbx.Text = translate.toEng(listOfCitiies_cbx.Text, "Location//translit.txt");
                    if (listOfCitiies_cbx.SelectedItem == null) MessageBox.Show("Город не найден");
                    break;
            }
        }
    }
}
