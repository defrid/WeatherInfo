using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Data.Linq;
using System.Linq;
using Tomers.WPF.Localization;

namespace WeatherInfo.Classes
{
    public class getCity
    {
        /// <summary>
        /// эту модель использую для десериализации xml с данными городов
        /// </summary>
        #region model_Full
        [XmlRoot("city")]
        public class cityFull
        {
            [XmlAttribute("nameRu")]
            public string nameRu { get; set; }
            [XmlAttribute("nameEng")]
            public string nameEng { get; set; }
            [XmlAttribute("idYa")]
            public int idYa { get; set; }
            [XmlAttribute("idOp")]
            public int idOp { get; set; }
        }

        [XmlRoot("country")]
        public class countryFull
        {
            [XmlAttribute("nameRu")]
            public string nameRu { get; set; }

            [XmlAttribute("nameEng")]
            public string nameEng { get; set; }

            [XmlAttribute("nameShort")]
            public string nameShort { get; set; }

            [XmlElement("city")]
            public List<cityFull> cities { get; set; }

            public countryFull() { cities = new List<cityFull>(); }
        }

        [XmlRoot("cities")]
        public class SpCountryFull
        {
            public SpCountryFull() { countries = new List<countryFull>(); }
            [XmlElement(ElementName = "country")]
            public List<countryFull> countries { get; set; }
        }
        #endregion



        static SpCountryFull File;

        /// <summary>
        /// Этот метод вернет всю иерархию
        /// </summary>
        /// <returns></returns>
        static public SpCountryFull getFullObject()
        {
            if (File == null)
            {
                XmlSerializer xs = new XmlSerializer(typeof(SpCountryFull));

                if (!Directory.Exists("Location"))
                {
                    throw new Exception(LanguageDictionary.Current.Translate<string>("locationFolderNotFound_gC", "Content"));
                }

                if (!System.IO.File.Exists(@"Location/Data.xml"))
                {
                    throw new Exception(LanguageDictionary.Current.Translate<string>("locationDataFileNotFound_gC", "Content"));
                }

                try
                {
                    File = (SpCountryFull)xs.Deserialize(new StreamReader(@"Location/Data.xml"));
                }
                catch { throw new Exception(@"Файл Location/Data.xml был поврежден!"); }
            }

            return File;
        }

        /// <summary>
        /// Возвращает названия стран (всё на русском)
        /// </summary>
        /// <returns></returns>
        static public List<string> getCountryNames()
        {
            getFullObject();

            List<string> res = new List<string>();

            foreach (var contr in File.countries)
            {
                res.Add(contr.nameRu);
            }

            res = res.OrderBy(item => item).ToList();
            return res;
        }

        /// <summary>
        /// Возвращает города в стране
        /// </summary>
        /// <param name="CountryName_rus">Русское название страны</param>
        /// <param name="needRussianName">true - русское название города, false - английское</param>
        /// <returns></returns>
        static public List<string> getCities(string CountryName_rus, bool needRussianName)
        {
            getFullObject();

            List<string> res = new List<string>();

            var Fcountry = File.countries.Where(c => c.nameRu == CountryName_rus).FirstOrDefault();
            if (Fcountry != null)
            {
                foreach (var cit in Fcountry.cities)
                {
                    if (needRussianName) res.Add(cit.nameRu);
                    else res.Add(cit.nameEng);
                }
            }

            res = res.OrderBy(item => item).ToList();
            return res;
        }

        /// <summary>
        /// Возвращает id города
        /// </summary>
        /// <param name="nameOFCity">Название (русское или английское)</param>
        /// <param name="WeHaveRusName">Вы послали русское название?</param>
        /// <param name="WeNeedYandexId">Вам нужно id для яндекса?</param>
        /// <param name="nameOfCountry_rus">Если известно, укажите название страны, иначе везде будем искать</param>
        /// <returns></returns>
        static public int getCityId(string nameOFCity, bool WeHaveRusName, bool WeNeedYandexId, string nameOfCountry_rus = "*")
        {
            getFullObject();
            cityFull Fc = null;

            if (nameOfCountry_rus == "*")
            {
                foreach (var c in File.countries)
                {
                    if (WeHaveRusName)
                    {
                        Fc = c.cities.Where(n => n.nameRu == nameOFCity).FirstOrDefault();
                    }
                    else
                    {
                        Fc = c.cities.Where(n => n.nameEng == nameOFCity).FirstOrDefault();
                    }

                    if (Fc != null) break;
                }
            }

            else
            {
                var countr = File.countries.Where(c => c.nameRu == nameOfCountry_rus).FirstOrDefault();
                if (WeHaveRusName)
                {
                    Fc = countr.cities.Where(c => c.nameRu == nameOFCity).FirstOrDefault();
                }
                else
                {
                    Fc = countr.cities.Where(c => c.nameEng == nameOFCity).FirstOrDefault();
                }
            }

            if (WeNeedYandexId) return Fc.idYa;
            else return Fc.idOp;
        }


        /// <summary>
        /// Получает из русского названия английское или наоборот
        /// </summary>
        /// <param name="nameOFCity">Название</param>
        /// <param name="WeHaveRusName">Оно русское? True - вренем английское и наоборот</param>
        /// <param name="nameOfCountry_rus">Если известно, задай страну</param>
        /// <returns></returns>
        static public string cityTranslate(string nameOFCity, bool WeHaveRusName, string nameOfCountry_rus = "*")
        {
            getFullObject();
            cityFull Fc = null;

            if (nameOfCountry_rus == "*")
            {
                foreach (var c in File.countries)
                {
                    if (WeHaveRusName)
                    {
                        Fc = c.cities.Where(n => n.nameRu == nameOFCity).FirstOrDefault();
                    }
                    else
                    {
                        Fc = c.cities.Where(n => n.nameEng == nameOFCity).FirstOrDefault();
                    }

                    if (Fc != null) break;
                }
            }

            else
            {
                var countr = File.countries.Where(c => c.nameRu == nameOfCountry_rus).FirstOrDefault();
                if (WeHaveRusName)
                {
                    Fc = countr.cities.Where(c => c.nameRu == nameOFCity).FirstOrDefault();
                }
                else
                {
                    Fc = countr.cities.Where(c => c.nameEng == nameOFCity).FirstOrDefault();
                }
            }

            if (WeHaveRusName) return Fc.nameEng;
            else return Fc.nameRu;

        }

        /// <summary>
        /// Переводит название страны
        /// </summary>
        /// <param name="nameOFcountry">Название страны</param>
        /// <param name="WeHaveRusName">Оно русское? True - вренем английское и наоборот</param>
        /// <returns></returns>
        static public string countryTranslate(string nameOFcountry, bool WeHaveRusName)
        {
            getFullObject();

            countryFull F = null;
            if (WeHaveRusName) F = File.countries.Where(c => c.nameRu == nameOFcountry).FirstOrDefault();
            else F = File.countries.Where(c => c.nameEng == nameOFcountry).FirstOrDefault();

            if (WeHaveRusName) return F.nameEng;
            else return F.nameRu;
        }
    }
}