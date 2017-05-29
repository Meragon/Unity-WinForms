using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class ToolStripDropDown : ToolStrip
    {
        public ToolStripDropDown()
        {
            Orientation = Forms.Orientation.Vertical;
            Visible = false;
        }

        /// <summary>
        /// null, MousePosition
        /// </summary>
        /// <param name="control"></param>
        /// <param name="position"></param>
        public void Show(Control control, Point position)
        {
            if (control == null)
                Location = position;
            else
                Location = control.PointToScreen(Point.Empty).Add(position);
            int height = 0;
            int width = 160;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].JustVisual) continue;
                if (Items[i] is ToolStripMenuItem && !String.IsNullOrEmpty((Items[i] as ToolStripMenuItem).ShortcutKeys))
                    width = 220;
                height += 24;
            }
            Size = new Drawing.Size(width, height);

            if (Location.X + width > Screen.PrimaryScreen.WorkingArea.Width)
                Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - width, Location.Y);

            Visible = true;

            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] is ToolStripDropDownItem)
                {
                    var ddi = Items[i] as ToolStripDropDownItem;
                    ddi.ArrowImage = ApplicationBehaviour.GdiImages.DropDownRightArrow;
                    ddi.ArrowColor = Color.Black;
                }
                Items[i].ForeColor = Color.FromArgb(64, 64, 64);
                Items[i].HoverColor = Color.FromArgb(160, 210, 222, 245);
                Items[i].TextAlign = ContentAlignment.MiddleLeft;
                switch (Orientation)
                {
                    case Forms.Orientation.Horizontal:

                        break;
                    case Forms.Orientation.Vertical:
                        Items[i].Size = new Size(Size.Width, 24);
                        break;
                }
            }
        }
    }
}
