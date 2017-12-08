﻿using System;
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

        public SNode<T> getc(bool l)
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
            SNode<T> pred = null, curr = root;
            if (root == null)
            {
                Monitor.Enter(root);
                root = new SNode<T>(x);
                Monitor.Exit(root);
            }
            else
            {
                Monitor.Enter(curr);
                try
                {
                    bool less = false;
                    while (curr != null || curr.key.CompareTo(x) != 0)
                    {
                        if (pred == null)
                            less = curr.key.CompareTo(x) < 0;
                        else
                            less = pred.key.CompareTo(x) < 0;
                        if (pred != null)
                            Monitor.Exit(pred);
                        pred = curr;
                        if (less)
                            curr = curr.left;
                        else
                            curr = curr.right;
                        Monitor.Enter(curr);
                        Monitor.Exit(pred);
                    }
                    if (curr == null)
                        pred.assign(less, new SNode<T>(x));
                }
                finally
                {
                    Monitor.Exit(pred);
                    Monitor.Exit(curr);
                }
            }
        }

        public void Remove(T x)
        {
            SNode<T> pred = null, curr = root;
            if (root == null)
            {
                Monitor.Enter(root);
                root = new SNode<T>(x);
                Monitor.Exit(root);
            }
            else
            {
                Monitor.Enter(curr);
                try
                {
                    bool less = false;
                    while (curr != null || curr.key.CompareTo(x) != 0)
                    {
                        if (pred == null)
                            less = curr.key.CompareTo(x) < 0;
                        else
                            less = pred.key.CompareTo(x) < 0;
                        if (pred != null)
                            Monitor.Exit(pred);
                        pred = curr;
                        if (less)
                            curr = curr.left;
                        else
                            curr = curr.right;
                        Monitor.Enter(curr);
                        Monitor.Exit(pred);
                    }
                    if (curr == null)
                        pred.assign(less, new SNode<T>(x));
                }
                finally
                {
                    Monitor.Exit(pred);
                    Monitor.Exit(curr);
                }
            }
        }

        public bool Contains(T x)
        {
            bool was = false;
            SNode<T> pred = null, curr = root;
            Monitor.Enter(curr);
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
                        Monitor.Exit(pred);
                    pred = curr;
                    if (less)
                        curr = curr.left;
                    else
                        curr = curr.right;
                    Monitor.Enter(curr);
                    Monitor.Exit(pred);
                }
                if (curr.key.CompareTo(x) == 0)
                    was = true;
            }
            finally
            {
                Monitor.Exit(pred);
                Monitor.Exit(curr);
            }
            return was;
        }
    }
}
