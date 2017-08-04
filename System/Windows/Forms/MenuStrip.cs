namespace System.Windows.Forms
{
    using System.Drawing;

    [Serializable]
    public class MenuStrip : ToolStrip
    {
        public MenuStrip()
        {
            BorderColor = Drawing.Color.Transparent;
            Orientation = Orientation.Horizontal;
            Padding = new Padding(2);
        }

        protected override Size DefaultSize
        {
            get { return new Size(200, 24); }
        }
    }
}
