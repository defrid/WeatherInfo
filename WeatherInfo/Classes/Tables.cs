using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data.Entity;

namespace WeatherInfo
{
   public class Tables
    {
        public class Country
        {
            public int CountryId { get; set; }
            public string Name_bukva { get; set; }
            public string Name_country { get; set; }

            public virtual List<Place> Places { get; set; }

        }

        public class City
        {
            public int CityId { get; set; }
            public int Number { get; set; }
            public string Name_cityEn { get; set; }
            public string Name_cityRus { get; set; }

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
            public DbSet<Data> Data { get; set; }
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
                catch (Exception e)
                {
                    string v = e.Message;
                }
            }
        }

        public void show()
        {
            using (var db = new Base())
            {
                var query = from b in db.Cities
                            orderby b.Name_cityRus
                            select b;

                //Console.WriteLine("All blogs in the database:");
                foreach (var item in query)
                {

                    string a = item.Name_cityRus.ToString();
                }
            }
        }

        //методы добавления
        public void ADD_country(string word_b1, string name1)
        {
            using (var db = new Base())
            {
                var name = name1;
                var word_b = word_b1;

                var country = new Country { Name_bukva = word_b, Name_country = name };
                try
                {
                    db.Countries.Add(country);
                    db.SaveChanges();

                }
                catch (Exception e)
                {
                    string error = e.Message;
                }
            }
        }

        public void ADD_placed(int number, string nameEn, string nameRus, string word, string name, int year, string month, string value, string sut, string path, int skal, string razmer)
        {
            using (var db = new Base())
            {
              
                var city = new City { Number = number, Name_cityEn = nameEn, Name_cityRus = nameRus};
                var country = new Country { Name_bukva = word, Name_country = name };
                var place = new Place();
                place.City = city;
                place.Country = country;
                var sutky = new Sutki { Name_sutki = sut };
                var data = new Data { Year = year, Month = month, Day = value};
                data.Sutki = sutky;
                var temp = new Temp { Value = skal, Pazmer = razmer };
                temp.Data = data;
                var pic = new Picture { Path = path};
                var prognoz = new Prognoz();
                prognoz.Place = place;
                prognoz.Temp = temp;
                prognoz.Picture = pic;
               

                try
                {
                    db.Cities.Add(city);
                    db.Countries.Add(country);
                    db.Places.Add(place);
                    db.Picturies.Add(pic);
                    
                    db.Sutkies.Add(sutky);
                    db.Data.Add(data);
                    db.Temps.Add(temp);
                    db.Prognozes.Add(prognoz);
                    db.SaveChanges();

                }
                catch (Exception e)
                {
                    string error = e.Message;
                }
            }
        }

       

        //public void ADD_temperature(string value1, string razmer1, int dataid)
        //{
        //    using (var db = new Base())
        //    {
        //        var value = value1;
        //        var razmer = razmer1;
        //        var d_id = dataid;

        //        var = new Temp { Value=value, Pazmer=razmer, };
        //        try
        //        {
        //            db.Countries.Add(country);
        //            db.SaveChanges();

        //        }
        //        catch (Exception e)
        //        {
        //            string error = e.Message;
        //        }
        //    }
        //}

    }
}
