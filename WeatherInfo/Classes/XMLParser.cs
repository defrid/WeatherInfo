using System;
using System.Collections.Generic;
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
        XmlDocument weather;

        public XMLParser(string _town)
        {
            town = _town;
            WeatherAPI con = null;// = new WeatherAPI(town);
            weather = con.getDetailedWeek();
        }
        
        public Forecast getHours(int hour) 
        {
            Forecast res = null;
            return res;
        }

        public Forecast[] getDay(int day)
        {
            Forecast[] res = null;
            return res;
        }

        public Forecast[][] getDetailedWeek()
        {
            Forecast[][] res = null;
            return res;
        }

        public Forecast[] getBigForecast()
        {
            Forecast[] res = null;
            return res;
        }
        public void update()
        {
            //weather = new 
        }
    }
}
