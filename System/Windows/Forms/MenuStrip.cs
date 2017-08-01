namespace System.Windows.Forms
{
    [Serializable]
    public class MenuStrip : ToolStrip
    {
        public MenuStrip()
        {
            BorderColor = Drawing.Color.Transparent;
            Orientation = Orientation.Horizontal;
            Padding = new Padding(2);
            Size = new Drawing.Size(600, 24);
        }
    }
}
