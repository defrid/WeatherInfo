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
        //public string delay = "10,15,20,30";
        //public string format = "по неделям,по дням,по часам";

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
        public string delay_save = "";//Delay.slightDelay;
        public string format_save = "";//FormatForecast.Days;
        public bool autostart_save = true;

        private void accept_btn_Click(object sender, RoutedEventArgs e)
        {
            country_save = listOfCountries_cbx.Text.ToString();
            city_save = listOfCitiies_cbx.Text.ToString();
            delay_save = Enum.GetName(typeof(Delay), int.Parse(listOfVariablesDelay_cbx.Text));//(Delay)Enum.Parse(typeof(Delay), listOfVariablesDelay_cbx.Text.ToString());
            format_save = SettingsHandler.GetValueByAttribute(listOfFormatsForecast_cbx.Text.ToString());//Enum.GetName(typeof(FormatForecast), SettingsHandler.GetValueByAttribute(listOfFormatsForecast_cbx.Text.ToString()));//(FormatForecast)Enum.Parse(typeof(FormatForecast), SettingsHandler.GetValueByAttribute(listOfFormatsForecast_cbx.Text.ToString()));
            autostart_save = (bool)autostartFlag_chbx.IsChecked;

            App.settings = new Settings(country_save, city_save, format_save, delay_save, autostart_save);

            SettingsHandler.WriteXml(App.settings);

            //MessageBox.Show(country_save + " " + city_save + " " + format_save + " " + delay_save);
        }

        bool flag = false;
        private void autostartFlag_chbx_Checked(object sender, RoutedEventArgs e)
        {
            flag = (bool)autostartFlag_chbx.IsChecked;//true;
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

            for (int i = 0; i < cities.Length; i++)
            {
                listOfCitiies_cbx.Items.Add(cities[i]);
            }
            listOfCitiies_cbx.SelectedItem = App.settings.city;

            string[] delays = Enum.GetNames(typeof(Delay));//delay.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var d in delays)
            {
                int value = (int)Enum.Parse(typeof(Delay), d);
                listOfVariablesDelay_cbx.Items.Add(value);
            }
            listOfVariablesDelay_cbx.SelectedItem = (int)Enum.Parse(typeof(Delay), App.settings.delay);

            string[] formats = Enum.GetNames(typeof(FormatForecast));//format.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var f in formats)
            {
                var value = SettingsHandler.GetFormatAttribute(f);
                listOfFormatsForecast_cbx.Items.Add(value);
            }
            listOfFormatsForecast_cbx.SelectedItem = SettingsHandler.GetFormatAttribute(App.settings.format);

            autostartFlag_chbx.IsChecked = App.settings.autostart;
        }        
    }
}
