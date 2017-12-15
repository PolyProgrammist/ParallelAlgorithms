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

//            BSTFineGrained<int> bst = new BSTFineGrained<int>();
//            bst.Add(5);
//            bst.Add(6);
//            bst.print();
           new TrySomething().f();
           //new AwaitWeb().rec("https://ya.ru");

           // new AwaitWeb().cycled("https://ya.ru");

            //LinksDownloader.Lol();

            //Thread.Sleep(5000);

//             new RBTTester().TestCorrectnessAuto();
//              new RBTTester().TestCorrectnessSync();
//            new RBTTester().TestAddEraseParallel();
//             new RBTTester().TestTimeSync();
//             new RBTTester().TestOnlyOneComandForEach();

            //   new RBTTester().TestOnlyOneComandForEach();
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
