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
        string townId;
        XDocument weather;
        IWeatherAPI opAPI;
        IYandexWeatherApi yaAPI;
        CultureInfo ci;
        string apiName = "{http://weather.yandex.ru/forecast}";

        public XMLParser(string _town, string _townId)
        {
            town = _town;
            townId = _townId;
            string townEng = translate.toEng(town, "Location//translit.txt");
            opAPI = new OpenWeatherAPI(townEng);
            yaAPI = new YandexWeatherAPI(_townId);
            ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
        }

        public ForecastHour getCurHour()
        {
            weather = opAPI.GetCurrentForecast();
            XElement cur = weather.Root;
            string time = cur.Element("lastupdate").Attribute("value").Value;
            int temp = Int32.Parse(cur.Element("temperature").Attribute("value").Value);
            string clouds = cur.Element("clouds").Attribute("name").Value;
            string icon = cur.Element("weather").Attribute("icon").Value;
            return new ForecastHour(temp, clouds, time, icon);
        }

        //массив из пяти листов, в каждом листе почасовые прогнозы. Листы, потому что в первом дне может быть меньше 8 записей
        public ForecastDay[] getDetailedWeek()
        {
            ForecastDay[] res = new ForecastDay[10];
            weather = yaAPI.GetForecast();
            XElement root = weather.Root;
            IEnumerable<XElement> days = root.Elements(apiName + "day");
            int curDay = 0;
            foreach (var day in days)
            {
                string date = day.Attribute("date").Value;
                res[curDay] = new ForecastDay(0, 0, new List<ForecastHour>(), date, null);
                foreach (var hour in day.Elements(apiName + "hour"))
                {
                    string time = hour.Attribute("at").Value;
                    time = time.Length > 0 ? time : "0" + time;
                    int temp = Int32.Parse(hour.Element(apiName + "temperature").Value);
                    string clouds = hour.Element(apiName + "weather_condition").Attribute("code").Value;
                    string icon = hour.Element(apiName + "image-v3").Value;
                    res[curDay].hours.Add(new ForecastHour(temp, clouds, time, icon));
                }

                IEnumerable<XElement> parts = day.Elements(apiName + "day_part").Where(
                    el => (el.Attribute("typeid").Value != "5" && el.Attribute("typeid").Value != "6"));
                foreach (var day_part in parts)
                {
                    string time = day_part.Attribute("type").Value;
                    int temp;
                    try { temp = Int32.Parse(day_part.Element(apiName + "temperature_to").Value); }
                    catch
                    { temp = Int32.Parse(day_part.Element(apiName + "temperature").Value); }
                    string clouds = day_part.Element(apiName + "weather_condition").Attribute("code").Value;
                    string icon = day_part.Element(apiName + "image-v3").Value;
                    res[curDay].hours.Add(new ForecastHour(temp, clouds, time, icon));
                }
                curDay++;
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
                int min = (int)float.Parse(time.Element("temperature").Attribute("min").Value, NumberStyles.Any, ci);
                int max = (int)float.Parse(time.Element("temperature").Attribute("max").Value, NumberStyles.Any, ci);
                string icon = time.Element("symbol").Attribute("var").Value;
                res[cur] = new ForecastDay(min, max, null, date, icon);
                cur++;
            }
            return res;
        }
    }
}
