using SettingsHandlerInterface;
using SettingsHandlerInterface.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdfDbSettingsHandler
{
    public class MdfDbSettingsHandler : SettingsHandler
    {
        public override void SaveSettings(Settings settings)
        {
            WriteToDb(settings);
        }

        public override Settings LoadSettings()
        {
            //var settings = ReadXml();

            //return settings;
            return Settings.GetDefaultSettings();
        }

        private void WriteToDb(Settings settings)
        {
            try
            {
                string countryId = "RU";

                SettingsService.ADD_country(countryId, settings.country);
                SettingsService.ADD_city(settings.city.cityId, settings.city.cityName);
                SettingsService.ADD_settings(countryId, settings.city.cityId, settings.format, settings.autostart ? 1 : 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SettingsHandler.WriteXml(): Непредвиденная ошибка. Не удалось сохранить настройки. Текст ошибки: " + ex.Message);
            }
        }
    }
}
