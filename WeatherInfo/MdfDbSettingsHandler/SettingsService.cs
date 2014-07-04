using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Data.SqlClient;
using DomainModel;
using DomainModel.Models;
using System.Reflection;

namespace MdfDbSettingsHandler
{
   internal class SettingsService
   {
       internal static string dbPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Config\DataBase.mdf";
       internal static DBSettings db = new DBSettings(@"Data Source=.\SQLEXPRESS;AttachDbFilename=" + dbPath + ";Integrated Security=True;Connect Timeout=30;User Instance=True");

       //добавление
       internal static void ADD_city(int number, string name)
       {
           City city = new City();
           city.cityId = number;
           city._cityName = name;
           db.City.InsertOnSubmit(city);
           db.SubmitChanges();
       }

       internal static void ADD_country(string id, string name)
       {
           Country country = new Country();
           country.country_id = id;
           country.country_name = name;
           db.Country.InsertOnSubmit(country);
           db.SubmitChanges();
       }

       internal static void ADD_settings(string ct_id, int c_id, string Format, int start)
       {
           Settings settings = new Settings();
           settings.country_id = ct_id;
           settings.id = c_id;
           settings.format = Format;
           settings.autostart = start;
           db.Settings.InsertOnSubmit(settings);
           db.SubmitChanges();
       }

       internal static string country_set(string name)
        {
            string id = "";
            foreach (Country l in db.Country)
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
