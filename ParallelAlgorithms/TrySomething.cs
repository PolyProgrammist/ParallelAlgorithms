using System;
using System.Threading.Tasks;

namespace ParallelAlgorithms
{
    internal class TrySomething
    {
        public void F()
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