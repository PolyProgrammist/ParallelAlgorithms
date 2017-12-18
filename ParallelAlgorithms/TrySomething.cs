using System;
using System.Threading.Tasks;

namespace ParallelAlgorithms
{
    class TrySomething
    {
        public void f()
        {
            Console.WriteLine("kek");
        }

        private static async Task MainSync()
        {
            await Task.Delay(10000);
        }

        private static async Task G()
        {
            await MainSync();
            Console.WriteLine("hello");
        }
    }
}
