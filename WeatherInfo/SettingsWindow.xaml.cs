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

        //Settings settings;
        public string country = "Россия,Англия,Турция";
        public string city = "Ульяновск,Москва,Димитровград";

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
        public string city_save = "";
        public string delay_save = "";
        public string format_save = "";
        public bool autostart_save = true;

        private void accept_btn_Click(object sender, RoutedEventArgs e)
        {
            country_save = listOfCountries_cbx.SelectedItem.ToString();
            city_save = listOfCitiies_cbx.SelectedItem.ToString();
            delay_save = Enum.GetName(typeof(Delay), int.Parse(listOfVariablesDelay_cbx.SelectedItem.ToString()));
            format_save = SettingsHandler.GetValueByAttribute(listOfFormatsForecast_cbx.SelectedItem.ToString());
            autostart_save = (bool)autostartFlag_chbx.IsChecked;

            App.settings = new Settings(country_save, city_save, format_save, delay_save, autostart_save);

            SettingsHandler.WriteXml(App.settings);
            Autorun();
            Close();
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
            string[] countries = country.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] cities = city.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < countries.Length; i++)
            {
                listOfCountries_cbx.Items.Add(countries[i]);
            }
            listOfCountries_cbx.SelectedItem = App.settings.country;
            listOfCountries_cbx.SelectionChanged += listOfCountries_cbx_SelectionChanged;

            for (int i = 0; i < cities.Length; i++)
            {
                listOfCitiies_cbx.Items.Add(cities[i]);
            }
            listOfCitiies_cbx.SelectedItem = App.settings.city;
            listOfCitiies_cbx.SelectionChanged += listOfCitiies_cbx_SelectionChanged;

            string[] delays = Enum.GetNames(typeof(Delay));
            foreach (var d in delays)
            {
                int value = (int)Enum.Parse(typeof(Delay), d);
                listOfVariablesDelay_cbx.Items.Add(value);
            }
            listOfVariablesDelay_cbx.SelectedItem = (int)Enum.Parse(typeof(Delay), App.settings.delay);
            listOfVariablesDelay_cbx.SelectionChanged += listOfVariablesDelay_cbx_SelectionChanged;

            string[] formats = Enum.GetNames(typeof(FormatForecast));
            foreach (var f in formats)
            {
                var value = SettingsHandler.GetFormatAttribute(f);
                listOfFormatsForecast_cbx.Items.Add(value);
            }
            listOfFormatsForecast_cbx.SelectedItem = SettingsHandler.GetFormatAttribute(App.settings.format);
            listOfFormatsForecast_cbx.SelectionChanged += listOfFormatsForecast_cbx_SelectionChanged;

            autostartFlag_chbx.IsChecked = App.settings.autostart;
        }

        private void listOfCountries_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            country_save = Enum.GetName(typeof(Delay), int.Parse(listOfCountries_cbx.SelectedItem.ToString()));
        }

        private void listOfCitiies_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            city_save = Enum.GetName(typeof(Delay), int.Parse(listOfCitiies_cbx.SelectedItem.ToString()));
        }
        
        private void listOfVariablesDelay_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            delay_save = Enum.GetName(typeof(Delay), int.Parse(listOfVariablesDelay_cbx.SelectedItem.ToString()));
        }

        private void listOfFormatsForecast_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            format_save = Enum.GetName(typeof(Delay), int.Parse(listOfFormatsForecast_cbx.SelectedItem.ToString()));
        }
    }
}
