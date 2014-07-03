using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using WeatherInfo.Classes;
using WeatherInfo.Interfaces;

namespace WeatherInfo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ISettingsHandler settingHandler = new XMLSettingsHandler();
        public static Settings settings = settingHandler.LoadSettings();
    }
}
