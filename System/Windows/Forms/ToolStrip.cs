namespace System.Windows.Forms
{
    using System.Drawing;

    public class ToolStrip : ScrollableControl
    {
        private readonly SolidBrush brushBack = new SolidBrush(Color.Transparent);
        private readonly Pen borderPen = new Pen(Color.Transparent);
        private readonly ToolStripItemCollection items;
        private readonly PaintEventArgs p_args;
        private readonly Pen verticalLinePen = new Pen(Color.FromArgb(215, 215, 215));

        public ToolStrip()
        {
            items = new ToolStripItemCollection(this, null);
            p_args = new PaintEventArgs();

            BackColor = Color.FromArgb(246, 246, 246);
            BorderColor = Color.FromArgb(204, 206, 219);
            Orientation = Orientation.Vertical;

            uwfAppOwner.UpClick += Application_UpClick;
        }
        public ToolStrip(ToolStripItem[] items)
        {
            this.items = new ToolStripItemCollection(this, items);
            this.items.AddRange(items);

            BackColor = Color.FromArgb(246, 246, 246);
            BorderColor = Color.FromArgb(204, 206, 219);
            Orientation = Orientation.Vertical;

            uwfAppOwner.UpClick += Application_UpClick;
        }

        public event ToolStripItemClickedEventHandler ItemClicked = delegate { };

        public override Color BackColor
        {
            get { return brushBack.Color; }
            set { brushBack.Color = value; }
        }
        public Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }
        public virtual ToolStripItemCollection Items { get { return items; } }
        public Orientation Orientation { get; set; }

        internal ToolStripItem OwnerItem { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(100, 25); }
        }

        public void ResetSelected()
        {
            for (int i = 0; i < items.Count; i++)
                items[i].Selected = false;
        }

        /// <summary>
        /// For menu strips.
        /// </summary>
        internal void MakeShadow()
        {
            uwfShadowHandler = (g) =>
            {
                var loc = PointToScreen(Point.Empty);
                var color = Color.FromArgb(12, 64, 64, 64);
                g.Graphics.uwfFillRectangle(color, loc.X - 3, loc.Y, Width + 6, Height + 3);
                g.Graphics.uwfFillRectangle(color, loc.X - 2, loc.Y, Width + 4, Height + 2);
                g.Graphics.uwfFillRectangle(color, loc.X - 1, loc.Y, Width + 2, Height + 1);
            };
        }

        protected override void Dispose(bool release_all)
        {
            uwfAppOwner.DownClick -= Application_UpClick;
            base.Dispose(release_all);
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            var mc_pos = ((MouseEventArgs)e).Location;
            for (int i = 0, x = Padding.Left, y = Padding.Top; i < items.Count; i++)
            {
                if (items[i].JustVisual) continue;

                items[i].RaiseOnMouseLeave(e);
                if (mc_pos.X > x && mc_pos.X < x + items[i].Width && mc_pos.Y > y && mc_pos.Y < y + items[i].Height)
                    items[i].RaiseOnMouseEnter(e);

                if (Orientation == Orientation.Horizontal)
                    x += items[i].Width;
                if (Orientation == Orientation.Vertical)
                    y += items[i].Height;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].JustVisual) continue;
                items[i].RaiseOnMouseLeave(e);
            }
        }
        protected override void OnMouseUp(MouseEventArgs e) // Click.
        {
            base.OnMouseUp(e);

            int prevSelected = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Selected) prevSelected = i;
                items[i].Selected = false;
            }

            var mc_pos = e.Location;
            for (int i = 0, x = Padding.Left, y = Padding.Top; i < items.Count; i++)
            {
                if (items[i].JustVisual) continue;

                if (mc_pos.X > x && mc_pos.X < x + items[i].Width && mc_pos.Y > y && mc_pos.Y < y + items[i].Height)
                {
                    if (i != prevSelected)
                        items[i].Selected = true;

                    ItemClicked(this, new ToolStripItemClickedEventArgs(items[i]));
                    items[i].RaiseOnMouseUp(e);
                    break;
                }

                if (Orientation == Orientation.Horizontal)
                    x += items[i].Width;
                if (Orientation == Orientation.Vertical)
                    y += items[i].Height;
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Down: break;
                case Keys.Left: break;
                case Keys.Right: break;
                case Keys.Up: break;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (OwnerItem != null && OwnerItem.Parent != null && OwnerItem.Parent.Orientation == Orientation.Horizontal && uwfShadowHandler == null)
            {
                MakeShadow();
            }

            base.OnPaint(e);

            p_args.Graphics = e.Graphics;
            p_args.ClipRectangle = e.ClipRectangle;
            p_args.Graphics.FillRectangle(brushBack, 0, 0, Width, Height);

            if (Orientation == Orientation.Vertical)
                p_args.Graphics.DrawLine(verticalLinePen, 30, 2, 30, Height - 2);

            for (int i = 0, x = Padding.Left, y = Padding.Top; i < items.Count; i++)
            {
                var item = items[i];
                p_args.ClipRectangle = new Rectangle(x, y, item.Width, item.Height);
                item.RaiseOnPaint(p_args);

                if (item.JustVisual) continue;
                if (Orientation == Orientation.Horizontal)
                    x += item.Width;
                if (Orientation == Orientation.Vertical)
                    y += item.Height;
            }

            p_args.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }

        private void Application_UpClick(object sender, EventArgs e)
        {
            bool reset = true;
            var toolStrip = sender as ToolStrip;
            if (toolStrip != null)
            {
                if (toolStrip.OwnerItem != null)
                {
                    var parent = toolStrip.OwnerItem.Parent;
                    while (true)
                    {
                        if (parent == null) break;
                        if (parent == this)
                        {
                            reset = false;
                            break;
                        }

                        if (parent.OwnerItem == null) break;

                        parent = parent.OwnerItem.Parent;
                    }
                }
            }

            var mc_pos = PointToClient(MousePosition);
            if (!ClientRectangle.Contains(mc_pos) && reset)
                for (int i = 0; i < items.Count; i++)
                    items[i].Selected = false;
        }
    }
}
