using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Data.SqlClient;

namespace DomainModel.Models
{
    public class Cities
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
    }


}

