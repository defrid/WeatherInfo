using System;
using System.Collections.Generic;
using DataHandlerInterface.Classes;

namespace DataHandlerInterface.Interfaces
{
    public interface IDataHandler
    {
        /// <summary>
        /// Метод загржуает настройки из хранилища.
        /// </summary>
        /// <returns>Объект, содержащий загруженные настройки.</returns>
        UserSettings LoadSettings();

        /// <summary>
        /// Метод сохраняет настройки в хранилище.
        /// </summary>
        /// <param name="settings"></param>
        void SaveSettings(UserSettings settings);

        /// <summary>
        /// Метод сохраняет прогноз погоды в хранилище.
        /// </summary>
        /// <param name="forecastDetailed">Объект, содержащий информацию о погоде для выбранных городов</param>
        void SaveForecasrDetailed(ForecastDetailed forecastDetailed);

        /// <summary>
        /// Метод загружает хранящийся прогноз погоды из хранилища.
        /// </summary>
        /// <returns>Объект, содержащий информацию о погоде для выбранных городов</returns>
        ForecastDetailed LoadForecastDetailed();
    }

    public abstract class DataHandler : IDataHandler
    {
        public abstract UserSettings LoadSettings();

        public abstract void SaveSettings(UserSettings settings);
        public abstract void SaveForecasrDetailed(ForecastDetailed forecastDetailed);
        public abstract ForecastDetailed LoadForecastDetailed();

        /// <summary>
        /// Метод проверяет переданные ему настройки на корректность.
        /// </summary>
        /// <param name="settings">Объект, содержащий текущие настройки, сохраняемые или загружаемые, предназнаенные для проверки.</param>
        /// <returns>Флаг успешности проверки.</returns>
        public static bool ValidateSettings(UserSettings settings)
        {

            try
            {
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

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
