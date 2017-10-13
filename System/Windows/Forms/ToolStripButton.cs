namespace System.Windows.Forms
{
    using System.Drawing;

    public class ToolStripButton : ToolStripItem
    {
        //// Default size is 23, 22.

        public ToolStripButton()
        {
        }
        public ToolStripButton(string text) : base(text, null, null)
        {
        }
        public ToolStripButton(Image image) : base(null, image, null)
        {
        }
        public ToolStripButton(string text, Image image) : base(text, image, null)
        {
        }
        public ToolStripButton(string text, Image image, EventHandler onClick) : base(text, image, onClick)
        {
        }
        public ToolStripButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {
        }

        public override bool CanSelect
        {
            get { return true; }
        }

        protected internal override void SetBounds(Rectangle nbounds)
        {
            if (AutoSize && nbounds.Width != 23) // Always set width = 23 if autosize?
                nbounds = new Rectangle(nbounds.X, nbounds.Y, 23, nbounds.Height);

            base.SetBounds(nbounds);
        }
    }
}