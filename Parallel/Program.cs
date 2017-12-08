using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace MyParallel
{
    class Program
    {
        static void Main(string[] args)
        {
            /*BSTFineGrained<int> t = new BSTFineGrained<int>();
            t.Add(5);
            t.Add(6);
            t.Remove(5);
            t.Remove(7);
            t.print();//*/
            //Monitor.Enter(null);
            // Console.WriteLine(5.CompareTo(6));
            // new RBTTester().TestTime();
            //new RBTTester().TestCorrectnessManually();
            //new RBTTester().TestCorrectnessAuto();
            //new RBTTester().TestTimeSync();
            new RBTTester().TestCorrectnessAuto();
            new RBTTester().TestCorrectnessSync();
            new RBTTester().TestAddEraseParallel();
            new RBTTester().TestTimeSync();
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
