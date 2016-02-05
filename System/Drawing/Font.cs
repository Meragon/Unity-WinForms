using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    [Serializable]
    public sealed class Font
    {
        private string _name = "";
        private FontStyle _style;

        public string Name { get { return _name; } }
        public float Size { get; set; }
        public FontStyle Style { get { return _style; } }

        internal Font()
        {

        }
        public Font(string familyName, float emSize)
        {
            _name = familyName;
            Size = emSize;
        }
        public Font(string familyName, float emSize, FontStyle style)
        {
            _name = familyName;
            _style = style;
            Size = emSize;
        }

        public override string ToString()
        {
            return "{ " + Name + "; " + Size.ToString() + "; " + Style.ToString() + " }";
        }
    }

    public enum FontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikeout = 8,
    }
}
