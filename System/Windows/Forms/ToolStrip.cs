using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    [Serializable]
    public class ToolStrip : ScrollableControl
    {
        private SolidBrush brushBack = new SolidBrush(Color.Transparent);
        private ToolStripItemCollection _items;
        private PaintEventArgs p_args;

        public ToolStrip()
        {
            _items = new ToolStripItemCollection(this, null);
            p_args = new PaintEventArgs();

            BackColor = Color.FromArgb(246, 246, 246);
            BorderColor = Color.FromArgb(204, 206, 219);
            Orientation = Forms.Orientation.Vertical;

            uwfAppOwner.UpClick += Application_UpClick;
        }
        public ToolStrip(ToolStripItem[] items)
        {
            _items = new ToolStripItemCollection(this, items);
            _items.AddRange(items);

            BackColor = Color.FromArgb(246, 246, 246);
            BorderColor = Color.FromArgb(204, 206, 219);
            Orientation = Forms.Orientation.Vertical;

            uwfAppOwner.UpClick += Application_UpClick;
        }

        public override Color BackColor
        {
            get { return brushBack.Color; }
            set { brushBack.Color = value; }
        }
        public Color BorderColor { get; set; }
        public virtual ToolStripItemCollection Items { get { return _items; } }
        public Orientation Orientation { get; set; }
        internal ToolStripItem OwnerItem { get; set; }

        void Application_UpClick(object sender, EventArgs e)
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
                for (int i = 0; i < _items.Count; i++)
                    _items[i].Selected = false;
        }

        public override void Dispose()
        {
            uwfAppOwner.DownClick -= Application_UpClick;
            base.Dispose();
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            var mc_pos = ((MouseEventArgs)e).Location;
            for (int i = 0, x = Padding.Left, y = Padding.Top; i < _items.Count; i++)
            {
                if (_items[i].JustVisual) continue;

                _items[i].RaiseOnMouseLeave(e);
                if (mc_pos.X > x && mc_pos.X < x + _items[i].Width && mc_pos.Y > y && mc_pos.Y < y + _items[i].Height)
                    _items[i].RaiseOnMouseEnter(e);

                if (Orientation == Forms.Orientation.Horizontal)
                    x += _items[i].Width;
                if (Orientation == Forms.Orientation.Vertical)
                    y += _items[i].Height;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].JustVisual) continue;
                _items[i].RaiseOnMouseLeave(e);
            }
        }
        protected override void OnMouseUp(MouseEventArgs e) // Click.
        {
            base.OnMouseUp(e);

            int prevSelected = -1;
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Selected) prevSelected = i;
                _items[i].Selected = false;
            }

            var mc_pos = e.Location;
            for (int i = 0, x = Padding.Left, y = Padding.Top; i < _items.Count; i++)
            {
                if (_items[i].JustVisual) continue;

                if (mc_pos.X > x && mc_pos.X < x + _items[i].Width && mc_pos.Y > y && mc_pos.Y < y + _items[i].Height)
                {
                    if (i != prevSelected)
                        _items[i].Selected = true;

                    ItemClicked(this, new ToolStripItemClickedEventArgs(_items[i]));
                    _items[i].RaiseOnMouseUp(e);
                    break;
                }

                if (Orientation == Forms.Orientation.Horizontal)
                    x += _items[i].Width;
                if (Orientation == Forms.Orientation.Vertical)
                    y += _items[i].Height;
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case UnityEngine.KeyCode.DownArrow: break;
                case UnityEngine.KeyCode.LeftArrow: break;
                case UnityEngine.KeyCode.RightArrow: break;
                case UnityEngine.KeyCode.UpArrow: break;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (OwnerItem != null && OwnerItem.Parent != null && OwnerItem.Parent.Orientation == Forms.Orientation.Horizontal && uwfShadowHandler == null)
            {
                MakeShadow();
            }

            base.OnPaint(e);

            p_args.Graphics = e.Graphics;
            p_args.ClipRectangle = e.ClipRectangle;
            p_args.Graphics.FillRectangle(brushBack, 0, 0, Width, Height);

            if (Orientation == Forms.Orientation.Vertical)
                p_args.Graphics.DrawLine(new Drawing.Pen(Drawing.Color.FromArgb(215, 215, 215)), 24, 2, 24, Height - 2);

            for (int i = 0, x = Padding.Left, y = Padding.Top; i < _items.Count; i++)
            {
                var item = _items[i];
                p_args.ClipRectangle = new Drawing.Rectangle(x, y, item.Width, item.Height);
                item.RaiseOnPaint(p_args);

                if (item.JustVisual) continue;
                if (Orientation == Forms.Orientation.Horizontal)
                    x += item.Width;
                if (Orientation == Forms.Orientation.Vertical)
                    y += item.Height;
            }

            p_args.Graphics.uwfDrawRectangle(BorderColor, 0, 0, Width, Height);
        }
        protected override object uwfOnPaintEditor(float width)
        {
            var control = base.uwfOnPaintEditor(width);

#if UNITY_EDITOR
            Editor.NewLine(1);
            Editor.ColorField("BorderColor", BorderColor, new Action<Color>((c) => { BorderColor = c; }));
            Editor.Label("Orientation", Orientation);
            Editor.Label("OwnerItem", OwnerItem);
#endif

            return control;
        }

        public event ToolStripItemClickedEventHandler ItemClicked = delegate { };

        public void ResetSelected()
        {
            for (int i = 0; i < _items.Count; i++)
                _items[i].Selected = false;
        }

        /// <summary>
        /// For menu strips.
        /// </summary>
        internal void MakeShadow()
        {
            uwfShadowHandler = (g) =>
            {
                var loc = PointToScreen(Point.Zero);
                var color = Color.FromArgb(12, 64, 64, 64);
                g.Graphics.uwfFillRectangle(color, loc.X - 3, loc.Y, Width + 6, Height + 3);
                g.Graphics.uwfFillRectangle(color, loc.X - 2, loc.Y, Width + 4, Height + 2);
                g.Graphics.uwfFillRectangle(color, loc.X - 1, loc.Y, Width + 2, Height + 1);
            };
        }
    }
}
