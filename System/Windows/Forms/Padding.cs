namespace System.Windows.Forms
{
    using System.Drawing;

    public struct Padding : IEquatable<Padding>
    {
        public static readonly Padding Empty = new Padding(0);

        public Padding(int all) : this()
        {
            Left = Bottom = Right = Top = 0;

            Left = all;
            Bottom = all;
            Right = all;
            Top = all;
        }
        public Padding(int left, int bottom, int right, int top) : this()
        {
            this.Left = this.Bottom = this.Right = this.Top = 0;

            this.Left = left;
            this.Bottom = bottom;
            this.Right = right;
            this.Top = top;
        }

        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Horizontal { get { return Left + Right; } }
        public int Right { get; set; }
        public Size Size
        {
            get { return new Size(Horizontal, Vertical); }
        }
        public int Top { get; set; }
        public int Vertical { get { return Top + Bottom; } }

        public static Padding operator +(Padding p1, Padding p2)
        {
            return new Padding(p1.Left + p2.Left, p1.Top + p2.Top, p1.Right + p2.Right, p1.Bottom + p2.Bottom);
        }
        public static Padding operator -(Padding p1, Padding p2)
        {
            return new Padding(p1.Left - p2.Left, p1.Top - p2.Top, p1.Right - p2.Right, p1.Bottom - p2.Bottom);
        }
        public static bool operator ==(Padding p1, Padding p2)
        {
            if (p1.Left == p2.Left && p1.Top == p2.Top && p1.Right == p2.Right)
                return p1.Bottom == p2.Bottom;
            return false;
        }
        public static bool operator !=(Padding p1, Padding p2)
        {
            return !(p1 == p2);
        }

        public bool Equals(Padding other)
        {
            return Bottom == other.Bottom && Left == other.Left && Right == other.Right && Top == other.Top;
        }
        public override bool Equals(object obj)
        {
            if (obj is Padding)
                return (Padding) obj == this;
            return false;
        }
        public override int GetHashCode()
        {
            return Left
                   ^ WindowsFormsUtils.RotateLeft(Top, 8)
                   ^ WindowsFormsUtils.RotateLeft(Right, 16)
                   ^ WindowsFormsUtils.RotateLeft(Bottom, 24);
        }
        public override string ToString()
        {
            return "{Left=" + Left + ",Top=" + Top + ",Right=" + Right + ",Bottom=" + Bottom + "}";
        }
    }
}
