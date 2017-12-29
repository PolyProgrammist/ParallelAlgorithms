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
        private bool[] P;
        public T[] Snapshots;
        public bool Toggle;
        public T Val;

        public RegisterMy(int registerCount)
        {
            Toggle = false;
            P = new bool[registerCount];
            Snapshots = new T[registerCount];
        }
    }
}