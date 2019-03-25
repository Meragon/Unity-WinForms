namespace System.Windows.Forms
{
    using System.Text;

    public class CreateParams
    {
        public string Caption { get; set; }
        public string ClassName { get; set; }
        public int ClassStyle { get; set; }
        public int ExStyle { get; set; }
        public int Height { get; set; }
        public object Param { get; set; }
        public IntPtr Parent { get; set; }
        /// <summary>
        /// TODO: https://msdn.microsoft.com/en-us/library/windows/desktop/ms632600(v=vs.85).aspx
        /// </summary>
        public int Style { get; set; }
        public int Width { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("CreateParams {'");
            sb.Append(ClassName);
            sb.Append("', '");
            sb.Append(Caption);
            sb.Append("', 0x");
            sb.Append(Convert.ToString(Style, 16));
            sb.Append(", 0x");
            sb.Append(Convert.ToString(ExStyle, 16));
            sb.Append(", {");
            sb.Append(X);
            sb.Append(", ");
            sb.Append(Y);
            sb.Append(", ");
            sb.Append(Width);
            sb.Append(", ");
            sb.Append(Height);
            sb.Append("}");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
