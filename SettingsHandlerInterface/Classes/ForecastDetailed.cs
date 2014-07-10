using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHandlerInterface.Classes
{
    public class ForecastDetailed
    {
        public ForecastDetailed() { }
        public ForecastDetailed(List<ForecastDay[]> _dtldForecasts, List<ForecastDay[]> _shrtForecasts, List<City> _cities)
        {
            dtldForecasts = new List<ForecastDay[]>(_dtldForecasts);
            shrtForecasts = new List<ForecastDay[]>(_shrtForecasts);
            cities = new List<City>(_cities);
        }

        public List<ForecastDay[]> dtldForecasts { get; set; }
        public List<ForecastDay[]> shrtForecasts { get; set; }
        public List<City> cities { get; set; }
    }
}
