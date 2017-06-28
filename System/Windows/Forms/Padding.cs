using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public struct Padding : IEquatable<Padding>
    {
        private int bottom;
        private int left;
        private int right;
        private int top;

        public int Bottom { get { return bottom; } set { bottom = value; } }
        public int Left { get { return left; } set { left = value; } }
        public int Horizontal { get { return left + right; } }
        public int Right { get { return right; } set { right = value; } }
        public Size Size
        {
            get { return new Size(Horizontal, Vertical); }
        }
        public int Top { get { return top; } set { top = value; } }
        public int Vertical { get { return top + bottom; } }

        public static readonly Padding Empty = new Padding(0);

        public static Padding operator +(Padding p1, Padding p2)
        {
            return new Padding(p1.left + p2.left, p1.top + p2.top, p1.right + p2.right, p1.bottom + p2.bottom);
        }
        public static Padding operator -(Padding p1, Padding p2)
        {
            return new Padding(p1.left - p2.left, p1.top - p2.top, p1.right - p2.right, p1.bottom - p2.bottom);
        }
        public static bool operator ==(Padding p1, Padding p2)
        {
            if (p1.left == p2.left && p1.top == p2.top && p1.right == p2.right)
                return p1.bottom == p2.bottom;
            return false;
        }
        public static bool operator !=(Padding p1, Padding p2)
        {
            return !(p1 == p2);
        }

        public Padding(int all)
        {
            left = bottom = right = top = 0;

            left = all;
            bottom = all;
            right = all;
            top = all;
        }
        public Padding(int left, int bottom, int right, int top)
        {
            this.left = this.bottom = this.right = this.top = 0;

            this.left = left;
            this.bottom = bottom;
            this.right = right;
            this.top = top;
        }

        public bool Equals(Padding other)
        {
            return bottom == other.bottom && left == other.left && right == other.right && top == other.top;
        }
        public override bool Equals(object other)
        {
            if (other is Padding)
                return (Padding)other == this;
            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = bottom;
                hashCode = (hashCode * 397) ^ left;
                hashCode = (hashCode * 397) ^ right;
                hashCode = (hashCode * 397) ^ top;
                return hashCode;
            }
        }
        public override string ToString()
        {
            return "{ L: " + left + "; B: " + bottom + "; R: " + right + "; T: " + top + " }";
        }

        public static Padding Add(Padding p1, Padding p2)
        {
            return p1 + p2;
        }
        public static Padding Subtract(Padding p1, Padding p2)
        {
            return p1 - p2;
        }
    }
}
