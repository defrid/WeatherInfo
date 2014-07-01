using System.Collections.Generic;
using WeatherInfo.Classes;


namespace WeatherInfo.Interfaces
{
    /// <summary>
    /// тут будет обработка xml, и отсюда вызывается API
    /// </summary>
    interface XMLWorker
    {
        //тут конструктор, принимающий город



        /// <summary>
        /// инфа за текущий час
        /// </summary>
        /// <returns></returns>
        Forecast getCurHour();

        /// <summary>
        /// получает подробно(по три часа) пять дней
        /// </summary>
        /// <returns>возвращает массив прогнозов дней, которые сами листы прогнозов по часам</returns>
        List<Forecast>[] getDetailedWeek();
        
        /// <summary>
        /// Неподробная инфа за 14 дней
        /// </summary>
        /// <returns>массив дней</returns>
        Forecast[] getBigForecast();
    }
}
