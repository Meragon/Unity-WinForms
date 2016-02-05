
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    [Serializable]
    public struct Point
    {
        private int _x;
        private int _y;

        public static readonly Point Empty = default(Point);

        public static implicit operator PointF(Point p)
        {
            return new PointF(p.X, p.Y);
        }

        public static Point Zero { get { return new Point(0, 0); } }
        public static Point operator +(Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }
        public static Point operator +(Point left, int right)
        {
            return new Point(left.X + right, left.Y + right);
        }
        public static Point operator +(Point left, int[] right)
        {
            if (right.Length >= 2)
                return new Point(left.X + right[0], left.Y + right[1]);
            throw new ArgumentOutOfRangeException();
        }
        public static Point operator -(Point point)
        {
            return new Point(-point.X, -point.Y);
        }
        public static Point operator -(Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
        }
        public static Point operator -(Point left, int right)
        {
            return new Point(left.X + right, left.Y + right);
        }
        public static Point operator -(Point left, int[] right)
        {
            if (right.Length >= 2)
                return new Point(left.X + right[0], left.Y + right[1]);
            throw new ArgumentOutOfRangeException();
        }
        public static Point operator *(Point left, int right)
        {
            return new Point(left.X * right, left.Y * right);
        }
        public static Point operator *(Point left, float right)
        {
            return new Point((int)(left.X * right), (int)(left.Y * right));
        }
        public static bool operator ==(Point left, Point right)
        {
            if (left.X == right.X && left.Y == right.Y)
                return true;
            return false;
        }
        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }

        public UnityEngine.Vector2 AsVector2 { get { return new UnityEngine.Vector2(_x, _y); } }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }

        public Point(int x, int y)
        {
            _x = 0;
            _y = 0;

            X = x;
            Y = y;
        }

        public int Distance(Point to)
        {
            return (int)Math.Sqrt(Math.Pow(to.X - X, 2) + Math.Pow(to.Y - Y, 2));
        }
        public void Offset(Point point)
        {
            _x += point.X;
            _y += point.Y;
        }
        public static Point FromVector2(UnityEngine.Vector2 vector)
        {
            return new Point((int)vector.x, (int)vector.y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
            {
                return false;
            }
            Point point = (Point)obj;
            return point.X == this.X && point.Y == this.Y;
        }
        public override int GetHashCode()
        {
            return _x ^ _y;
        }
        public override string ToString()
        {
            return "{ X: " + X.ToString() + "; Y: " + Y.ToString() + " }";
        }
    }
}
