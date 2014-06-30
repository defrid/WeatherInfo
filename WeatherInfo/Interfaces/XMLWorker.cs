using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherInfo.Interfaces
{
    /// <summary>
    /// тут будет обработка xml, и отсюда вызывается API
    /// все методы будут возвращать массив строк, где:
    /// [0] средняя температура
    /// [1] минимальная
    /// [2] максимальная
    /// [3] облачность
    /// </summary>
    interface XMLWorker
    {
        //тут конструктор, принимающий город

        string[] getHour(int hour);//инфа за один час(отрезок времени, вроде как минимум - 3 часа)

        string[] getDay(int day);//здесь парсится инфа по часам за заданный день

        string[] getWeek();//здесь вся неделя

        void update();//полное обновление текущего xml
    }
}
