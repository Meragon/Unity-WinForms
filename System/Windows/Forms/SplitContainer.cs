using System.Drawing;

namespace System.Windows.Forms
{
    public class SplitContainer : ContainerControl
    {
        private readonly SplitterPanel panel1;
        private readonly SplitterPanel panel2;
        
        private BorderStyle borderStyle = BorderStyle.None;
        private Orientation orientation = Orientation.Vertical;
        private int panel1MinSize = 25;
        private int panel2MinSize = 25;
        private bool panel1WasEnabled;
        private bool panel2WasEnabled;
        private int splitterDistance = 50;
        private bool splitterFixed;
        private bool splitterMoving;
        private int splitterMovingOffset;
        private int splitterWidth = 4;

        public SplitContainer()
        {
            panel1 = new SplitterPanel(this);
            panel2 = new SplitterPanel(this);
            
            Controls.Add(panel1);
            Controls.Add(panel2);
            
            UpdateSplitter();
            
            Application.UpdateEvent += ApplicationOnUpdateEvent;
            MouseHook.MouseUp += MouseHookOnMouseUp;
        }
        
        public BorderStyle BorderStyle
        {
            get { return borderStyle; }
            set
            {
                if (borderStyle == value)
                    return;

                borderStyle = value;
                
                panel1.BorderStyle = borderStyle;
                panel2.BorderStyle = borderStyle;
            }
        }
        public Orientation Orientation
        {
            get { return orientation; }
            set
            {
                if (orientation == value)
                    return;

                orientation = value;
                
                UpdateSplitter();
            }
        }
        public bool IsSplitterFixed 
        {
            get { return splitterFixed; }
            set { splitterFixed = value; }
        }
        public SplitterPanel Panel1
        {
            get { return panel1; }
        }
        public bool Panel1Collapsed {
            get { return panel1.collapsed; }
            set {
                if (panel1.collapsed == value) 
                    return;
                
                if (panel2.collapsed && value)
                    CollapsePanel(panel2, false);
                    
                CollapsePanel(panel1, value);
            }
        }
        public int Panel1MinSize {
            get {  return panel1MinSize; }
            set 
            { 
                if (panel1MinSize == value)
                    return;
                
                if (value < 0) 
                    throw new ArgumentOutOfRangeException("value");
                
                panel1MinSize = value;
            }
        }
        public SplitterPanel Panel2
        {
            get { return panel2; }
        }
        public bool Panel2Collapsed {
            get { return panel2.collapsed; }
            set {
                if (panel2.collapsed == value) 
                    return;

                if (panel1.collapsed && value)
                    CollapsePanel(panel1, false);
                
                CollapsePanel(panel2, value);
            }
        }
        public int Panel2MinSize
        {
            get { return panel2MinSize; }
            set
            {
                if (panel2MinSize == value)
                    return;
                
                if (value < 0) 
                    throw new ArgumentOutOfRangeException("value");

                panel2MinSize = value;
            }
        }
        public int SplitterDistance
        {
            get { return splitterDistance; }
            set
            {
                if (splitterDistance == value)
                    return;

                splitterDistance = value;
                UpdateSplitter();
            }
        }
        public int SplitterWidth
        {
            get { return splitterWidth; }
            set
            {
                if (splitterWidth == value)
                    return;

                splitterWidth = value;
                UpdateSplitter();
            }
        }
        
        protected override Size DefaultSize
        {
            get { return new Size(150, 100); }
        }
        
        protected override void Dispose(bool release_all)
        {
            base.Dispose(release_all);
            
            Application.UpdateEvent -= ApplicationOnUpdateEvent;
            MouseHook.MouseUp -= MouseHookOnMouseUp;
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (splitterFixed == false)
            {
                if (Orientation == Orientation.Vertical && e.X >= splitterDistance && e.X <= splitterDistance + splitterWidth ||
                    Orientation == Orientation.Horizontal && e.Y >= splitterDistance && e.Y <= splitterDistance + splitterWidth)
                {
                    splitterMoving = true;
                    panel1WasEnabled = panel1.Enabled;
                    panel2WasEnabled = panel2.Enabled;

                    if (Orientation == Orientation.Vertical)
                    {
                        splitterMovingOffset = e.X - splitterDistance;
                        Cursor.Current = Cursors.VSplit;
                    }
                    else
                    {
                        splitterMovingOffset = e.Y - splitterDistance;
                        Cursor.Current = Cursors.HSplit;
                    }
                }
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            UpdateSplitter();
        }
        
        private void ApplicationOnUpdateEvent()
        {
            var mclient = PointToClient(MousePosition);
            if (splitterMoving)
            {
                if (Orientation == Orientation.Vertical)
                    splitterDistance = mclient.X - splitterMovingOffset;
                else
                    splitterDistance = mclient.Y - splitterMovingOffset;

                UpdateSplitter();
            }
            else if (ClientRectangle.Contains(mclient))
            {
                Cursor.Current = null;
                
                if (Orientation == Orientation.Vertical)
                {
                    if (mclient.X >= splitterDistance &&
                        mclient.X <= splitterDistance + splitterWidth)
                        Cursor.Current = Cursors.VSplit;
                }
                else 
                {
                    if (mclient.Y >= splitterDistance &&
                        mclient.Y <= splitterDistance + splitterWidth)
                        Cursor.Current = Cursors.HSplit;
                }
            }
        }
        private void CollapsePanel(SplitterPanel p, bool collapsing) 
        {
            p.collapsed = collapsing;
            p.Visible = !collapsing;

            UpdateSplitter();
        }
        private void MouseHookOnMouseUp(object sender, MouseEventArgs e)
        {
            if (splitterMoving)
            {
                panel1.Enabled = panel1WasEnabled;
                panel2.Enabled = panel2WasEnabled;
                splitterMoving = false;
                Cursor.Current = null;
            }            
        }
        private void UpdateSplitter()
        {
            if (Orientation == Orientation.Vertical)
                splitterDistance = MathHelper.Clamp(splitterDistance, panel1MinSize, Width - panel2MinSize - splitterWidth);
            else if (Orientation == Orientation.Horizontal)
                splitterDistance = MathHelper.Clamp(splitterDistance, panel1MinSize, Height - panel2MinSize - splitterWidth);
            
            panel1.SuspendLayout();
            panel2.SuspendLayout();

            if (Panel1Collapsed == false && Panel2Collapsed == false)
            {
                if (Orientation == Orientation.Vertical)
                {
                    panel1.Location = new Point();
                    panel1.Size = new Size(splitterDistance, Height);
                    panel2.Location = new Point(splitterDistance + splitterWidth, 0);
                    panel2.Size = new Size(Width - splitterDistance - splitterWidth, Height);
                }
                else
                {
                    panel1.Location = new Point();
                    panel1.Size = new Size(Width, splitterDistance);
                    panel2.Location = new Point(0, splitterDistance + splitterWidth);
                    panel2.Size = new Size(Width, Height - splitterDistance - splitterWidth);
                }
            }
            else
            {
                if (Panel1Collapsed)
                {
                    panel2.Location = new Point();
                    panel2.Size = Size;
                }
                else if (Panel2Collapsed)
                {
                    panel1.Location = new Point();
                    panel1.Size = Size;
                }
            }

            panel1.ResumeLayout();
            panel2.ResumeLayout();
        }
    }
}