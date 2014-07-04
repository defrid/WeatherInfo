using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Data.SqlClient;

namespace DomainModel.Models
{
    public class Countries
    {
        [Table]
        public class Country
        {
            [Column(IsPrimaryKey = true)]
            public string country_id;
            [Column]
            public string country_name;

        }
    }
}
