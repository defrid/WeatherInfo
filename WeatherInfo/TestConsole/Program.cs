using System;
using WeatherInfo.Classes;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var xml = new XMLParser("Moscow");
            //xml.getBigForecast();
            xml.getCurHour();
            //xml.getDetailedWeek();
            //Console.WriteLine(api.GetCurrentForecast());
            //Console.ReadKey();
            //Console.WriteLine(api.GetDailyForecast(3));
            //Console.ReadKey();
            //Console.WriteLine(api.GetDetailedWeek());
            //Console.ReadKey();
            //Console.WriteLine(api.GetBigForecast());
            //Console.ReadKey();

        }
    }
}
