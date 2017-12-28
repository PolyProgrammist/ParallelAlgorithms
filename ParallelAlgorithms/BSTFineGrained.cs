using System;
using System.Collections.Generic;
using System.Threading;

namespace ParallelAlgorithms
{
    public class SNode<T> where T : IComparable<T>
    {
        public T Key;
        public SNode<T> Left, Right;

        public SNode(T key)
        {
            Key = key;
        }

        public SNode<T> Child(bool needInRight)
        {
            return needInRight ? Left : Right;
        }

        public void Assign(bool needInRight, SNode<T> sn)
        {
            if (needInRight)
            {
                Right = sn;
            }
            else
            {
                Left = sn;
            }
        }
    }

    internal class BstFineGrained<T> : IUniqueContainer<T> where T : IComparable<T>
    {
        private SNode<T> _root;

        public void Add(T x)
        {
            SNode<T> pred = null, curr = _root;
            if (_root == null)
            {
                _root = new SNode<T>(x);
            }
            else
            {
                Myenter(curr);
                try
                {
                    bool needInRight = false;
                    while (curr != null && curr.Key.CompareTo(x) != 0)
                    {
                        needInRight = curr.Key.CompareTo(x) < 0;
                        Myexit(pred);
                        pred = curr;
                        if (needInRight)
                        {
                            curr = curr.Right;
                        }
                        else
                        {
                            curr = curr.Left;
                        }
                        Myenter(curr);
                    }
                    if (curr == null)
                    {
                        pred.Assign(pred.Key.CompareTo(x) < 0, new SNode<T>(x));
                    }
                }
                finally
                {
                    Myexit(pred);
                    Myexit(curr);
                }
            }
            Myexit(_root);
        }

        public void Remove(T x)
        {
            SNode<T> pred = null, curr = _root;
            if (_root == null)
            {
            }
            else
            {
                Myenter(curr);
                try
                {
                    bool needInRight = false;
                    while (curr != null && curr.Key.CompareTo(x) != 0)
                    {
                        needInRight = curr.Key.CompareTo(x) < 0;
                        if (pred != null)
                        {
                            Myexit(pred);
                        }
                        pred = curr;
                        if (needInRight)
                        {
                            curr = curr.Right;
                        }
                        else
                        {
                            curr = curr.Left;
                        }
                        Myenter(curr);
                    }
                    if (curr == null)
                    {
                        return;
                    }
                    ;
                    Myenter(curr.Left);
                    Myenter(curr.Right);
                    if (curr.Left == null || curr.Right == null)
                    {
                        if (curr.Right == null)
                        {
                            if (pred == null)
                            {
                                _root = curr.Left;
                            }
                            else
                            {
                                pred.Assign(needInRight, curr.Left);
                            }
                        }
                        else if (pred == null)
                        {
                            _root = curr.Right;
                        }
                        else
                        {
                            pred.Assign(needInRight, curr.Right);
                        }
                    }
                    else
                    {
                        SNode<T> thepred = null, thecur = curr.Right;
                        Myenter(thecur);
                        Myenter(thepred);
                        try
                        {
                            while (thecur.Left != null)
                            {
                                Myexit(thepred);
                                thepred = thecur;
                                thecur = thecur.Left;
                                Myenter(thecur);
                            }
                            curr.Key = thecur.Key;
                            if (thepred == null)
                            {
                                curr.Right = thecur.Right;
                            }
                            else
                            {
                                thepred.Left = thecur.Right;
                            }
                            int a = 5;
                        }
                        finally
                        {
                            Myexit(thepred);
                            Myexit(thecur);
                        }
                    }
                }
                finally
                {
                    Myexit(pred);
                    Myexit(curr);
                    if (curr != null)
                    {
                        Myexit(curr.Left);
                        Myexit(curr.Right);
                    }
                    Myexit(_root);
                }
            }
        }

        public bool Contains(T x)
        {
            bool was = false;
            SNode<T> pred = null, curr = _root;
            Myenter(curr);
            try
            {
                bool needInRight = false;
                while (curr != null && curr.Key.CompareTo(x) != 0)
                {
                    needInRight = curr.Key.CompareTo(x) < 0;
                    if (pred != null)
                    {
                        Myexit(pred);
                    }
                    pred = curr;
                    if (needInRight)
                    {
                        curr = curr.Right;
                    }
                    else
                    {
                        curr = curr.Left;
                    }
                    Myenter(curr);
                }
                if (curr != null && curr.Key.CompareTo(x) == 0)
                {
                    was = true;
                }
            }
            finally
            {
                Myexit(pred);
                Myexit(curr);
            }
            return was;
        }

        private void Myenter(SNode<T> x)
        {
            if (x != null)
            {
                Monitor.Enter(x);
            }
        }

        private void Myexit(SNode<T> x)
        {
            if (x != null && Monitor.IsEntered(x))
            {
                Monitor.Exit(x);
            }
        }

        private void Assign(SNode<T> x, SNode<T> y)
        {
            if (x == null)
            {
                _root = y;
            }
            else
            {
                x = y;
            }
        }

