
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public struct PointF
    {
        private float _x;
        private float _y;

        public static readonly PointF Empty = default(PointF);

        public static implicit operator Point(PointF p)
        {
            return new Point((int)p.X, (int)p.Y);
        }

        public static bool operator ==(PointF left, PointF right)
        {
            if (left.X == right.X && left.Y == right.Y)
                return true;
            return false;
        }
        public static bool operator !=(PointF left, PointF right)
        {
            return !(left == right);
        }

        public static PointF Zero { get { return new PointF(0, 0); } }

        public UnityEngine.Vector2 AsVector2 { get { return new UnityEngine.Vector2(_x, _y); } }
        public float X { get { return _x; } set { _x = value; } }
        public float Y { get { return _y; } set { _y = value; } }

        public PointF(float x, float y)
        {
            _x = 0;
            _y = 0;

            X = x;
            Y = y;
        }

        public float Distance(PointF to)
        {
            return (float)Math.Sqrt(Math.Pow(to.X - X, 2) + Math.Pow(to.Y - Y, 2));
        }
        public void Offset(Point point)
        {
            _x += point.X;
            _y += point.Y;
        }
        public static Point FromVector2(UnityEngine.Vector2 vector)
        {
            return new PointF(vector.x, vector.y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PointF))
            {
                return false;
            }
            PointF point = (PointF)obj;
            return point.X == this.X && point.Y == this.Y;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return "{ X: " + X.ToString() + "; Y: " + Y.ToString() + " }";
        }
    }
}
