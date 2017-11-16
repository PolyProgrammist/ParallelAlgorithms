using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MyParallel
{
    class Program
    {
        static void Main(string[] args)
        {
            new RBTTester().TestTime();
            //new RBTTester().TestCorrectnessManually();
           // new Qsort().Test();
           // new DirectoryHash().Test();
           // new CountPrimesTo().Test();
        }

        public static void CountTime(Action action, string name)
        {
            Stopwatch sw = Stopwatch.StartNew();
            action.Invoke();
            long t = sw.ElapsedMilliseconds;
            Console.WriteLine(name + ": " + t);
        }
    }
}
