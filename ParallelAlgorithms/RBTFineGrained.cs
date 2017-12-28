using System;

namespace ParallelAlgorithms
{
    public class RbtFineGrained<T> : IUniqueContainer<T> where T : IComparable
    {
        private Node<T> _root = Node<T>.Nil;


        public void Add(T x)
        {
            if (Contains(x))
            {
                return;
            }
            Insert(new Node<T>(x, Node<T>.Nil));
        }

        public void Remove(T x)
        {
            Erase(Treefind(_root, x));
        }

        public bool Contains(T x)
        {
            return Treefind(_root, x) != Node<T>.Nil;
        }

        private void Rotate(Node<T> x, bool b)
        {
//b = right
            Node<T> y = x.R(b);
            x.SetR(y.L(b), b);
            if (x.R(b) != Node<T>.Nil)
            {
                x.R(b).Parent = x;
            }
            y.Parent = x.Parent;
            if (x.Parent == Node<T>.Nil)
            {
                _root = y;
            }
            x.Parent.SetR(y, b ^ (x.Parent.L(b) == x));
            y.SetL(x, b);
            x.Parent = y;
        }

        private void Print(Node<T> t, bool en = true)
        {
            Console.Write('(');
            if (t == Node<T>.Nil)
            {
                Console.Write("NIL");
            }
            else
            {
                Console.Write("d: ");
                Console.Write(t.Data);
                Console.Write(", c: ");
                Console.Write(t.Color);
                Console.Write(", l: ");
                Print(t.Left, false);
                Console.Write(", r: ");
                Print(t.Right, false);
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

        private void insert_fixup(Node<T> z)
        {
            while (z.Parent.Color == NodeColor.Red)
            {
                bool b = z.Parent != z.Parent.Parent.Left;
                Node<T> y = z.Parent.Parent.R(b); //uncle
                if (y.Color == NodeColor.Red)
                {
                    z.Parent.Color = NodeColor.Black;
                    y.Color = NodeColor.Black;
                    z.Parent.Parent.Color = NodeColor.Red;
                    z = z.Parent.Parent;
                }
                else
                {
                    if (z == z.Parent.R(b))
                    {
                        z = z.Parent;
                        Rotate(z, b);
                    }
                    z.Parent.Color = NodeColor.Black;
                    z.Parent.Parent.Color = NodeColor.Red;
                    Rotate(z.Parent.Parent, !b);
                }
            }
            _root.Color = NodeColor.Black;
        }

        private void Insert(Node<T> z)
        {
            Node<T> y = Node<T>.Nil, x = _root;
            while (x != Node<T>.Nil)
            {
                y = x;
                x = x.R(z.Data.CompareTo(x.Data) < 0);
            }
            z.Parent = y;
            if (y == Node<T>.Nil)
            {
                _root = z;
            }
            else
            {
                y.SetR(z, z.Data.CompareTo(y.Data) < 0);
            }
            insert_fixup(z);
        }

        private void Transplant(Node<T> u, Node<T> v)
        {
            if (u.Parent == Node<T>.Nil)
            {
                _root = v;
            }
            else
            {
                u.Parent.SetR(v, u == u.Parent.Left);
            }
            v.Parent = u.Parent;
        }

        private Node<T> Treemin(Node<T> t)
        {
            while (t.Left != Node<T>.Nil)
            {
                t = t.Left;
            }
            return t;
        }

        private void erase_fixup(Node<T> x)
        {
            while (x != _root && x.Color == NodeColor.Black)
            {
                bool b = x != x.Parent.Left;
                Node<T> w = x.Parent.R(b);
                if (w.Color == NodeColor.Red)
                {
                    Swap(ref w.Color, ref x.Parent.Color);
                    Rotate(x.Parent, b);
                    w = x.Parent.R(b);
                }
                if (w.L(b).Color == NodeColor.Black && w.R(b).Color == NodeColor.Black)
                {
                    w.Color = NodeColor.Red;
                    x = x.Parent;
                }
                else
                {
                    if (w.R(b).Color == NodeColor.Black)
                    {
                        Swap(ref w.L(b).Color, ref w.Color);
                        Rotate(w, !b);
                        w = x.Parent.R(b);
                    }
                    w.Color = x.Parent.Color;
                    x.Parent.Color = NodeColor.Black;
                    w.R(b).Color = NodeColor.Black;
                    Rotate(x.Parent, b);
                    x = _root;
                }
            }
            x.Color = NodeColor.Black;
        }

        private void Erase(Node<T> z)
        {
            if (z == Node<T>.Nil)
            {
                return;
            }
            Node<T> y = z, x;
            NodeColor originColor = y.Color;
            if (z.Left == Node<T>.Nil || z.Right == Node<T>.Nil)
            {
                x = z.R(z.Right == Node<T>.Nil);
                Transplant(z, z.R(z.Right == Node<T>.Nil));
            }
            else
            {
                y = Treemin(z.Right);
                originColor = y.Color;
                x = y.Right;
                if (y.Parent == z)
                {
                    x.Parent = y;
                }
                else
                {
                    Transplant(y, y.Right);
                    y.Right = z.Right;
                    y.Right.Parent = y;
                }
                Transplant(z, y);
                y.Left = z.Left;
                y.Left.Parent = y;
                y.Color = z.Color;
            }
            if (originColor == NodeColor.Black)
            {
                erase_fixup(x);
            }
        }

        private Node<T> Treefind(Node<T> t, T x)
        {
            if (t == Node<T>.Nil || t.Data.CompareTo(x) == 0)
            {
                return t;
            }
            return Treefind(t.R(t.Data.CompareTo(x) > 0), x);
        }

        private int H(Node<T> t)
        {
            if (t == Node<T>.Nil)
            {
                return 0;
            }
            return Math.Max(H(t.Left), H(t.Right)) + 1;
        }

        private int Size(Node<T> t)
        {
            if (t == Node<T>.Nil)
            {
                return 0;
            }
            return Size(t.Left) + Size(t.Right) + 1;
        }

        public int GetHeight()
        {
            return H(_root);
        }

        public int GetSize()
        {
            return Size(_root);
        }

        private void Swap<TE>(ref TE a, ref TE b)
        {
            TE temp = a;
            a = b;
            b = temp;
        }
    }
}