        private void Print(SNode<T> t, bool en = true)
        {
            Console.Write('(');
            if (t == null)
            {
                Console.Write("n");
            }
            else
            {
                //Console.Write("d: ");
                Console.Write(t.Key);
                if (t.Left != null)
                {
                    Console.Write(", l: ");
                    Print(t.Left, false);
                }
                if (t.Right != null)
                {
                    Console.Write(", r: ");
                    Print(t.Right, false);
                }
            }
            Console.Write(')');
            if (en)
            {
                Console.WriteLine();
            }
        }

        public void Print()
        {
            Print(_root);
        }

        public int Sizelineartime()
        {
            return Badsize(_root);
        }

        private int Badsize(SNode<T> t)
        {
            if (t == null)
            {
                return 0;
            }
            return Badsize(t.Left) + 1 + Badsize(t.Right);
        }
    }

    internal class BstWithoutMutex<T> : IUniqueContainer<T> where T : IComparable<T>
    {
        private SNode<T> _root;

        public void Add(T x)
        {
            SNode<T> pred = null, curr = _root;
            if (_root == null)
            {
                _root = new SNode<T>(x);
            }
            else
            {
                bool needInRight = false;
                while (curr != null && curr.Key.CompareTo(x) != 0)
                {
                    needInRight = curr.Key.CompareTo(x) < 0;
                    pred = curr;
                    if (needInRight)
                    {
                        curr = curr.Right;
                    }
                    else
                    {
                        curr = curr.Left;
                    }
                }
                if (curr == null)
                {
                    pred.Assign(pred.Key.CompareTo(x) < 0, new SNode<T>(x));
                }
            }
        }

        public void Remove(T x)
        {
            SNode<T> pred = null, curr = _root;
            if (_root == null)
            {
            }
            else
            {
                bool needInRight = false;
                while (curr != null && curr.Key.CompareTo(x) != 0)
                {
                    needInRight = curr.Key.CompareTo(x) < 0;
                    pred = curr;
                    if (needInRight)
                    {
                        curr = curr.Right;
                    }
                    else
                    {
                        curr = curr.Left;
                    }
                }
                if (curr == null)
                {
                    return;
                }
                ;
                if (curr.Left == null || curr.Right == null)
                {
                    if (curr.Right == null)
                    {
                        if (pred == null)
                        {
                            _root = curr.Left;
                        }
                        else
                        {
                            pred.Assign(needInRight, curr.Left);
                        }
                    }
                    else if (pred == null)
                    {
                        _root = curr.Right;
                    }
                    else
                    {
                        pred.Assign(needInRight, curr.Right);
                    }
                }
                else
                {
                    SNode<T> thepred = null, thecur = curr.Right;
                    while (thecur.Left != null)
                    {
                        thepred = thecur;
                        thecur = thecur.Left;
                    }
                    curr.Key = thecur.Key;
                    if (thepred == null)
                    {
                        curr.Right = thecur.Right;
                    }
                    else
                    {
                        thepred.Left = thecur.Right;
                    }
                    int a = 5;
                }
            }
        }

        public bool Contains(T x)
        {
            bool was = false;
            SNode<T> pred = null, curr = _root;
            bool needInRight = false;
            while (curr != null && curr.Key.CompareTo(x) != 0)
            {
                needInRight = curr.Key.CompareTo(x) < 0;
                pred = curr;
                if (needInRight)
                {
                    curr = curr.Right;
                }
                else
                {
                    curr = curr.Left;
                }
            }
            if (curr != null && curr.Key.CompareTo(x) == 0)
            {
                was = true;
            }
            return was;
        }

        private void Assign(SNode<T> x, SNode<T> y)
        {
            if (x == null)
            {
                _root = y;
            }
            else
            {
                x = y;
            }
        }

        private void Print(SNode<T> t, bool en = true)
        {
            Console.Write('(');
            if (t == null)
            {
                Console.Write("n");
            }
            else
            {
                //Console.Write("d: ");
                Console.Write(t.Key);
                if (t.Left != null)
                {
                    Console.Write(", l: ");
                    Print(t.Left, false);
                }
                if (t.Right != null)
                {
                    Console.Write(", r: ");
                    Print(t.Right, false);
                }
            }
            Console.Write(')');
            if (en)
            {
                Console.WriteLine();
            }
        }

        public void Print()
        {
            Print(_root);
        }

        public int Sizelineartime()
        {
            return Badsize(_root);
        }

        private int Badsize(SNode<T> t)
        {
            if (t == null)
            {
                return 0;
            }
            return Badsize(t.Left) + 1 + Badsize(t.Right);
        }
    }

    internal class ConcurrentBuiltInSetRough<T> : IUniqueContainer<T> where T : IComparable<T>
    {
        public SortedSet<T> Ss = new SortedSet<T>();

        public void Add(T x)
        {
            WaitForThis();
            Ss.Add(x);
            ReleaseThis();
        }

        public void Remove(T x)
        {
            WaitForThis();
            Ss.Remove(x);
            ReleaseThis();
        }

        public bool Contains(T x)
        {
            WaitForThis();
            bool res = Ss.Contains(x);
            ReleaseThis();
            return res;
        }

        private void WaitForThis()
        {
            Monitor.Enter(Ss);
        }

        private void ReleaseThis()
        {
            Monitor.Exit(Ss);
        }
    }
}