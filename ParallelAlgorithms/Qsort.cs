using System;
using System.Threading.Tasks;

namespace ParallelAlgorithms
{
    internal class Qsort
    {
        private const int Tplen = 1;
        private int _arlen = 10000;

        public int[] GenerateIntArray(int n)
        {
            Random r = new Random();
            int[] a = new int[n];
            for (int i = 0; i < n; i++)
            {
                a[i] = r.Next() % 15;
            }
            return a;
        }

        private void PrintArray(int[] a)
        {
            foreach (int t in a)
            {
                Console.Write(t + " ");
            }
            Console.WriteLine();
        }

        private void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        private int Partition<T>(T[] a, int l, int r) where T : IComparable<T>
        {
            int m = (l + r) / 2;
            T pivot = a[m];
            int j = l;
            for (int i = l; i <= r; i++)
            {
                if (a[i].CompareTo(pivot) <= 0)
                {
                    Swap(ref a[i], ref a[j]);
                    if (i == m)
                    {
                        m = j;
                    }
                    else if (j == m)
                    {
                        m = i;
                    }
                    j++;
                }
            }
            if (a[j - 1].CompareTo(a[m]) <= 0)
            {
                Swap(ref a[j - 1], ref a[m]);
            }
            return j - 1;
        }

        public void QsortThreaded<T>(T[] a, int l, int r) where T : IComparable<T>
        {
            if (l >= r)
            {
                return;
            }
            int someThisConstant = 500;
            if (r - l > someThisConstant)
            {
                QsortSimple(a, l, r);
                return;
            }
            int i = Partition(a, l, r);
            Task ltTask = Task.Run(() => QsortThreaded(a, l, i - 1));
            Task rtTask = Task.Run(() => QsortThreaded(a, i + 1, r));
            Task.WaitAll(ltTask, rtTask);
            //            Parallel.Invoke(
            //                ()=>QsortThreaded(a, l, i - 1),
            //                ()=>QsortThreaded(a, i + 1, r)
            //            );
        }

        public void QsortSimple<T>(T[] a, int l, int r) where T : IComparable<T>
        {
            if (l >= r)
            {
                return;
            }
            int i = Partition(a, l, r);
            QsortSimple(a, l, i - 1);
            QsortSimple(a, i + 1, r);
        }

        private void LaunchSomeSort(int[] a)
        {
        }

        public void Test(int len = 50000)
        {
            _arlen = len;
            int[] a = GenerateIntArray(_arlen);
            Console.WriteLine("QsortArrayLen = " + a.Length);

            int[] b;

            b = (int[]) a.Clone();
            Program.CountTime(() => QsortSimple(b, 0, b.Length - 1), "Qsimple");
            b = (int[]) a.Clone();
            Program.CountTime(() => QsortThreaded(b, 0, b.Length - 1), "Qthreaded");
        }


        private delegate void SomeDelegate();
    }
}