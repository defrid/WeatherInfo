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
}
