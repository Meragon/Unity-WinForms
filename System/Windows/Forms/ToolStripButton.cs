namespace System.Windows.Forms
{
    using System.Drawing;

    //// Default size is 23, 22.
    public class ToolStripButton : ToolStripItem
    {
        private CheckState checkState = CheckState.Unchecked;
        private bool checkOnClick;
        
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

        public event EventHandler CheckedChanged;
        public event EventHandler CheckStateChanged;

        public override bool CanSelect
        {
            get { return true; }
        }
        public bool Checked
        {
            get { return checkState != CheckState.Unchecked; }
            set
            {
                if (value == Checked)
                    return;

                CheckState = value ? CheckState.Checked : CheckState.Unchecked;
            }
        }
        public CheckState CheckState
        {
            get { return checkState; }
            set
            {
                if (value == checkState)
                    return;

                checkState = value;
                OnCheckedChanged(EventArgs.Empty);
                OnCheckStateChanged(EventArgs.Empty);
            }
        }
        public bool CheckOnClick {
            get { return checkOnClick; }
            set { checkOnClick = value; }
        }

        protected internal override void SetBounds(Rectangle nbounds)
        {
            if (AutoSize && nbounds.Width != 23) // Always set width = 23 if autosize?
                nbounds = new Rectangle(nbounds.X, nbounds.Y, 23, nbounds.Height);

            base.SetBounds(nbounds);
        }

        protected override void OnClick(EventArgs e)
        {
            if (checkOnClick)
                Checked = !Checked;
            
            base.OnClick(e);
        }
        protected virtual void OnCheckedChanged(EventArgs e) {
            var handler = CheckedChanged;
            if (handler != null) 
                handler(this,e);
        }
        protected virtual void OnCheckStateChanged(EventArgs e)
        {
            var handler = CheckStateChanged;
            if (handler != null) 
                handler(this,e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (Checked)
                e.Graphics.DrawRectangle(selectPen, Bounds);
        }
    }
}