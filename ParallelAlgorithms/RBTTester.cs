using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParallelAlgorithms
{
    internal interface IUniqueContainer<T>
    {
        void Add(T x);
        void Remove(T x);
        bool Contains(T x);
    }

    public class BuiltInSet<T> : IUniqueContainer<T> where T : IComparable
    {
        private readonly SortedSet<T> _ss = new SortedSet<T>();

        public void Add(T x)
        {
            _ss.Add(x);
        }

        public void Remove(T x)
        {
            _ss.Remove(x);
        }

        public bool Contains(T x)
        {
            return _ss.Contains(x);
        }
    }

    internal class RbtTester
    {
        public enum RbtComand
        {
            Insert,
            Erase,
            Find
        }

        private readonly Random _rnd;

        public RbtTester()
        {
            Random seed = new Random();
            int t = seed.Next();
            //Console.WriteLine("Seed = " + t);
            _rnd = new Random(t);
        }

        public RbtTester(int seed)
        {
            _rnd = new Random(seed);
        }

        private void InsertAll(int[] a, RbtSimple<int> r)
        {
            foreach (int i in a)
            {
                r.Add(i);
            }
        }

        private void EraseAll(int[] a, RbtSimple<int> r)
        {
            foreach (int i in a)
            {
                r.Remove(i);
            }
        }

        public void TesterInsertThanEraseTimeOnly()
        {
            const int n = 1000000;
            Console.Write("size: ");
            Console.WriteLine(n);
            int[] a = new int[n];
            for (int i = 0; i < n; i++)
            {
                a[i] = _rnd.Next();
            }

            RbtSimple<int> r = new RbtSimple<int>();
            Program.CountTime(() => InsertAll(a, r), "RBT Add");
            a = a.OrderBy(item => _rnd.Next()).ToArray();
            Program.CountTime(() => EraseAll(a, r), "RBT Remove");
        }

        public Tuple<RbtComand, int>[] GetComands(int operations, int initialSize, double insertProbability,
            double eraseProbability, double findProbabity, int maxNum = int.MaxValue)
        {
            const double probEps = 1e-5;
            if (Math.Abs(insertProbability + eraseProbability + findProbabity - 1) > probEps)
            {
                throw new ArgumentException("Probabilities broken");
            }
//            Console.WriteLine("Operations: " + operations);
//            Console.WriteLine("Initial size: " + initialSize);

            Tuple<RbtComand, int>[] comands = new Tuple<RbtComand, int>[operations + initialSize];
            for (int i = 0; i < initialSize; i++)
            {
                comands[i] = new Tuple<RbtComand, int>(RbtComand.Insert, _rnd.Next() % maxNum);
            }
            for (int i = initialSize; i < initialSize + operations; i++)
            {
                RbtComand cmd = _rnd.NextDouble() < insertProbability
                    ? RbtComand.Insert
                    : (_rnd.NextDouble() < eraseProbability
                        ? RbtComand.Erase
                        : RbtComand.Find
                    );
                comands[i] = new Tuple<RbtComand, int>(cmd, _rnd.Next() % maxNum);
            }
            return comands;
        }

        public Tuple<RbtComand, int>[] GetSimpleComands(int operations = 10000, int maxnum = int.MaxValue)
        {
            return GetComands(operations, operations / 10, 1.0 / 3, 1.0 / 3, 1.0 / 3, maxnum = maxnum);
        }

        public void TestCorrectnessAuto()
        {
            BstFineGrained<int> r = new BstFineGrained<int>();
            SortedSet<int> s = new SortedSet<int>();
            Tuple<RbtComand, int>[] comands = GetSimpleComands(1000000);
            int number = -1;
            foreach (Tuple<RbtComand, int> cmd in comands)
            {
                number++;
                switch (cmd.Item1)
                {
                    case RbtComand.Insert:
                        r.Add(cmd.Item2);
                        s.Add(cmd.Item2);
                        break;
                    case RbtComand.Erase:
                        r.Remove(cmd.Item2);
                        s.Remove(cmd.Item2);
                        break;
                    case RbtComand.Find:
                        if (r.Contains(cmd.Item2) != s.Contains(cmd.Item2))
                        {
                            Console.WriteLine("This is not good, ok?");
                        }
                        break;
                }
            }
        }

        public void TestCorrectnessManually()
        {
            RbtSimple<int> r = new RbtSimple<int>();
            r.Add(0);
            r.Add(6);
            r.Add(4);
            r.Remove(0);
            r.Print();
        }

        private void DoOperation(IUniqueContainer<int> r, Tuple<RbtComand, int> cmd)
        {
            switch (cmd.Item1)
            {
                case RbtComand.Insert:
                    r.Add(cmd.Item2);
                    break;
                case RbtComand.Erase:
                    r.Remove(cmd.Item2);
                    break;
                case RbtComand.Find:
                    r.Contains(cmd.Item2);
                    break;
            }
        }

        public void DoOperations(IUniqueContainer<int> r, Tuple<RbtComand, int>[] comands)
        {
            foreach (Tuple<RbtComand, int> cmd in comands)
            {
                DoOperation(r, cmd);
            }
        }

        public void TestTime()
        {
            Tuple<RbtComand, int>[] comands = GetSimpleComands(1000000);
            Program.CountTime(() => DoOperations(new BuiltInSet<int>(), comands), "BuiltInSet");
            Program.CountTime(() => DoOperations(new RbtSimple<int>(), comands), "RBTSimple");
        }

        public List<Tuple<RbtComand, int>[]> SplitToSublists(Tuple<RbtComand, int>[] source)
        {
            int n = 4;
            return source
                .Select((s, i) => new {Value = s, Index = i})
                .GroupBy(x => x.Index / n)
                .Select(grp => grp.Select(x => x.Value).ToArray())
                .ToList();
        }

        private void CheckWithSs<T>(ConcurrentBuiltInSetRough<T> b, BstFineGrained<T> a) where T : IComparable<T>
        {
            int s1 = b.Ss.Count;
            int s2 = a.Sizelineartime();
            bool correct = true;
            if (s1 != s2)
            {
                correct = false;
                Console.WriteLine("bad sizes");
                Console.WriteLine(s1);
                Console.WriteLine(s2);
            }
            foreach (T i in b.Ss)
            {
                if (!a.Contains(i))
                {
                    Console.WriteLine("doesn't contain element " + i);

                    correct = false;
                }
            }
            if (correct)
            {
                Console.WriteLine("Correct test");
            }
        }

        public void TestCorrectnessSync()
        {
            int to = 50000;
            Tuple<RbtComand, int>[] comands = GetSimpleComands(to);
            BstFineGrained<int> a = new BstFineGrained<int>();
            ConcurrentBuiltInSetRough<int> b = new ConcurrentBuiltInSetRough<int>();
            Parallel.ForEach(comands, t => DoOperation(b, t));
            Parallel.ForEach(comands, t => DoOperation(a, t));
            CheckWithSs(b, a);
        }

        public void TestAddEraseParallel()
        {
            int to = 50000;
            Tuple<RbtComand, int>[] insertions = GetComands(to, 0, 1, 0, 0);
            Tuple<RbtComand, int>[] deletioins = GetComands(to, 0, 0, 1, 0);
            BstFineGrained<int> a = new BstFineGrained<int>();
            Parallel.Invoke(() => DoOperations(a, insertions), () => DoOperations(a, deletioins));
        }

        public void TestOnlyOneComandForEach()
        {
            int to = 500000;
            int maxnum = (int) (1.5 * to);
            Tuple<RbtComand, int>[] insertions = GetComands(to, 0, 1, 0, 0, maxnum);
            Tuple<RbtComand, int>[] finds = GetComands(to, 0, 0, 0, 1, maxnum);
            Tuple<RbtComand, int>[] deletions = GetComands(to, 0, 0, 1, 0, maxnum);
            BstFineGrained<int> a = new BstFineGrained<int>();
            ConcurrentBuiltInSetRough<int> b = new ConcurrentBuiltInSetRough<int>();
            TestComandsWithTrees(insertions, a, b, "Only insertions");
            TestComandsWithTrees(finds, a, b, "Only finds");
            TestComandsWithTrees(deletions, a, b, "Only deletions");
        }

        public void TestComandsWithTrees(Tuple<RbtComand, int>[] comands,
            BstFineGrained<int> a, ConcurrentBuiltInSetRough<int> b, string testcase = "Some test case")
        {
            Console.WriteLine("[" + testcase + "]");
            Program.CountTime(() => Parallel.ForEach(comands, t => DoOperation(b, t)), "ConcurrentBuiltInSetRough");
            Program.CountTime(() => Parallel.ForEach(comands, t => DoOperation(a, t)), "BSTFineGrained");
            Program.CountTime(() => DoOperations(a, comands), "SecuentialBST");
            Console.WriteLine();
        }

        public void TestComands(Tuple<RbtComand, int>[] comands, string testcase = "Some test case")
        {
            TestComandsWithTrees(comands, new BstFineGrained<int>(), new ConcurrentBuiltInSetRough<int>(), testcase);
        }


        public void TestTimeSync()
        {
            TestComands(GetSimpleComands(500000), "All comands");
        }
    }
}