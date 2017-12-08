using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyParallel
{
    class InternetSort
    {
        public void MyTest(int[] A)
        {

        }

        static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        static int Partition<T>(T[] items, int left, int right) where T : IComparable<T>
        {
            int pivotPos = (left + right) / 2;
            T pivotValue = items[pivotPos];
            Swap(ref items[right - 1], ref items[pivotPos]);
            int store = left;
            for (int i = left; i < right - 1; ++i)
            {
                if (items[i].CompareTo(pivotValue) < 0)
                {
                    Swap(ref items[i], ref items[store]);
                    ++store;
                }
            }
            Swap(ref items[right - 1], ref items[store]);
            return store;
        }

        public static void QuicksortSequential<T>(T[] arr, int left, int right)
            where T : IComparable<T>
        {
            if (right > left)
            {
                int pivot = Partition(arr, left, right);
                QuicksortSequential(arr, left, pivot - 1);
                QuicksortSequential(arr, pivot + 1, right);
            }
        }

        public static void QuicksortParallelOptimised<T>(T[] arr, int left, int right)
            where T : IComparable<T>
        {
            const int SEQUENTIAL_THRESHOLD = 500;
            if (right > left)
            {
                if (right - left < SEQUENTIAL_THRESHOLD)
                {

                    QuicksortSequential(arr, left, right);
                }
                else
                {
                    int pivot = Partition(arr, left, right);
                    Task ltTask = Task.Run(() => QuicksortParallelOptimised(arr, left, pivot - 1));
                    Task rtTask = Task.Run(() => QuicksortParallelOptimised(arr, pivot + 1, right));
                    Task.WaitAll(ltTask, rtTask);
                    //                    Parallel.Invoke(
                    //                        () => QuicksortParallelOptimised(arr, left, pivot - 1),
                    //                        () => QuicksortParallelOptimised(arr, pivot + 1, right)
                    //                        );
                }
            }
        }
    }
}
