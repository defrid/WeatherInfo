using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class DBSettings : DataContext
    {
        public DBSettings(string cs)
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
