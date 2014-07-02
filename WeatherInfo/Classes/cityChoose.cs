using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
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

        #region openweather
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
            Dictionary<string, string> two_full = new Dictionary<string, string>();

            //countries.txt

            if (!File.Exists("Location//countries.txt")) throw new Exception("В папке Location не найден countries.txt");
            using (StreamReader sr = new StreamReader("Location//countries.txt"))
            {
                //"	"
                while (!sr.EndOfStream)
                {
                    string[] temp = sr.ReadLine().Split('	');
                    two_full.Add(temp[1], temp[0]);
                }
            }


            if (!(File.Exists("Location//savedCountries.txt")))
            {
                if (all == "") all = getAll();

                Regex reg = new Regex(@"((?<!\w)..(?=\n|$))");
                var mathes = reg.Matches(all);

                using (StreamWriter sw = new StreamWriter("Location//savedCountries.txt"))
                    foreach (var a in mathes)
                    {
                        if (two_full.ContainsKey(a.ToString()) && !res.Contains(two_full[a.ToString()]))
                        {
                            res.Add(two_full[a.ToString()]);
                            sw.WriteLine(two_full[a.ToString()]);
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

            res = res.OrderBy(item => item).ToList();

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

            if (!File.Exists("Location//countries.txt")) throw new Exception("В папке Location не найден countries.txt");
            using (StreamReader sr = new StreamReader("Location//countries.txt"))
            {
                //"	"
                while (!sr.EndOfStream)
                {
                    string[] temp = sr.ReadLine().Split('	');
                    if (temp[0] == country) country = temp[1];
                }
            }

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

            res = res.OrderBy(item => item).ToList();

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
        #endregion

        #region Yandex
        #region model
        [XmlRoot("city")]
        public class city
        {
            [XmlText]
            public string name { get; set; }
            [XmlAttribute("id")]
            public int id { get; set; }
        }

        [XmlRoot("country")]
        public class country
        {
            [XmlAttribute("name")]
            public string name { get; set; }

            [XmlElement("city")]
            public List<city> cities { get; set; }

            public country() { cities = new List<city>(); }
        }

        [XmlRoot("cities")]
        public class SpCountry
        {
            public SpCountry() { countries = new List<country>(); }
            [XmlElement(ElementName = "country")]
            public List<country> countries { get; set; }
        }
        #endregion

        XmlSerializer xs = new XmlSerializer(typeof(SpCountry));
        SpCountry YandexFile;

        /// <summary>
        /// получить идентификатор города
        /// </summary>
        /// <param name="city">город</param>
        /// <returns></returns>
        public int GetCityNumberYandex(string city)
        {
            if (YandexFile == null) YAgetAll();

            foreach (var a in YandexFile.countries)
            {
                foreach (var b in a.cities)
                {
                    if (b.name == city) return b.id;
                }
            }

            return 0;
        }

        /// <summary>
        /// получить названия стран
        /// </summary>
        /// <returns></returns>
        public List<string> CountryNamesYandex()
        {
            if (YandexFile == null) YAgetAll();
            List<string> res = new List<string>();

            foreach (var a in YandexFile.countries)
            {
                res.Add(a.name);
            }

            res = res.OrderBy(item => item).ToList();
            return res;
        }

        /// <summary>
        /// получить города в стране
        /// </summary>
        /// <param name="country">страна</param>
        /// <returns></returns>
        public List<string> CityNamesYandex(string country)
        {
            if (YandexFile == null) YAgetAll();
            List<string> res = new List<string>();

            var F = YandexFile.countries.Where(c => c.name == country).FirstOrDefault();
            foreach (var a in F.cities)
            {
                res.Add(a.name);
            }

            res = res.OrderBy(item => item).ToList();
            return res;
        }

        //тут скачивается или грузится инфа
        void YAgetAll()
        {
            string res = "";

            if (!(File.Exists("Location//YandexFile.txt")))
            {
                System.Net.WebRequest reqGET = System.Net.WebRequest.Create(@"http://weather.yandex.ru/static/cities.xml");
                System.Net.WebResponse resp = reqGET.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                res = sr.ReadToEnd();

                using (StreamWriter sw = new StreamWriter("Location//YandexFile.txt"))
                {
                    sw.Write(res);
                }
            }

            else
            {
                using (StreamReader sr = new StreamReader("Location//YandexFile.txt"))
                {
                    res = sr.ReadToEnd();
                }
            }

            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(res));
            YandexFile = (SpCountry)xs.Deserialize(memoryStream);
        }
        #endregion
    }
}
