using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHandlerInterface.Classes
{
    public class ForecastDetailed
    {
        public ForecastDetailed() { }
        public ForecastDetailed(List<ForecastDay[]> _daysForecast)
        {
            forecasts = new List<ForecastDay[]>(_daysForecast);
        }

        public List<ForecastDay[]> forecasts { get; set; }
    }
}
