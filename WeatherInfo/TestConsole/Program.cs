using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var request = WebRequest.Create(@"http://api.openweathermap.org/data/2.5/forecast?q=London,us&mode=xml");
            var response = request.GetResponse();
            var data = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Console.WriteLine(data);
            Console.ReadKey();
        }
    }
}
