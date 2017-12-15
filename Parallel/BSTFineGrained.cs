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

        public SNode<T> child(bool needInRight)
        {
            return needInRight ? left : right;
        }
        public void assign(bool needInRight, SNode<T> sn)
        {
            if (needInRight)
            {
                right = sn;
            }
            else
            {
                left = sn;
            }
        }
    }
    class BSTFineGrained<T> : IUniqueContainer<T> where T: IComparable<T>
    {
        private SNode<T> root;

        public void Add(T x)
        {
            SNode<T> pred = null, curr = root;
            if (root == null)
            {
                root = new SNode<T>(x);
            }
            else
            {
                myenter(curr);
                try
                {
                    bool needInRight = false;
                    while (curr != null && curr.key.CompareTo(x) != 0)
                    {
                        needInRight = curr.key.CompareTo(x) < 0;
                        myexit(pred);
                        pred = curr;
                        if (needInRight)
                            curr = curr.right;
                        else
                            curr = curr.left;
                        myenter(curr);
                    }
                    if (curr == null)
                        pred.assign(pred.key.CompareTo(x) < 0, new SNode<T>(x));
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
                myenter(curr);
                try
                {
                    bool needInRight = false;
                    while (curr != null && curr.key.CompareTo(x) != 0)
                    {
                        needInRight = curr.key.CompareTo(x) < 0;
                        if (pred != null)
                            myexit(pred);
                        pred = curr;
                        if (needInRight)
                            curr = curr.right;
                        else
                            curr = curr.left;
                        myenter(curr);
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
                                pred.assign(needInRight, curr.left);
                        else
                            if (pred == null)
                                root = curr.right;
                            else
                                pred.assign(needInRight, curr.right);
                    else
                    {
                        SNode<T> thepred = null, thecur = curr.right;
                        myenter(thecur);
                        myenter(thepred);
                        try
                        {
                            while (thecur.left != null)
                            {
                                myexit(thepred);
                                thepred = thecur;
                                thecur = thecur.left;
                                myenter(thecur);
                            }
                            curr.key = thecur.key;
                            if (thepred == null)
                                curr.right = thecur.right;
                            else
                                thepred.left = thecur.right;
                            int a = 5;
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
                    myexit(root);
                }
            }
        }

        public bool Contains(T x)
        {
            bool was = false;
            SNode<T> pred = null, curr = root;
            myenter(curr);
            try
            {
                bool needInRight = false;
                while (curr != null && curr.key.CompareTo(x) != 0)
                {
                    needInRight = curr.key.CompareTo(x) < 0;
                    if (pred != null)
                        myexit(pred);
                    pred = curr;
                    if (needInRight)
                        curr = curr.right;
                    else
                        curr = curr.left;
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
                Console.Write("n");
            else
            {
                //Console.Write("d: ");
                Console.Write(t.key);
                if (t.left != null)
                {
                    Console.Write(", l: ");
                    print(t.left, false);
                }
                if (t.right != null)
                {
                    Console.Write(", r: ");
                    print(t.right, false);
                }
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
            WaitForThis();
            ss.Add(x);
            ReleaseThis();
        }

        public void Remove(T x)
        {
            WaitForThis();
            ss.Remove(x);
            ReleaseThis();
        }

        public bool Contains(T x)
        {
            WaitForThis();
            bool res = ss.Contains(x);
            ReleaseThis();
            return res;
        }

        private void WaitForThis()
        {
            Monitor.Enter(ss);
        }

        private void ReleaseThis()
        {
            Monitor.Exit(ss);
        }
    }


    class BSTWithoutMutex<T> : IUniqueContainer<T> where T : IComparable<T>
    {
        private SNode<T> root;

        public void Add(T x)
        {
            SNode<T> pred = null, curr = root;
            if (root == null)
            {
                root = new SNode<T>(x);
            }
            else
            {
                    bool needInRight = false;
                    while (curr != null && curr.key.CompareTo(x) != 0)
                    {
                        needInRight = curr.key.CompareTo(x) < 0;
                        pred = curr;
                        if (needInRight)
                            curr = curr.right;
                        else
                            curr = curr.left;
                    }
                    if (curr == null)
                        pred.assign(pred.key.CompareTo(x) < 0, new SNode<T>(x));
            }
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
                    bool needInRight = false;
                    while (curr != null && curr.key.CompareTo(x) != 0)
                    {
                        needInRight = curr.key.CompareTo(x) < 0;
                        pred = curr;
                        if (needInRight)
                            curr = curr.right;
                        else
                            curr = curr.left;
                    }
                    if (curr == null)
                        return;
                    ;
                    if (curr.left == null || curr.right == null)
                        if (curr.right == null)
                            if (pred == null)
                                root = curr.left;
                            else
                                pred.assign(needInRight, curr.left);
                        else
                            if (pred == null)
                            root = curr.right;
                        else
                            pred.assign(needInRight, curr.right);
                    else
                    {
                        SNode<T> thepred = null, thecur = curr.right;
                            while (thecur.left != null)
                            {
                                thepred = thecur;
                                thecur = thecur.left;
                            }
                            curr.key = thecur.key;
                            if (thepred == null)
                                curr.right = thecur.right;
                            else
                                thepred.left = thecur.right;
                            int a = 5;
                    }

            }
        }

        public bool Contains(T x)
        {
            bool was = false;
            SNode<T> pred = null, curr = root;
                bool needInRight = false;
                while (curr != null && curr.key.CompareTo(x) != 0)
                {
                    needInRight = curr.key.CompareTo(x) < 0;
                    pred = curr;
                    if (needInRight)
                        curr = curr.right;
                    else
                        curr = curr.left;
                }
                if (curr != null && curr.key.CompareTo(x) == 0)
                    was = true;
            return was;
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
                Console.Write("n");
            else
            {
                //Console.Write("d: ");
                Console.Write(t.key);
                if (t.left != null)
                {
                    Console.Write(", l: ");
                    print(t.left, false);
                }
                if (t.right != null)
                {
                    Console.Write(", r: ");
                    print(t.right, false);
                }
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
}
