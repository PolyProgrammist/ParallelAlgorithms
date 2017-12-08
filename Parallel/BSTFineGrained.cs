using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyParallel
{
    public class SNode<T> where T : IComparable<T>
    {
        public T key;
        public SNode<T> left = null, right = null;
        public SNode (T _key)
        {
            key = _key;
        }

        public SNode<T> child(bool l)
        {
            return l ? left : right;
        }
        public void assign(bool l, SNode<T> sn)
        {
            if (l)
            {
                left = sn;
            }
            else
            {
                right = sn;
            }
        }
    }
    class BSTFineGrained<T> : IUniqueContainer<T> where T: IComparable<T>
    {
        private SNode<T> root;

        public void Add(T x)
        {
            Monitor.Enter(this);
            myenter(root);
            Monitor.Exit(this);
            SNode<T> pred = null, curr = root;
            if (root == null)
            {
                Monitor.Enter(this);
                root = new SNode<T>(x);
                Monitor.Exit(this);
            }
            else
            {
                myenter(curr);
                try
                {
                    bool less = false;
                    while (curr != null && curr.key.CompareTo(x) != 0)
                    {
                        if (pred == null)
                            less = curr.key.CompareTo(x) < 0;
                        else
                            less = pred.key.CompareTo(x) < 0;
                        myexit(pred);
                        pred = curr;
                        if (less)
                            curr = curr.left;
                        else
                            curr = curr.right;
                        myenter(curr);
                    }
                    if (curr == null)
                        pred.assign(less, new SNode<T>(x));
                }
                finally
                {
                    myexit(pred);
                    myexit(curr);
                }
            }
            myexit(root);
        }

        public void Remove(T x)
        {
            SNode<T> pred = null, curr = root;
            if (root == null)
            {
                return;
            }
            else
            {
                Monitor.Enter(this);
                myenter(curr);
                Monitor.Exit(this);
                try
                {
                    bool less = false;
                    while (curr != null && curr.key.CompareTo(x) != 0)
                    {
                        if (pred == null)
                            less = curr.key.CompareTo(x) < 0;
                        else
                            less = pred.key.CompareTo(x) < 0;
                        if (pred != null)
                            myexit(pred);
                        pred = curr;
                        if (less)
                            curr = curr.left;
                        else
                            curr = curr.right;
                        myenter(curr);
                        myexit(pred);
                    }
                    if (curr == null)
                        return;
                    ;
                    myenter(curr.left);
                    myenter(curr.right);
                    if (curr.left == null || curr.right == null)
                        if (curr.right == null)
                            if (pred == null)
                                root = curr.left;
                            else
                                pred = curr.left;
                        else
                            if (pred == null)
                                root = curr.right;
                            else
                                pred = curr.right;
                    else
                    {
                        SNode<T> thepred = curr, thecur = curr.right;
                        myenter(thecur);
                        myenter(thepred);
                        try
                        {
                            while (thecur.left != null)
                            {
                                myexit(thepred);
                                thepred = thecur;
                                thecur = thecur.left;
                                myenter(thepred);
                                myenter(thecur);
                            }
                            pred.assign(less, thecur);
                            thepred.left = null;
                            thecur.left = curr.left;
                        }
                        finally
                        {
                            myexit(thepred);
                            myexit(thecur);
                        }
                    }

                }
                finally
                {
                    myexit(pred);
                    myexit(curr);
                    if (curr != null)
                    {
                        myexit(curr.left);
                        myexit(curr.right);
                    }
                }
            }
        }

        public bool Contains(T x)
        {
            bool was = false;
            SNode<T> pred = null, curr = root;
            Monitor.Enter(this);
            myenter(curr);
            Monitor.Exit(this);
            try
            {
                bool less = false;
                while (curr != null && curr.key.CompareTo(x) != 0)
                {
                    if (pred == null)
                        less = curr.key.CompareTo(x) < 0;
                    else
                        less = pred.key.CompareTo(x) < 0;
                    if (pred != null)
                        myexit(pred);
                    pred = curr;
                    if (less)
                        curr = curr.left;
                    else
                        curr = curr.right;
                    myenter(curr);
                }
                if (curr != null && curr.key.CompareTo(x) == 0)
                    was = true;
            }
            finally
            {
                myexit(pred);
                myexit(curr);
            }
            return was;
        }

        private void myenter(SNode<T> x)
        {
            if (x != null)
                Monitor.Enter(x);
        }

        private void myexit(SNode<T> x)
        {
            if (x != null && Monitor.IsEntered(x))
                Monitor.Exit(x);
        }

        private void assign(SNode<T> x, SNode<T> y)
        {
            if (x == null)
                root = y;
            else
                x = y;
        }

        void print(SNode<T> t, bool en = true)
        {
            Console.Write('(');
            if (t == null)
                Console.Write("null");
            else
            {
                Console.Write("d: ");
                Console.Write(t.key);
                Console.Write(", l: ");
                print(t.left, false);
                Console.Write(", r: ");
                print(t.right, false);
            }
            Console.Write(')');
            if (en)
                Console.WriteLine();
        }

        public void print()
        {
            print(root);
        }

        public int sizelineartime()
        {
            return badsize(root);
        }

        private int badsize(SNode<T> t)
        {
            if (t == null)
                return 0;
            return badsize(t.left) + 1 + badsize(t.right);
        }
    }

    class ConcurrentBuiltInSetRough<T> : IUniqueContainer<T> where T: IComparable<T>
    {
        public SortedSet<T> ss = new SortedSet<T>();
        public void Add(T x)
        {
            Monitor.Enter(ss);
            ss.Add(x);
            Monitor.Exit(ss);
        }

        public void Remove(T x)
        {
            Monitor.Enter(ss);
            ss.Remove(x);
            Monitor.Exit(ss);
        }

        public bool Contains(T x)
        {
            Monitor.Enter(ss);
            bool res = ss.Contains(x);
            Monitor.Exit(ss);
            return res;
        }
    }
}
