using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParallelAlgorithms
{
    class CountPrimesTo
    {
        public List<int> CountAsParallel(int to)
        {
            return
            (from n in Enumerable.Range(2, to).AsParallel()
             where IsPrime(n)
             select n).ToList();
        }

        public List<int> CountTask(int to)
        {
            int processes = 4;
            List<List<int>> a = new List<List<int>>(processes);
            List<Task> tasks = new List<Task>(new Task[processes]);
            for (int i = 0; i < processes; i++)
            {
                int start = (to / processes) * i + 1;
                int stop = i == processes - 1 ? to : ((to) / processes * (i + 1));
                tasks[i] = Task.Run(() => a.Add(CountSimple(stop, start)));
            }
            tasks.ForEach((t) => t.Wait());
            return a.SelectMany(x => x).ToList();
        }

        public List<int> CountParallelInvoke(int to)
        {
            int processes = 4;
            List<Tuple<int, int>> ranges = new List<Tuple<int, int>>(processes);
            for (int i = 0; i < processes; i++)
            {
                int start = (to / processes) * i + 1;
                int stop = i == processes - 1 ? to : ((to) / processes * (i + 1));
                ranges.Add(new Tuple<int, int>(start, stop));
            }
            List<List<int>> a = new List<List<int>>();
            Parallel.ForEach(ranges, t=>a.Add(CountSimple(t.Item2, t.Item1)));
            return a.SelectMany(x => x).ToList();
        }

        public List<int> CountSimple(int to, int fr = 0)
        {
            return
            (from n in Enumerable.Range(fr, to - fr + 1)
             where IsPrime(n)
             select n).ToList();
        }

        public void Test(int to = (int)5e6)
        {
            Program.CountTime(() => CountSimple(to), "PrimesSimple");
            Program.CountTime(() => CountAsParallel(to), "PrimesAsParallel");
            Program.CountTime(() => CountTask(to), "PrimesAsTasks");
            Program.CountTime(() => CountParallelInvoke(to), "PrimesParallelInvoke");
        }


        bool IsPrime(int n)
        {
            if (n < 2)
                return false;
            int t = n;
            for (int i = 2; i * i <= n; i++)
                while (t % i == 0)
                    return false;
            return true;
        }
    }
}