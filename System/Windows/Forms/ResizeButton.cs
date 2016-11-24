using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    internal class ResizeButton : Button, IResizableControl
    {
        private bool pressed;
        private Form owner;

        public ResizeButton(Form form, Bitmap img)
        {
            owner = form;

            Anchor = AnchorStyles.BottomRight;
            Image = img;
            ImageColor = Color.White;
            ImageHoverColor = Color.FromArgb(0, 122, 204);
            Text = "";
            Height = img.Height;
            Width = img.Width;

            this.ClearColor(Color.Transparent);

            Owner.UpClick += Owner_UpClick;
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

        public override void Dispose()
        {
            base.Dispose();

            Owner.UpClick -= Owner_UpClick;
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
