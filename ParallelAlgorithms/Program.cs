using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ParallelAlgorithms
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            new TrySomething( ).f();
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