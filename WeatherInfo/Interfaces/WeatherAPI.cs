using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WeatherInfo.Interfaces
{
    /// <summary>
    /// public должны быть только конструктор и описанный метод, всё остальное private и в целом неважно как будет устроено
    /// </summary>
    interface WeatherAPI
    {
        //тут должен быть конструктор, здесь по идее подключение к серверу

        XmlDocument getDetailedWeek();
        /*здесь получаем XML с прогнозом на неделю, 
         * при этом он подробный, а значит отдельно по дням получать уже не нужно
         * пока что это будет XMLDocument, позже тип может смениться, но суть будет одна - XML*/

        /// <summary>
        /// Полный прогноз на 14 дней
        /// </summary>
        /// <returns></returns>
        XmlDocument getBigForecast();
    }
}
