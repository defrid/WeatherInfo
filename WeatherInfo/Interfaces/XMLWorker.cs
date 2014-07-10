using System.Collections.Generic;
using DataHandlerInterface.Classes;
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
        ForecastHour getCurHour();

        /// <summary>
        /// получает подробно(по три часа) пять дней
        /// </summary>
        /// <returns>возвращает массив прогнозов дней, которые сами листы прогнозов по часам</returns>
        ForecastDay[] getDetailedWeek();
        
        /// <summary>
        /// Неподробная инфа за 14 дней
        /// </summary>
        /// <returns>массив дней</returns>
        ForecastDay[] getBigForecast();
    }
}
