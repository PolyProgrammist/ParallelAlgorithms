using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyParallel
{
    class TrySomething
    {
        public void f()
        {
            Task t = g();
            //t.Wait();
        }

        private async static Task MainSync()
        {
            await Task.Delay(10000);
        }

        private async static Task g()
        {
            await MainSync();
            Console.WriteLine("hello");
        }
    }
}
