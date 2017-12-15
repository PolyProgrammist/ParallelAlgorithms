using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MyParallel
{
    class AwaitWeb
    {
        private string pattern;
        Regex reg;

        public AwaitWeb()
        {
//            string a = @"https://([\w]+.)+[\w]+/";
//            string b = @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$";
//            string c = @"(?<url>http(s)?[\w\.:?&-_=#/]*)";
//            string d = @"^http(s) ?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$";
//            string e = @"<url>";
            string f = @"https://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            pattern = f;
            reg = new Regex(pattern);
        }

        private SortedSet<string> alreadyWas = new SortedSet<string>();

        public void rec(string address)
        {
           // AsyncLen(address);
            Task<bool> t = AsLen(address);
            t.Wait();
            Thread.Sleep(100000);
        }

        public async Task<bool> AsLen(string url, int level = 0, string from = "no input")
        {
            string page = null;
            try
            {
                page = await new WebClient().DownloadStringTaskAsync(url);
            }
            catch (Exception e)
            {
                                Console.WriteLine("----------------");
                Console.WriteLine(e.Message);
                Console.WriteLine("bad: " + url);
                Console.WriteLine("from: "+ from);
                Console.WriteLine("--------------------");

            }
            if (page == null)
                return false;

            Console.WriteLine(url + " - " + page.Length);


            if (level == 1)
                return false;
            MatchCollection mc = reg.Matches(page);
            foreach (Match m in mc)
            {
                //Console.WriteLine("hello " + m.ToString() + " " + alreadyWas.Count);
                if (!alreadyWas.Contains(m.ToString()))
                {
                    alreadyWas.Add(m.ToString());
                    AsLen(m.ToString(), level + 1, url);
                }
            }
            return false;
        }
    }
}
