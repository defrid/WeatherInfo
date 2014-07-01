using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherInfo.Classes
{
    /// <summary>
    /// Класс прогноза, в него будем складывать информацию за отрезок времени
    /// </summary>
    public class Forecast
    {
        int temp;//температура
        int min;
        int max;
        string clouds;//обланость

        public Forecast(int _temp, int _min, int _max, string _clouds)
        {
            int temp = _temp;
            int min = _min;
            int max = _max;
            string clouds = _clouds;
        }
    }
}
