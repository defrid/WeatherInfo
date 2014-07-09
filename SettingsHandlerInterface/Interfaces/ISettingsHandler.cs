using SettingsHandlerInterface.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SettingsHandlerInterface
{
    public interface ISettingsHandler
    {
        /// <summary>
        /// Метод загржуает настройки из хранилища.
        /// </summary>
        /// <returns></returns>
        Settings LoadSettings();

        /// <summary>
        /// Метод сохраняет настройки в хранилище.
        /// </summary>
        /// <param name="settings"></param>
        void SaveSettings(Settings settings);
    }

    public abstract class SettingsHandler : ISettingsHandler
    {
        public abstract Settings LoadSettings();

        public abstract void SaveSettings(Settings settings);

        /// <summary>
        /// Метод проверяет переданные ему настройки на корректность.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static bool ValidateSettings(Settings settings)
        {

            try
            {
                var isValid = true;

                if (settings == null || settings.cities == null)
                {
                    return false;
                }

                if (settings.cities.Count == 0)
                {
                    return false;
                }

                foreach (var city in settings.cities)
                {
                    if (string.IsNullOrWhiteSpace(city.country.countryId))
                    {
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(city.country.countryRusName))
                    {
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(city.country.countryEngName))
                    {
                        return false;
                    }

                    if (city.region.regionId < 0)
                    {
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(city.region.regionName))
                    {
                        return false;
                    }

                    if (city.city.cityYaId < 0 || city.city.cityOWId < 0)
                    {
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(city.city.cityEngName))
                    {
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(city.city.cityRusName))
                    {
                        return false;
                    }
                }

                if (string.IsNullOrWhiteSpace(settings.format))
                {
                    return false;
                }

                if (settings.updatePeriod < 10 || settings.updatePeriod > 180)
                {
                    return false;
                }

                return isValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
