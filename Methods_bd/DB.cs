using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Data.SqlClient;

namespace Methods_bd
{
    public class DB_class
    {

        public class DB : DataContext
        {
            public DB(string cs)
                : base(cs)
            {
            }
            
            public System.Data.Linq.Table<Setting.Settings> Settings
            {
                get { return this.GetTable<Setting.Settings>(); }

            }

            public System.Data.Linq.Table<Cities.City> City
            {
                get { return this.GetTable<Cities.City>(); }

            }

            public System.Data.Linq.Table<Countries.Country> Country
            {
                get { return this.GetTable<Countries.Country>(); }

            }
        }
    }
}
