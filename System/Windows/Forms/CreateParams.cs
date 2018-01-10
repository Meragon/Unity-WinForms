namespace System.Windows.Forms
{
    using System.Text;

    public class CreateParams
    {
        private string caption;
        private string className;
        private int classStyle;
        private int exStyle;
        private int height;
        private object param;
        private IntPtr parent;
        private int style;
        private int width;
        private int x;
        private int y;

        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }
        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }
        public int ClassStyle
        {
            get { return classStyle; }
            set { classStyle = value; }
        }
        public int ExStyle
        {
            get { return exStyle; }
            set { exStyle = value; }
        }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        public object Param
        {
            get { return param; }
            set { param = value; }
        }
        public IntPtr Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        /// <summary>
        /// TODO: https://msdn.microsoft.com/en-us/library/windows/desktop/ms632600(v=vs.85).aspx
        /// </summary>
        public int Style
        {
            get { return style; }
            set { style = value; }
        }
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("CreateParams {'");
            sb.Append(className);
            sb.Append("', '");
            sb.Append(caption);
            sb.Append("', 0x");
            sb.Append(Convert.ToString(style, 16));
            sb.Append(", 0x");
            sb.Append(Convert.ToString(exStyle, 16));
            sb.Append(", {");
            sb.Append(x);
            sb.Append(", ");
            sb.Append(y);
            sb.Append(", ");
            sb.Append(width);
            sb.Append(", ");
            sb.Append(height);
            sb.Append("}");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
