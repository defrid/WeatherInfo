using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Entity_base
{
    public class DataBase
    {


        public class Country
        {
            public int CountryId { get; set; }
            public string Name_bukva { get; set; }
            public string Name_Englcountry { get; set; }
            public string Name_Ruscountry { get; set; }

            public virtual List<Place> Places { get; set; }

        }

        public class City
        {
            public int CityId { get; set; }
            public int Number { get; set; }
            public string Name_city { get; set; }

            public virtual List<Place> Places { get; set; }

        }

        public class Place
        {
            public int PlaceId { get; set; }

            public int CountyId { get; set; }
            public int CityId { get; set; }

            public virtual List<Prognoz> Prognozes { get; set; }

            public virtual Country Country { get; set; }
            public virtual City City { get; set; }
        }

        public class Sutki
        {
            public int SutkiId { get; set; }
            public string Name_sutki { get; set; }

           // public virtual List<Prognoz> Prognozes { get; set; }

            public virtual List<Data> Datas { get; set; }
        }

        public class Data
        {
            public int DataId { get; set; }
            public int Year { get; set; }
            public string Month { get; set; }
            public string Day { get; set; }

            public virtual List<Temp> Temps { get; set; }
            public virtual List<Prognoz> Prognozes { get; set; }

            public int SutkiId { get; set; }
            public virtual Sutki Sutki { get; set; }
        }

        public class Temp
        {
            public int TempId { get; set; }
            public int Value { get; set; }
            public string Pazmer { get; set; }

            public virtual List<Prognoz> Prognozes { get; set; }

            public int DataId { get; set; }
            public virtual Data Data { get; set; }
        }

        public class Picture
        {
            public int PictureId { get; set; }
            public string Path { get; set; }

            public virtual List<Prognoz> Prognozes { get; set; }

        }

        public class Prognoz
        {
            public int PrognozId { get; set; }

            public int PlaceId { get; set; }
           // public int DataId { get; set; }
            public int TempId { get; set; }
            public int PictureId { get; set; }
           // public int SutkiId { get; set; }

            public virtual Place Place { get; set; }
           // public virtual Data Data { get; set; }
            public virtual Temp Temp { get; set; }
            public virtual Picture Picture { get; set; }
           // public virtual Sutki Sutki { get; set; }
        }

        public class Base : System.Data.Entity.DbContext
        {
            public DbSet<Country> Countries { get; set; }
            public DbSet<City> Cities { get; set; }
            public DbSet<Temp> Temps { get; set; }
            public DbSet<Picture> Picturies { get; set; }
            public DbSet<Sutki> Sutkies { get; set; }
            public DbSet<Prognoz> Prognozes { get; set; }
            public DbSet<Place> Places { get; set; }
        }

        public void ADD_BD()
        {
            using (var db = new Base())
            {
                // Create and save a new Blog 
                // Console.Write("Enter a name for a new Blog: ");
                var name = "djcbsdjkcb";

                var blog = new Picture { Path = name };
                try
                {
                    db.Picturies.Add(blog);
                    db.SaveChanges();

                }
                catch(Exception e)
                {
                    string v = e.Message;
                }
            }
        }

        public void show()
        {
            using (var db = new Base())
            {
                var query = from b in db.Picturies
                            orderby b.Path
                            select b;

                //Console.WriteLine("All blogs in the database:");
                foreach (var item in query)
                {

                    string a = item.Path.ToString();
                }
            }
        }
    }

}



