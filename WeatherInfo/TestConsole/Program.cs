using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeatherInfo.Classes;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new WeatherApi("London");
            Console.WriteLine(api.GetCurrentForecast());
            Console.ReadKey();
            Console.WriteLine(api.GetDailyForecast(3));
            Console.ReadKey();
            Console.WriteLine(api.GetDetailedWeek());
            Console.ReadKey();
            Console.WriteLine(api.GetBigForecast());
            Console.ReadKey();
        }
    }
}
