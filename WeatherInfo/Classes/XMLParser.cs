﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using WeatherInfo.Interfaces;

namespace WeatherInfo.Classes
{
    public class XMLParser : XMLWorker
    {
        string town;
        XDocument weather;
        IWeatherAPI opAPI;
        IYandexWeatherApi yaAPI;
        CultureInfo ci;


        public XMLParser(string _town)
        {
            town = _town;
            opAPI = new OpenWeatherAPI(town);
            yaAPI = new YandexWeatherAPI("27786");
            ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
        }
        
        public ForecastHour getCurHour() 
        {
            weather = opAPI.GetCurrentForecast();
            XElement cur = weather.Root;
            string time = cur.Element("lastupdate").Attribute("value").Value;
            int min = (int)float.Parse(cur.Element("temperature").Attribute("min").Value, NumberStyles.Any, ci);
            int max = (int)float.Parse(cur.Element("temperature").Attribute("max").Value, NumberStyles.Any, ci);
            string clouds = cur.Element("clouds").Attribute("name").Value;
            string icon = cur.Element("weather").Attribute("icon").Value;
            return new ForecastHour(min, clouds, time, icon);
        }

        //массив из пяти листов, в каждом листе почасовые прогнозы. Листы, потому что в первом дне может быть меньше 8 записей
        public List<ForecastDay>[] getDetailedWeek()
        {
            List<ForecastDay>[] res = new List<ForecastDay>[10];
            string apiName = "{http://weather.yandex.ru/forecast}";
            weather = yaAPI.GetForecast();
            XElement root = weather.Root;
            IEnumerable<XElement> days = root.Elements(apiName + "day");
            foreach (var day in days)
            {
                foreach (var hour in day.Elements(apiName + "hour")) 
                {
                    string name = hour.Name.ToString();
                }
            }
            return res;
        }

        public ForecastDay[] getBigForecast()
        {
            ForecastDay[] res = new ForecastDay[14];
            weather = opAPI.GetBigForecast();
            IEnumerable<XElement> forecasts = weather.Root.Element("forecast").Elements();
            int cur = 0;
            foreach (var time in forecasts)
            {
                string date = time.Attribute("day").Value;
                int mor = (int)float.Parse(time.Element("temperature").Attribute("morn").Value, NumberStyles.Any, ci);
                int day = (int)float.Parse(time.Element("temperature").Attribute("day").Value, NumberStyles.Any, ci);
                int eve = (int)float.Parse(time.Element("temperature").Attribute("eve").Value, NumberStyles.Any, ci);
                int ngt = (int)float.Parse(time.Element("temperature").Attribute("night").Value, NumberStyles.Any, ci);
                string clouds = time.Element("clouds").Attribute("value").Value;
                string icon = time.Element("symbol").Attribute("var").Value;
                res[cur] = new ForecastDay(mor, day, eve, ngt, clouds, date, icon);
                cur++;
            }
            return res;
        }
    }
}
