using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace WeatherInfo.Classes
{
    public class getCity
    {
        public getCity()
        {
            if (!(Directory.Exists("Location")))
            {
                Directory.CreateDirectory("Location");
            }
        }

        //тут просто храниться инфа
        string all = "";

        /// <summary>
        /// получить идентификатор города
        /// </summary>
        /// <param name="city">город</param>
        /// <returns></returns>
        public int GetCityNumber(string city)
        {
            if (all == "") all = getAll();

            //(\d*)(?=.*Novosibirsk.*)
            Regex reg = new Regex("(\\d*)(?=.*" + city + ".*)", RegexOptions.IgnoreCase);
            var mathes = reg.Match(all);
            int res = 0;
            try
            {
                res = Convert.ToInt32(mathes.Groups[0].ToString());
            }
            catch { }
            return res;
        }

        /// <summary>
        /// получить названия стран
        /// </summary>
        /// <returns></returns>
        public List<string> CountryNames()
        {
            List<string> res = new List<string>();

            if (!(File.Exists("Location//savedCountries.txt")))
            {
                if (all == "") all = getAll();

                Regex reg = new Regex(@"((?<!\w)..(?=\n|$))");
                var mathes = reg.Matches(all);

                using (StreamWriter sw = new StreamWriter("Location//savedCountries.txt"))
                    foreach (var a in mathes)
                    {
                        if (!res.Contains(a.ToString()))
                        {
                            res.Add(a.ToString());
                            sw.WriteLine(a.ToString());
                        }
                    }
            }
            else
            {
                using (StreamReader sr = new StreamReader("Location//savedCountries.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        res.Add(sr.ReadLine());
                    }
                }
            }


            return res;
        }

        /// <summary>
        /// получить города в стране
        /// </summary>
        /// <param name="country">страна</param>
        /// <returns></returns>
        public List<string> CityNames(string country)
        {
            List<string> res = new List<string>();

            if (!(File.Exists("Location//" + country + ".txt")))
            {
                if (all == "") all = getAll();

                Regex reg = new Regex(@"(?<=\d	)(\D*)(?=	[\d-].*" + country + ")");
                var mathes = reg.Matches(all);

                using (StreamWriter sw = new StreamWriter("Location//" + country + ".txt"))
                {
                    foreach (var a in mathes)
                    {
                        if (!res.Contains(a.ToString()) && a.ToString() != string.Empty)
                        {
                            res.Add(a.ToString());
                            sw.WriteLine(a.ToString());
                        }
                    }
                }
            }

            else
            {
                using (StreamReader sr = new StreamReader("Location//" + country + ".txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        res.Add(sr.ReadLine());
                    }
                }
            }

            return res;
        }

        //тут скачивается или грузится инфа
        string getAll()
        {
            string res = "";

            if (!(File.Exists("Location//savedCity.txt")))
            {
                System.Net.WebRequest reqGET = System.Net.WebRequest.Create(@"http://openweathermap.org/help/city_list.txt");
                System.Net.WebResponse resp = reqGET.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                res = sr.ReadToEnd();

                using (StreamWriter sw = new StreamWriter("Location//savedCity.txt"))
                {
                    sw.Write(res);
                }
            }

            else
            {
                using (StreamReader sr = new StreamReader("Location//savedCity.txt"))
                {
                    res = sr.ReadToEnd();
                }
            }

            return res;
        }
    }
}
