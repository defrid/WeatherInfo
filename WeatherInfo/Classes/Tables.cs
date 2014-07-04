using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Data.SqlClient;
using Methods_bd;

namespace WeatherInfo
{
    class Tables
    {

        public Class1.DB db = new Class1.DB(@"Data Source=.\SQLEXPRESS;AttachDbFilename=" + "C:\\Users\\vip\\Documents\\DataBase.mdf" + ";Integrated Security=True;Connect Timeout=30;User Instance=True");

        //добавление
        public void ADD_city(int number, string name)
        {
            Class1.City city = new Class1.City();
            city.cityId = number;
            city._cityName = name;
            db.City.InsertOnSubmit(city);
            db.SubmitChanges();
        }

        public void ADD_country(string id, string name)
        {
            Class1.Country country = new Class1.Country();
            country.country_id = id;
            country.country_name = name;
            db.Country.InsertOnSubmit(country);
            db.SubmitChanges();
        }

        public void ADD_settings(string ct_id, int c_id, string Format, int start)
        {
            Class1.Settings settings = new Class1.Settings();
            settings.country_id = ct_id;
            settings.id = c_id;
            settings.format = Format;
            settings.autostart = start;
            db.Settings.InsertOnSubmit(settings);
            db.SubmitChanges();
        }
    }
}
