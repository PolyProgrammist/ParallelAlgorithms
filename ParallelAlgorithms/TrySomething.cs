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
    public class RegisterMy<T>
    {
        public bool[] P { get; set; }
        public T[] Snapshots { get; set; }
        public bool Toggle { get; set; }
        public T Val { get; set; }

        public RegisterMy(int registerCount)
        {
            Toggle = false;
            P = new bool[registerCount];
            Snapshots = new T[registerCount];
        }
    }
}