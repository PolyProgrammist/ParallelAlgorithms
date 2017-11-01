using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyParallel
{
    class Qsort
    {
        private const int tplen = 1;
        private int arlen = 10000;

        public Qsort()
        {
        }

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

        private void printArray(int[] a)
        {
            foreach (var t in a)
            {
                Console.Write(t + " ");
            }
            Console.WriteLine();
        }

        void Swap<T>(ref T a, ref T b)
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
                if (a[i].CompareTo(pivot) <= 0)
                {
                    Swap(ref a[i], ref a[j]);
                    if (i == m)
                        m = j;
                    else if (j == m)
                        m = i;
                    j++;
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
                return;
            var SOME_THIS_CONSTANT = 500;
            if (r - l > SOME_THIS_CONSTANT)
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
                return;
            int i = Partition(a, l, r);
            QsortSimple(a, l, i - 1);
            QsortSimple(a, i + 1, r);
        }


        delegate void SomeDelegate();

        private void LaunchSomeSort(int[] A)
        {

        }

        public void Test(int len = 50000)
        {
            arlen = len;
            int[] A = GenerateIntArray(arlen);
            Console.WriteLine("QsortArrayLen = " + A.Length);

            int[] B;

            B = (int[])A.Clone();
            Program.CountTime(() => QsortSimple(B, 0, B.Length - 1), "Qsimple");
            B = (int[])A.Clone();
            Program.CountTime(() => QsortThreaded(B, 0, B.Length - 1), "Qthreaded");
        }
    }
}