using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ParallelAlgorithms
{
    public class Register<T>
    {
        public T val;
        public bool toggle;
        public bool[] p;
        public T[] snapshots;

        public Register(int registerCount)
        {
            toggle = false;
            p = new bool[registerCount];
            snapshots = new T[registerCount];
        }
    }
    
    public class AtomicSnapshots<T>
    {
        private Register<T>[] registers;

        private int[] moved;
        private bool[,] q;
        
        private Stopwatch timer = new Stopwatch();
        private Dictionary<TimeSpan, T>[] logWrite;
        private Dictionary<TimeSpan, T[]> logRead = new Dictionary<TimeSpan, T[]>();
        
        public AtomicSnapshots(int registersCount)
        {
            registers = new Register<T>[registersCount];
            moved = new int[registersCount];
            logWrite = new Dictionary<TimeSpan, T>[registersCount];
            q = new bool[registersCount, registersCount];
            for (int i = 0; i < registersCount; i++)
            {
                logWrite[i] = new Dictionary<TimeSpan, T>();
                registers[i] = new Register<T>(registersCount);
            }
            timer.Start();
        }

        public T[] Scan(int i, bool snapOnly = true)
        {
            Array.Clear(moved, 0, moved.Length);
            while (true)
            {
                for (int j = 0; j < registers.Length; j++)
                    q[i,j] = registers[j].p[i];
                var a = (Register<T>[]) registers.Clone();
                var b = (Register<T>[]) registers.Clone();
                bool ok = true;
                for (int j = 0; j < registers.Length; j++)
                    if (!(
                        a[j].p[i] == b[j].p[i] &&
                        b[j].p[i] == q[i, j] &&
                        a[j].toggle == b[j].toggle
                    ))
                        ok = false;
                if (ok)
                {
                    var result = new T[registers.Length];
                    for (int j = 0; j < registers.Length; j++)
                        result[j] = b[j].val;
                    if (snapOnly)
                        logRead.Add(timer.Elapsed, result);
                    return result;
                } 
                for (int j = 0; j < registers.Length; j++)
                    if (
                        a[j].p[i] != q[i, j] || 
                        b[j].p[i] != q[i, j] ||
                        a[j].toggle != b[j].toggle
                        )
                    {
                        if (moved[j] == 1)
                        {
                            if (snapOnly)
                                logRead.Add(timer.Elapsed, b[j].snapshots);
                            return b[j].snapshots;
                        }
                        else
                            moved[j]++;
                        
                    }
                
            }
        }

        public void Update(int i, T val)
        {
            var f = new bool[registers.Length];
            for (int j = 0; j < registers.Length; j++)
                f[j] = !q[j, i];
            registers[i].snapshots = Scan(i, false);
            registers[i].val = val;
            registers[i].p = f;
            registers[i].toggle = !registers[i].toggle;
            logWrite[i].Add(timer.Elapsed, val);
        }
        
        public void Print()
        {
            for (var i = 0; i < registers.Length; i++)
            {
                Console.WriteLine("\nLogs of register {0}:", i);
                Console.WriteLine("({0}, [{1}], {2}, [{3}])", registers[i].val, string.Join(",", registers[i].p), 
                    registers[i].toggle, string.Join(",", registers[i].snapshots));
                foreach (var change in logWrite[i])
                {
                    Console.WriteLine("val: {0}, time: {1}", change.Value, change.Key.Ticks);
                }
            }
            Console.WriteLine("\nReadLog:");
            foreach (var scan in logRead)
            {
                Console.WriteLine("vals: ({0}), time: {1}", string.Join(", ", scan.Value), scan.Key.Ticks);
            }  
        }
        
        public static void Test()
        {
            var atomicSnapshots = new AtomicSnapshots<int>(2);
            var rnd = new Random();
            var tasks = new Task[2];

            for (var i = 0; i < 30; i++)
            {
                int id = i % 2;
                int value = rnd.Next(100);
                tasks[id] = Task.Run(() =>
                {
                    Console.WriteLine("Write val: {0}, reg: {1}", value, id);
                    atomicSnapshots.Update(id, value);
                });

                if (i % 3 == 0)
                {
                    var count = i;
                    Task.Run(() =>
                    {
                        Console.WriteLine("Read thread: {0}, iteration: {1}, regs: ({2})", id, count, string.Join(", ", atomicSnapshots.Scan(id)));
                    });
                }

                if (i % 2 == 1)
                {
                    Task.WaitAll(tasks);
                }
            }

            atomicSnapshots.Print();
        }
    }
}