using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyParallel;

namespace MyParallel
{
    interface IUniqueContainer<T>
    {
        void Add(T x);
        void Remove(T x);
        bool Contains(T x);
    }

    public class BuiltInSet<T> : IUniqueContainer<T> where T : IComparable
    {
        private SortedSet<T> ss = new SortedSet<T>();
        public void Add(T x)
        {
            ss.Add(x);
        }

        public void Remove(T x)
        {
            ss.Remove(x);
        }

        public bool Contains(T x)
        {
            return ss.Contains(x);
        }
    }

    class RBTTester
    {
        public RBTTester()
        {
            rnd = new Random();
        }

        public RBTTester(int seed)
        {
            rnd = new Random(seed);
        }

        Random rnd;
        void InsertAll(int[] a, RBTSimple<int> r)
        {
            foreach (var i in a)
                r.Add(i);
        }

        void EraseAll(int[] a, RBTSimple<int> r)
        {
            foreach (var i in a)
                r.Remove(i);
        }
        public void TesterInsertThanEraseTimeOnly()
        {
            const int n = 1000000;
            Console.Write("size: ");
            Console.WriteLine(n);
            int[] a = new int[n];
            for (int i = 0; i < n; i++)
                a[i] = rnd.Next();

            RBTSimple<int> r = new RBTSimple<int>();
            Program.CountTime(() => InsertAll(a, r), "RBT Add");
            a = a.OrderBy(item => rnd.Next()).ToArray();
            Program.CountTime(() => EraseAll(a, r), "RBT Remove");

        }

        public enum RBTComand { INSERT, ERASE, FIND }

        public Tuple<RBTComand, int>[] GetComands(int operations, int initialSize, double insertProbability, double eraseProbability, double findProbabity, int maxNum = Int32.MaxValue)
        {

            const double probEps = 1e-5;
            if (Math.Abs(insertProbability + eraseProbability + findProbabity - 1) > probEps)
                throw new ArgumentException("Probabilities broken");
            Console.WriteLine("Operations: " + operations);
            Console.WriteLine("Initial size: " + initialSize);

            Tuple<RBTComand, int>[] comands = new Tuple<RBTComand, int>[operations + initialSize];
            for (int i = 0; i < initialSize; i++)
                comands[i] = new Tuple<RBTComand, int>(RBTComand.INSERT, rnd.Next() % maxNum);
            for (int i = initialSize; i < initialSize + operations; i++)
            {
                var cmd = rnd.NextDouble() < insertProbability
                    ? RBTComand.INSERT
                    : (rnd.NextDouble() < eraseProbability
                        ? RBTComand.ERASE
                        : RBTComand.FIND
                    );
                comands[i] = new Tuple<RBTComand, int>(cmd, rnd.Next() % maxNum);
            }
            return comands;
        }

        public Tuple<RBTComand, int>[] GetSimpleComands(int operations = 10000)
        {
            return GetComands(operations, operations / 10, 1.0 / 3, 1.0 / 3, 1.0 / 3);
        }

        public void TestCorrectnessAuto()
        {
            BSTFineGrained<int> r = new BSTFineGrained<int>();
            SortedSet<int> s = new SortedSet<int>();
            var comands = GetSimpleComands();
            foreach (var cmd in comands)
            {
                switch (cmd.Item1)
                {
                    case RBTComand.INSERT:
                        r.Add(cmd.Item2);
                        s.Add(cmd.Item2);
                        break;
                    case RBTComand.ERASE:
                        r.Remove(cmd.Item2);
                        s.Remove(cmd.Item2);
                        break;
                    case RBTComand.FIND:
                        if (r.Contains(cmd.Item2) != s.Contains(cmd.Item2))
                            Console.WriteLine("This is not good, ok?");
                        break;
                }
            }
        }

        public void TestCorrectnessManually()
        {
            RBTSimple<int> r = new RBTSimple<int>();
            r.Add(0);
            r.Add(6);
            r.Add(4);
            r.Remove(0);
            r.print();
        }

        private void DoOperation(IUniqueContainer<int> r, Tuple<RBTComand, int> cmd)
        {
            switch (cmd.Item1)
            {
                case RBTComand.INSERT:
                    r.Add(cmd.Item2);
                    break;
                case RBTComand.ERASE:
                    r.Remove(cmd.Item2);
                    break;
                case RBTComand.FIND:
                    r.Contains(cmd.Item2);
                    break;
            }
        }

        public void DoOperations(IUniqueContainer<int> r, Tuple<RBTComand, int>[] comands)
        {
            foreach (var cmd in comands)
            {
                DoOperation(r, cmd);
            }
        }

        public void TestTime()
        {
            var comands = GetSimpleComands(1000000);
            Program.CountTime(() => DoOperations(new BuiltInSet<int>(), comands), "BuiltInSet");
            Program.CountTime(() => DoOperations(new RBTSimple<int>(), comands), "RBTSimple");
        }

        public List<Tuple<RBTComand, int>[]> SplitToSublists(Tuple<RBTComand, int>[] source)
        {
            int n = 4;
            return source
                .Select((s, i) => new { Value = s, Index = i })
                .GroupBy(x => x.Index / n)
                .Select(grp => grp.Select(x => x.Value).ToArray())
                .ToList();
        }

        private void CheckWithSS<T>(ConcurrentBuiltInSetRough<T> b, BSTFineGrained<T> a) where T: IComparable<T>
        {
            int s1 = b.ss.Count;
            int s2 = a.sizelineartime();
            bool correct = true;
            if (s1 != s2)
            {
                correct = false;
                Console.WriteLine("bad sizes");
                Console.WriteLine(s1);
                Console.WriteLine(s2);
            }
            foreach (T i in b.ss)
            {
                if (!a.Contains(i))
                {
                    Console.WriteLine("doesn't contain element " + i);

                    correct = false;
                }
            }
            if (correct)
                Console.WriteLine("Correct test");
        }

        public void TestCorrectnessSync()
        {
            int to = 50000;
            var comands = GetSimpleComands(to);
            BSTFineGrained<int> a = new BSTFineGrained<int>();
            ConcurrentBuiltInSetRough<int> b = new ConcurrentBuiltInSetRough<int>();
            Parallel.ForEach(comands, t => DoOperation(b, t));
            Parallel.ForEach(comands, t => DoOperation(a, t));
            CheckWithSS(b, a);
        }

        public void TestAddEraseParallel()
        {
            int to = 50000;
            var insertions = GetComands(to, 0, 1, 0, 0);
            var deletioins = GetComands(to, 0, 0, 1, 0);
            BSTFineGrained<int> a = new BSTFineGrained<int>();
            Parallel.Invoke(()=>DoOperations(a, insertions), ()=>DoOperations(a, deletioins));
        }


        public void TestTimeSync()
        {
            int to = 500000;
            var comands = GetSimpleComands(to);
            var blocks = SplitToSublists(comands);
            IUniqueContainer<int> a = new BSTFineGrained<int>();
            IUniqueContainer<int> b = new ConcurrentBuiltInSetRough<int>();
            Program.CountTime(() => Parallel.ForEach(comands, t => DoOperation(b, t)), "ConcurrentBuiltInSetRough");
            Program.CountTime(() => Parallel.ForEach(comands, t => DoOperation(a, t)), "BSTFineGrained");
            Program.CountTime(() => DoOperations(a, comands), "SecuentialBST");
        }
    }
}
