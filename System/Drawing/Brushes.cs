using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public sealed class Brushes
    {
        public static SolidBrush Black { get { return new SolidBrush(Color.Black); } }
        public static SolidBrush DarkGray { get { return new SolidBrush(Color.DarkGray); } }
        public static SolidBrush DarkRed { get { return new SolidBrush(Color.DarkRed); } }
        public static SolidBrush Gray { get { return new SolidBrush(Color.Gray); } }
        public static SolidBrush IndianRed { get { return new SolidBrush(Color.IndianRed); } }
        public static SolidBrush LightGray { get { return new SolidBrush(Color.LightGray); } }
        public static SolidBrush White { get { return new SolidBrush(Color.White); } }
    }
}
