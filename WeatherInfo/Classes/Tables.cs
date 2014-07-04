using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Data.SqlClient;

namespace WeatherInfo
{
    class Tables
    {
        [Table]
        public class City
        {
            [Column(IsPrimaryKey = true, IsDbGenerated = true)]
            public int id;
            [Column]
            public string _cityName;
            [Column]
            public int cityId;
        }

        [Table]
        public class Country
        {
            [Column(IsPrimaryKey = true)]
            public string country_id;
            [Column]
            public string country_name;
            
        }

        [Table]
        public class Settings
        {
            [Column(IsPrimaryKey = true, IsDbGenerated = true)]
            public int Id_settings;
            [Column]
            public string country_id;
            [Column]
            public int id;
            [Column]
            public string format;
            [Column]
            public int updatePeriod;
            [Column]
            public int autostart;
        }

        public class DB : DataContext
        {
            public DB(string cs)
                : base(cs)
            {
            }

            public System.Data.Linq.Table<Settings> Settings
            {
                get { return this.GetTable<Settings>(); }

            }

            public System.Data.Linq.Table<City> City
            {
                get { return this.GetTable<City>(); }

            }

            public System.Data.Linq.Table<Country> Country
            {
                get { return this.GetTable<Country>(); }

            }
        }

        public DB db = new DB(@"Data Source=.\SQLEXPRESS;AttachDbFilename=" + "C:\\Users\\vip\\Documents\\DataBase.mdf" + ";Integrated Security=True;Connect Timeout=30;User Instance=True");

        //добавление
        public void ADD_city(int number, string name)
        {
            City city = new City();
            city.cityId = number;
            city._cityName = name;
            db.City.InsertOnSubmit(city);
            db.SubmitChanges();
        }

        public void ADD_country(string id,string name)
        {
            Country country = new Country();
            country.country_id = id;
            country.country_name = name;
            db.Country.InsertOnSubmit(country);
            db.SubmitChanges();
        }

        public void ADD_settings(string ct_id, int c_id, string Format, int start  )
        {
            Settings settings = new Settings();
            settings.country_id = ct_id;
            settings.id = c_id;
            settings.format = Format;
            settings.autostart = start;
            db.Settings.InsertOnSubmit(settings);
            db.SubmitChanges();
        }
    }
}
