using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Methods_bd;

namespace WeatherInfo
{
    class Models
    {
        
        Tables tb = new Tables();

        public string country_set(string name)
        {
            string id = "";
            foreach (Countries.Country l in tb.db.Country)
            {
                if (name == l.country_name.ToString())
                {
                    id = l.country_id;
                }
            }
            return id;
        }

    }
}
