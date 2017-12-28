using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ParallelAlgorithms
{
    public class Register<T>
    {
        public bool[] P;
        public T[] Snapshots;
        public bool Toggle;
        public T Val;

        public Register(int registerCount)
        {
            Toggle = false;
            P = new bool[registerCount];
            Snapshots = new T[registerCount];
        }
    }

    public class AtomicSnapshots<T>
    {
        private readonly Dictionary<TimeSpan, T[]> _logRead = new Dictionary<TimeSpan, T[]>();
        private readonly Dictionary<TimeSpan, T>[] _logWrite;

        private readonly int[] _moved;
        private readonly bool[,] _q;
        private readonly Register<T>[] _registers;

        private readonly Stopwatch _timer = new Stopwatch();

        public AtomicSnapshots(int registersCount)
        {
            _registers = new Register<T>[registersCount];
            _moved = new int[registersCount];
            _logWrite = new Dictionary<TimeSpan, T>[registersCount];
            _q = new bool[registersCount, registersCount];
            for (int i = 0; i < registersCount; i++)
            {
                _logWrite[i] = new Dictionary<TimeSpan, T>();
                _registers[i] = new Register<T>(registersCount);
            }
            _timer.Start();
        }

        public T[] Scan(int i, bool snapOnly = true)
        {
            Array.Clear(_moved, 0, _moved.Length);
            while (true)
            {
                for (int j = 0; j < _registers.Length; j++)
                {
                    _q[i, j] = _registers[j].P[i];
                }

                Register<T>[] a = (Register<T>[]) _registers.Clone();
                Register<T>[] b = (Register<T>[]) _registers.Clone();
                bool ok = true;
                for (int j = 0; j < _registers.Length; j++)
                {
                    if (!(
                        a[j].P[i] == b[j].P[i] &&
                        b[j].P[i] == _q[i, j] &&
                        a[j].Toggle == b[j].Toggle
                    ))
                    {
                        ok = false;
                    }
                }
                if (ok)
                {
                    T[] result = new T[_registers.Length];
                    for (int j = 0; j < _registers.Length; j++)
                    {
                        result[j] = b[j].Val;
                    }
                    if (snapOnly)
                    {
                        _logRead.Add(_timer.Elapsed, result);
                    }
                    return result;
                }
                for (int j = 0; j < _registers.Length; j++)
                {
                    if (
                        a[j].P[i] != _q[i, j] ||
                        b[j].P[i] != _q[i, j] ||
                        a[j].Toggle != b[j].Toggle
                    )
                    {
                        if (_moved[j] == 1)
                        {
                            if (snapOnly)
                            {
                                _logRead.Add(_timer.Elapsed, b[j].Snapshots);
                            }
                            return b[j].Snapshots;
                        }
                        _moved[j]++;
                    }
                }
            }
        }

        public void Update(int i, T val)
        {
            bool[] f = new bool[_registers.Length];
            for (int j = 0; j < _registers.Length; j++)
            {
                f[j] = !_q[j, i];
            }
            _registers[i].Snapshots = Scan(i, false);
            _registers[i].Val = val;
            _registers[i].P = f;
            _registers[i].Toggle = !_registers[i].Toggle;
            _logWrite[i].Add(_timer.Elapsed, val);
        }

        public void Print()
        {
            for (int i = 0; i < _registers.Length; i++)
            {
                Console.WriteLine("\nLogs of register {0}:", i);
                Console.WriteLine("({0}, [{1}], {2}, [{3}])", _registers[i].Val, string.Join(",", _registers[i].P),
                    _registers[i].Toggle, string.Join(",", _registers[i].Snapshots));
                foreach (KeyValuePair<TimeSpan, T> change in _logWrite[i])
                {
                    Console.WriteLine("val: {0}, time: {1}", change.Value, change.Key.Ticks);
                }
            }
            Console.WriteLine("\nReadLog:");
            foreach (KeyValuePair<TimeSpan, T[]> scan in _logRead)
            {
                Console.WriteLine("vals: ({0}), time: {1}", string.Join(", ", scan.Value), scan.Key.Ticks);
            }
        }

        public static void Test()
        {
            AtomicSnapshots<int> atomicSnapshots = new AtomicSnapshots<int>(2);
            Random rnd = new Random();
            Task[] tasks = new Task[2];

            for (int i = 0; i < 30; i++)
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
                    int count = i;
                    Task.Run(() =>
                    {
                        Console.WriteLine("Read thread: {0}, iteration: {1}, regs: ({2})", id, count,
                            string.Join(", ", atomicSnapshots.Scan(id)));
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