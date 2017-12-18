using System;

namespace ParallelAlgorithms
{
    public enum nodeColor { BLACK, RED };

    public class Node<T> where T: IComparable
    {
        public Node<T> left, right, parent;
        public nodeColor color;
        public T data;

        public Node()
        {

        }
        public Node(T data, Node<T> NIL)
        {
            this.data = data;
            this.left = NIL;
            this.right = NIL;
            this.parent = NIL;
            this.color = nodeColor.RED;
        }

        public static Node<T> NIL = new Node<T>(NIL);

        public Node(Node<T> NIL)
        {
            this.left = NIL;
            this.right = NIL;
            this.parent = NIL;
            this.color = nodeColor.BLACK;
        }

        public void setL(Node<T> d, bool op)
        {
            if (op)
                right = d;
            else
                left = d;
        }

        public void setR(Node<T> d, bool op)
        {
            if (op)
                left = d;
            else
                right = d;
        }

        public Node<T> l(bool op) { return op ? right : left; }
        public Node<T> r(bool op) { return op ? left : right; }
    };

    public class RBTSimple<T> : IUniqueContainer<T> where T:IComparable
    {

        Node<T> root = Node<T>.NIL;

        void rotate(Node<T> x, bool b)
        {//b = right
            Node<T> y = x.r(b);
            x.setR(y.l(b), b);
            if (x.r(b) != Node<T>.NIL)
                x.r(b).parent = x;
            y.parent = x.parent;
            if (x.parent == Node<T>.NIL)
                root = y;
            x.parent.setR(y, b ^ (x.parent.l(b) == x));
            y.setL(x, b);
            x.parent = y;
        }

        void print(Node<T> t, bool en = true)
        {
            Console.Write('(');
            if (t == Node<T>.NIL)
                Console.Write("NIL");
            else
            {
                Console.Write("d: ");
                Console.Write(t.data);
                Console.Write(", c: ");
                Console.Write(t.color);
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

        void insert_fixup(Node<T> z)
        {
            while (z.parent.color == nodeColor.RED)
            {
                bool b = z.parent != z.parent.parent.left;
                Node<T> y = z.parent.parent.r(b); //uncle
                if (y.color == nodeColor.RED)
                {
                    z.parent.color = nodeColor.BLACK;
                    y.color = nodeColor.BLACK;
                    z.parent.parent.color = nodeColor.RED;
                    z = z.parent.parent;
                }
                else
                {
                    if (z == z.parent.r(b))
                    {
                        z = z.parent;
                        rotate(z, b);
                    }
                    z.parent.color = nodeColor.BLACK;
                    z.parent.parent.color = nodeColor.RED;
                    rotate(z.parent.parent, !b);
                }
            }
            root.color = nodeColor.BLACK;
        }

        void insert(Node<T> z)
        {
            Node<T> y = Node<T>.NIL, x = root;
            while (x != Node<T>.NIL)
            {
                y = x;
                x = x.r(z.data.CompareTo(x.data) < 0);
            }
            z.parent = y;
            if (y == Node<T>.NIL)
                root = z;
            else
                y.setR(z, z.data.CompareTo(y.data) < 0);
            insert_fixup(z);
        }

        void transplant(Node<T> u, Node<T> v)
        {
            if (u.parent == Node<T>.NIL)
                root = v;
            else
                u.parent.setR(v, u == u.parent.left);
            v.parent = u.parent;
        }

        Node<T> treemin(Node<T> t)
        {
            while (t.left != Node<T>.NIL)
                t = t.left;
            return t;
        }

        void erase_fixup(Node<T> x)
        {
            while (x != root && x.color == nodeColor.BLACK)
            {
                bool b = x != x.parent.left;
                Node<T> w = x.parent.r(b);
                if (w.color == nodeColor.RED)
                {
                    Swap(ref w.color, ref x.parent.color);
                    rotate(x.parent, b);
                    w = x.parent.r(b);
                }
                if (w.l(b).color == nodeColor.BLACK && w.r(b).color == nodeColor.BLACK)
                {
                    w.color = nodeColor.RED;
                    x = x.parent;
                }
                else
                {
                    if (w.r(b).color == nodeColor.BLACK)
                    {
                        Swap(ref w.l(b).color, ref w.color);
                        rotate(w, !b);
                        w = x.parent.r(b);
                    }
                    w.color = x.parent.color;
                    x.parent.color = nodeColor.BLACK;
                    w.r(b).color = nodeColor.BLACK;
                    rotate(x.parent, b);
                    x = root;
                }
            }
            x.color = nodeColor.BLACK;
        }

        void erase(Node<T> z)
        {
            if (z == Node<T>.NIL)
                return;
            Node<T> y = z, x;
            nodeColor origin_color = y.color;
            if (z.left == Node<T>.NIL || z.right == Node<T>.NIL)
            {
                x = z.r(z.right == Node<T>.NIL);
                transplant(z, z.r(z.right == Node<T>.NIL));
            }
            else
            {
                y = treemin(z.right);
                origin_color = y.color;
                x = y.right;
                if (y.parent == z)
                    x.parent = y;
                else
                {
                    transplant(y, y.right);
                    y.right = z.right;
                    y.right.parent = y;
                }
                transplant(z, y);
                y.left = z.left;
                y.left.parent = y;
                y.color = z.color;
            }
            if (origin_color == nodeColor.BLACK)
                erase_fixup(x);
        }

        Node<T> treefind(Node<T> t, T x)
        {
            if (t == Node<T>.NIL || t.data.CompareTo(x) == 0)
                return t;
            return treefind(t.r(t.data.CompareTo(x) > 0), x);
        }

        int h(Node<T> t)
        {
            if (t == Node<T>.NIL)
                return 0;
            return Math.Max(h(t.left), h(t.right)) + 1;
        }
        int size(Node<T> t)
        {
            if (t == Node<T>.NIL)
                return 0;
            return size(t.left) + size(t.right) + 1;
        }


        public void Add(T x)
        {
            if (Contains(x))
                return;
            insert(new Node<T>(x, Node<T>.NIL));
        }
        public void Remove(T x)
        {
            erase(treefind(root, x));
        }
        public bool Contains(T x)
        {
            return treefind(root, x) != Node<T>.NIL;
        }
        public int GetHeight()
        {
            return h(root);
        }
        public int GetSize()
        {
            return size(root);
        }

        void Swap<E>(ref E a, ref E b)
        {
            E temp = a;
            a = b;
            b = temp;
        }
    } 
}
