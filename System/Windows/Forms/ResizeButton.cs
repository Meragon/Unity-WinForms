using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    internal sealed class ResizeButton : Button, IResizableControl
    {
        private bool pressed;
        private readonly Form owner;

        public ResizeButton(Form form, Bitmap img)
        {
            owner = form;

            Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Image = img;
            ImageColor = Color.White;
            ImageHoverColor = Color.FromArgb(0, 122, 204);
            Text = "";
            Height = img.Height;
            Width = img.Width;

            SetStyle(ControlStyles.Selectable, false);

            this.ClearColor(Color.Transparent);

            uwfAppOwner.UpClick += Owner_UpClick;
        }

        private void Owner_UpClick(object sender, MouseEventArgs e)
        {
            if (pressed)
                Cursor.CurrentSystem = Cursors.Default;
            pressed = false;
        }

        public ControlResizeTypes GetResizeAt(Point mclient)
        {
            return ControlResizeTypes.RightDown;
        }

        protected override void Dispose(bool release_all)
        {
            base.Dispose(release_all);

            uwfAppOwner.UpClick -= Owner_UpClick;
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            pressed = true;

            owner.SetResize(ControlResizeTypes.RightDown);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            Cursor.CurrentSystem = Cursors.SizeNWSE;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (pressed == false)
                Cursor.CurrentSystem = Cursors.Default;
        }
    }
}
