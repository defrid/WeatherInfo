using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherInfo.Classes;

namespace WeatherInfo.Interfaces
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
}
