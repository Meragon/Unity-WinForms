namespace System.Windows.Forms
{
    using System.Drawing;

    public class ContextMenuStrip : ToolStripDropDownMenu
    {
        public ContextMenuStrip()
        {
            BackColor = Color.FromArgb(246, 246, 246);

            uwfContext = true;
            uwfShadowBox = true;
        }
    }
}
