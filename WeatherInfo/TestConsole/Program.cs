﻿using System;
using WeatherInfo.Classes;
using System.Xml.Linq;
using System.Linq;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var xml = new XMLParser("Moscow");
            //xml.getBigForecast();
            //xml.getCurHour();
            //xml.getDetailedWeek();
            //Console.WriteLine(api.GetCurrentForecast());
            //Console.ReadKey();
            //Console.WriteLine(api.GetDailyForecast(3));
            //Console.ReadKey();
            //Console.WriteLine(api.GetDetailedWeek());
            //Console.ReadKey();
            //Console.WriteLine(api.GetBigForecast());
            //Console.ReadKey();
            var doc = XDocument.Parse("<x><t><e var=\"n\"/><e var=\"m\"/></t></x>");
            Console.WriteLine(doc.Element("x").Element("t"));
            Console.WriteLine(new XDocument(doc.Element("x").Element("t").Elements()));
            Console.WriteLine(doc);
            Console.ReadKey();
        }
    }
}
