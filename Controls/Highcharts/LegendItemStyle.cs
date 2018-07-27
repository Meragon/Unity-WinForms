namespace Highcharts
{
    using System;
    using System.Drawing;

    public struct LegendItemStyle : IEquatable<LegendItemStyle>
    {
        public readonly Color color;
        public readonly Font font;

        public LegendItemStyle(Color color)
        {
            this.color = color;
            font = Highchart.DefaultFont12B;
        }

        public bool Equals(LegendItemStyle other)
        {
            return color.Equals(other.color) && Equals(font, other.font);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LegendItemStyle && Equals((LegendItemStyle)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (color.GetHashCode() * 397) ^ (font != null ? font.GetHashCode() : 0);
            }
        }
    }
}
