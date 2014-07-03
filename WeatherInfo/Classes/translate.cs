using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WeatherInfo.Classes
{
    public class translate
    {
        /// <summary>
        /// Переводит русскую строчку в транслит
        /// </summary>
        /// <param name="Rus">строка</param>
        /// <param name="WayToTranslit">путь к файлу со словарем</param>
        /// <returns></returns>
        public static string toEng(string Rus, string WayToTranslit)
        {
            switch (Rus)
            {
                case "Москва": return "Moscow";
                case "Санкт-Петербург": return "Saint Petersburg";
            }

            Dictionary<char, string> table = new Dictionary<char, string>();

            if (File.Exists(WayToTranslit))
                using (StreamReader sr = new StreamReader(WayToTranslit))
                {
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            string[] a = sr.ReadLine().Split('=');
                            table.Add(Convert.ToChar(a[0]), a[1]);
                        }
                        catch { }
                    }
                }
            else
            {
                throw new Exception("Файл словаря транслита не найден. Соболезнуем.");
            }

            List<char> res = new List<char>();

            for (int k = 0; k < Rus.Length; k++)
            {
                char aT = Char.ToLower(Rus[k]);
                if (table.ContainsKey(aT))
                    for (int i = 0; i < table[aT].Length; i++)
                    {
                        char toadd = table[aT][i];
                        if (k == 0 && i == 0) toadd = Char.ToUpper(toadd);
                        if (res.Count > 0 && res.Last() == ' ') toadd = Char.ToUpper(toadd);

                        res.Add(toadd);
                    }
            }


            return new string(res.ToArray());
        }
    }
}