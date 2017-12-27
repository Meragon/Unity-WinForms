namespace UnityWinForms.Examples.Panels
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public abstract class BaseExamplePanel : Panel
    {
        protected int lineOffset = 36;

        // Panels are not supporting scrolling. This basic is workaround.
        private readonly VScrollBar vScrollBar;

        protected BaseExamplePanel()
        {
            BackColor = Color.FromArgb(239, 235, 233);

            vScrollBar = new VScrollBar();
            vScrollBar.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            vScrollBar.Location = new Point(Width - vScrollBar.Width, 0);
            vScrollBar.Height = Height;
            vScrollBar.ValueChanged += VScrollBarOnValueChanged;

            Controls.Add(vScrollBar);
        }

        private int ContentHeight
        {
            get
            {
                int bottom = Height;
                for (int i = 0; i < Controls.Count; i++)
                {
                    var item = Controls[i];
                    if (item == null || item == vScrollBar)
                        continue;

                    if (item.Location.Y + item.Height > bottom)
                        bottom = item.Location.Y + item.Height;
                }

                if (bottom > Height)
                    bottom += 8;

                return bottom;
            }
        }

        public abstract void Initialize();
        public void UpdateScroll()
        {
            var contentHeight = ContentHeight;

            vScrollBar.Visible = contentHeight > Height;
            vScrollBar.Maximum = contentHeight;
            vScrollBar.LargeChange = Height;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (vScrollBar.Visible)
                vScrollBar.Value -= e.Delta;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            UpdateScroll();
        }

        private void VScrollBarOnValueChanged(object sender, EventArgs eventArgs)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                var item = Controls[i];
                if (item == null || item == vScrollBar)
                    continue;

                item.uwfOffset = new Point(0, -vScrollBar.Value);
            }
        }
    }
}
