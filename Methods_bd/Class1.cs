using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Data.SqlClient;

namespace Methods_bd
{
    public class Class1
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
    }
}
