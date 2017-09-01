namespace System.Windows.Forms
{
    using System.Drawing;

    public class ToolStripMenuItem : ToolStripDropDownItem
    {
        private ContentAlignment shortcutKeysFormat = ContentAlignment.MiddleLeft;

        public ToolStripMenuItem()
        {
        }
        public ToolStripMenuItem(string text)
        {
            Text = text;
        }

        public bool Checked { get; set; }
        public string ShortcutKeys { get; set; } // TODO: enum (flag).

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Checked)
            {
                int rectWH = e.ClipRectangle.Height - 8;
                int checkedWH = e.ClipRectangle.Height - 12;

                e.Graphics.uwfFillRectangle(HoverColor, e.ClipRectangle.X + 8, e.ClipRectangle.Y + 4, rectWH, rectWH);
                e.Graphics.DrawRectangle(selectPen, e.ClipRectangle.X + 8, e.ClipRectangle.Y + 4, rectWH, rectWH);
                e.Graphics.DrawImage(Unity.API.UnityWinForms.GdiImages.Checked, e.ClipRectangle.X + 10, e.ClipRectangle.Y + 6, checkedWH, checkedWH);
            }

            if (!string.IsNullOrEmpty(ShortcutKeys))
            {
                //e.Graphics.DrawRectangle(Pens.DarkRed, e.ClipRectangle);
                if (Parent.Orientation == Orientation.Vertical)
                    e.Graphics.uwfDrawString(
                        ShortcutKeys,
                        Font,
                        Enabled ? ForeColor : ForeColor + Color.FromArgb(0, 100, 100, 100),
                        e.ClipRectangle.X + e.ClipRectangle.Width - 60,
                        e.ClipRectangle.Y,
                        60,
                        e.ClipRectangle.Height,
                        shortcutKeysFormat);
            }
        }
    }
}