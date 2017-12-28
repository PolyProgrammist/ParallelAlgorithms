using System;
using System.Diagnostics;

namespace ParallelAlgorithms
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            AtomicSnapshots<int>.Test();
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine("hello");
                }
            }
        }

        public static void Lol(int t)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("hello");
            }
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