namespace System.Windows.Forms
{
    using System.Drawing;

    public class ToolStripDropDownItem : ToolStripItem
    {
        private readonly ToolStripItemCollection dropDownItems;
        private ToolStrip dropDownToolStrip;
        private bool pressed;

        protected ToolStripDropDownItem()
        {
            dropDownItems = new ToolStripItemCollection(Parent, null);

            ArrowColor = Color.Black;
            ArrowImage = Unity.API.ApplicationBehaviour.GdiImages.DropDownRightArrow;
        }

        public Color ArrowColor { get; set; }
        public Bitmap ArrowImage { get; set; }
        public ToolStripItemCollection DropDownItems { get { return dropDownItems; } }
        public override bool Pressed
        {
            get
            {
                return pressed;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!Enabled) return;

            RaiseClick();
            if (DropDownItems.Count == 0)
            {
                ResetSelected();
                if (Parent.Parent == null)
                    Parent.Dispose();
                return;
            }

            if (dropDownToolStrip != null)
            {
                dropDownToolStrip = null;
                return;
            }

            if (!pressed)
            {
                dropDownToolStrip = new ToolStrip();
                dropDownToolStrip.uwfContext = true;
                dropDownToolStrip.OwnerItem = this;

                if (Parent.uwfShadowHandler != null)
                    dropDownToolStrip.MakeShadow();

                int index = Parent.Items.IndexOf(this);
                int x = 0, y = 0;
                for (int i = 0; i < index; i++)
                {
                    if (Parent.Orientation == Orientation.Horizontal)
                        x += Parent.Items[i].Width;
                    if (Parent.Orientation == Orientation.Vertical)
                        y += Parent.Items[i].Height;
                }
                //_dropDownToolStrip.BackColor = Parent.BackColor;
                dropDownToolStrip.Items.AddRange(DropDownItems);
                for (int i = 0; i < dropDownToolStrip.Items.Count; i++)
                {
                    dropDownToolStrip.Items[i].OwnerItem = this;
                    dropDownToolStrip.Items[i].Selected = false;
                }
                dropDownToolStrip.uwfShadowBox = true;
                dropDownToolStrip.Orientation = Orientation.Vertical;
                int height = 0;
                for (int i = 0; i < DropDownItems.Count; i++)
                    height += DropDownItems[i].Height;
                dropDownToolStrip.Size = new Size(DropDownItems[0].Width, height);

                var parentLocationClient = Parent.PointToScreen(Point.Empty);
                if (Parent.Orientation == Orientation.Horizontal)
                {
                    dropDownToolStrip.Location = new Point(parentLocationClient.X + x + Parent.Padding.Left, parentLocationClient.Y + Parent.Height - HoverPadding.Height - 1);
                    dropDownToolStrip.BorderColor = Color.Transparent;
                }
                else
                {
                    dropDownToolStrip.Location = new Point(parentLocationClient.X + x + Parent.Width, parentLocationClient.Y + y);
                    dropDownToolStrip.BorderColor = Parent.BorderColor;

                    if (dropDownToolStrip.Location.X + dropDownToolStrip.Width > Screen.PrimaryScreen.WorkingArea.Width)
                    {
                        dropDownToolStrip.Location = new Point(parentLocationClient.X - dropDownToolStrip.Width, dropDownToolStrip.Location.Y);
                    }
                }

                dropDownToolStrip.Disposed += (sender, args) =>
                {
                    var clientRect = new Rectangle(x, y, Width, Height);
                    var contains = clientRect.Contains(Parent.PointToClient(Control.MousePosition));
                    if (!contains)
                        pressed = false;
                    else
                        pressed = !pressed;
                    dropDownToolStrip = null;
                };
            }
            else
            {
                pressed = false;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (dropDownItems.Count > 0 && Parent.Orientation == Orientation.Vertical)
            {
                e.Graphics.uwfDrawImage(ArrowImage, ArrowColor, e.ClipRectangle.X + e.ClipRectangle.Width - 26, e.ClipRectangle.Y, 24, 24);
            }
        }
    }
}