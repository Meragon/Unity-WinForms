using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public sealed class Pens
    {
        public static Pen Black { get { return new Pen(Color.Black); } }
        public static Pen DarkGray { get { return new Pen(Color.DarkGray); } }
        public static Pen DarkRed { get { return new Pen(Color.DarkRed); } }
        public static Pen LightGray { get { return new Pen(Color.LightGray); } }
        public static Pen Gray { get { return new Pen(Color.Gray); } }
    }
}
