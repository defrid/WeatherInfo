using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherInfo.Classes;


namespace WeatherInfo.Interfaces
{
    /// <summary>
    /// тут будет обработка xml, и отсюда вызывается API
    /// </summary>
    interface XMLWorker
    {
        //тут конструктор, принимающий город

        Forecast getHours(int hour);//инфа за минимум - 3 часа

        /// <summary>
        /// Подробная инфа за день(возможно для первых пяти дней)
        /// </summary>
        /// <param name="day">номер дня</param>
        /// <returns>массив прогнозов по часам</returns>
        Forecast[] getDay(int day);

        /// <summary>
        /// получает подробно(по три часа) пять дней
        /// </summary>
        /// <returns>возвращает массив прогнозов дней, которые сами массивы прогнозов по часам</returns>
        Forecast[][] getDetailedWeek();
        
        /// <summary>
        /// Неподробная инфа за 14 дней
        /// </summary>
        /// <returns>массив дней</returns>
        Forecast[] getBigForecast();

        /// <summary>
        /// полное обновление прогнозов
        /// </summary>
        void update();
    }
}
