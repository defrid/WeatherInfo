using System.Xml.Linq;
using System.Drawing;

namespace WeatherInfo.Interfaces
{
    /// <summary>
    /// public должны быть только конструктор и описанный метод, всё остальное private и в целом неважно как будет устроено
    /// </summary>
    interface IWeatherAPI
    {
        /// <summary>
        /// Получение текущего прогноза
        /// </summary>
        /// <returns></returns>
        XDocument GetCurrentForecast();


        /// <summary>
        /// Подробный прогноз на 5 дней с интервалом в 3 часа
        /// </summary>
        /// <returns></returns>
        XDocument GetDetailedWeek();
        /*здесь получаем XML с прогнозом на неделю, 
         * при этом он подробный, а значит отдельно по дням получать уже не нужно
         * пока что это будет XMLDocument, позже тип может смениться, но суть будет одна - XML*/
        
        /// <summary>
        /// Полный прогноз на 14 дней
        /// </summary>
        /// <returns></returns>
        XDocument GetBigForecast();
    }
}
