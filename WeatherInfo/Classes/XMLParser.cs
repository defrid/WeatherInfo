using System;
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
        IWeatherAPI con;
        CultureInfo ci;


        public XMLParser(string _town)
        {
            town = _town;
            con = new WeatherAPI(town);
            ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
        }
        
        public Forecast getCurHour() 
        {
            weather = con.GetCurrentForecast();
            XElement cur = weather.Root;
            string time = cur.Element("lastupdate").Attribute("value").Value;
            int min = (int)float.Parse(cur.Element("temperature").Attribute("min").Value, NumberStyles.Any, ci);
            int max = (int)float.Parse(cur.Element("temperature").Attribute("max").Value, NumberStyles.Any, ci);
            string clouds = cur.Element("clouds").Attribute("name").Value;
            return new Forecast(min, max, clouds, time);
        }

        //массив из пяти листов, в каждом листе почасовые прогнозы. Листы, потому что в первом дне может быть меньше 8 записей
        public List<Forecast>[] getDetailedWeek()
        {
            List<Forecast>[] res = new List<Forecast>[5];
            weather = con.GetDetailedWeek();
            IEnumerable<XElement> forecasts = weather.Root.Element("forecast").Elements();
            int curDay = 0;
            string date = null;
            foreach (var time in forecasts)
            {
                string from = time.Attribute("from").Value;
                int min = (int)float.Parse(time.Element("temperature").Attribute("min").Value, NumberStyles.Any, ci);
                int max = (int)float.Parse(time.Element("temperature").Attribute("max").Value, NumberStyles.Any, ci);
                string clouds = time.Element("clouds").Attribute("value").Value;
                if (date == null)
                {
                    res[curDay] = new List<Forecast>();
                    date = from;
                }
                else if (Int32.Parse(date.Substring(8, 2)) != Int32.Parse(from.Substring(8, 2)))
                {
                    curDay++;
                    if (curDay == 5) break;
                    res[curDay] = new List<Forecast>();
                    date = from;
                }
                res[curDay].Add(new Forecast(min, max, clouds, from));
            } 
            return res;
        }

        public Forecast[] getBigForecast()
        {
            Forecast[] res = new Forecast[14];
            weather = con.GetBigForecast();
            IEnumerable<XElement> forecasts = weather.Root.Element("forecast").Elements();
            int cur = 0;
            foreach (var time in forecasts)
            {
                string from = time.Attribute("day").Value;
                int min = (int)float.Parse(time.Element("temperature").Attribute("min").Value, NumberStyles.Any, ci);
                int max = (int)float.Parse(time.Element("temperature").Attribute("max").Value, NumberStyles.Any, ci);
                string clouds = time.Element("clouds").Attribute("value").Value;
                res[cur] = new Forecast(min, max, clouds, from);
                cur++;
            }
            return res;
        }
    }
}
