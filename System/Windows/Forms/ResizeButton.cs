namespace System.Windows.Forms
{
    using System.Drawing;

    internal sealed class ResizeButton : Button, IResizableControl
    {
        private readonly Form owner;
        private bool pressed;

        public ResizeButton(Form form, Bitmap img)
        {
            owner = form;

            Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Image = img;
            uwfImageColor = Color.White;
            uwfImageHoverColor = Color.FromArgb(0, 122, 204);
            Text = "";
            Height = img.Height;
            Width = img.Width;

            SetStyle(ControlStyles.Selectable, false);

            BackColor = Color.Transparent;
            uwfBorderColor = Color.Transparent;
            uwfHoverColor = Color.Transparent;
            uwfBorderHoverColor = Color.Transparent;
            uwfBorderSelectColor = Color.Transparent;

            MouseHook.MouseUp += Owner_UpClick;
        }

        public ControlResizeTypes GetResizeAt(Point mclient)
        {
            return ControlResizeTypes.RightDown;
        }

        protected override void Dispose(bool release_all)
        {
            base.Dispose(release_all);

            MouseHook.MouseUp -= Owner_UpClick;
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

        private void Owner_UpClick(object sender, MouseEventArgs e)
        {
            if (pressed)
                Cursor.CurrentSystem = Cursors.Default;
            pressed = false;
        }
    }
}
