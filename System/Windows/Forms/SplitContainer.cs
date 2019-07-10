namespace System.Windows.Forms
{
    using System.Drawing;
    
    public class SplitContainer : ContainerControl
    {
        private bool cursorWasSet;
        private BorderStyle borderStyle = BorderStyle.None;
        private Orientation orientation = Orientation.Vertical;
        private int panel1MinSize = 25;
        private int panel2MinSize = 25;
        private bool panel1WasEnabled;
        private bool panel2WasEnabled;
        private int splitterDistance = 50;
        private bool splitterMoving;
        private int splitterMovingOffset;
        private int splitterWidth = 4;

        public SplitContainer()
        {
            Panel1 = new SplitterPanel(this);
            Panel2 = new SplitterPanel(this);
            
            Controls.Add(Panel1);
            Controls.Add(Panel2);
            
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
                
                Panel1.BorderStyle = borderStyle;
                Panel2.BorderStyle = borderStyle;
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
        public bool IsSplitterFixed { get; set; }
        public SplitterPanel Panel1 { get; private set; }
        public bool Panel1Collapsed {
            get { return Panel1.collapsed; }
            set {
                if (Panel1.collapsed == value) 
                    return;
                
                if (Panel2.collapsed && value)
                    CollapsePanel(Panel2, false);
                    
                CollapsePanel(Panel1, value);
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
        public SplitterPanel Panel2 { get; private set; }
        public bool Panel2Collapsed {
            get { return Panel2.collapsed; }
            set {
                if (Panel2.collapsed == value) 
                    return;

                if (Panel1.collapsed && value)
                    CollapsePanel(Panel1, false);
                
                CollapsePanel(Panel2, value);
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

            if (!IsSplitterFixed)
            {
                if (Orientation == Orientation.Vertical && e.X >= splitterDistance && e.X <= splitterDistance + splitterWidth ||
                    Orientation == Orientation.Horizontal && e.Y >= splitterDistance && e.Y <= splitterDistance + splitterWidth)
                {
                    splitterMoving = true;
                    panel1WasEnabled = Panel1.Enabled;
                    panel2WasEnabled = Panel2.Enabled;

                    if (Orientation == Orientation.Vertical)
                    {
                        splitterMovingOffset = e.X - splitterDistance;
                        Cursor.Current = Cursors.VSplit;
                        cursorWasSet = true;
                    }
                    else
                    {
                        splitterMovingOffset = e.Y - splitterDistance;
                        Cursor.Current = Cursors.HSplit;
                        cursorWasSet = true;
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
                if (Orientation == Orientation.Vertical)
                {
                    if (mclient.X >= splitterDistance &&
                        mclient.X <= splitterDistance + splitterWidth && hovered)
                    {
                        Cursor.Current = Cursors.VSplit;
                        cursorWasSet = true;
                    }
                    else
                    {
                        Cursor.Current = null;
                        cursorWasSet = false;
                    }
                }
                else
                {
                    if (mclient.Y >= splitterDistance &&
                        mclient.Y <= splitterDistance + splitterWidth && hovered)
                    {
                        Cursor.Current = Cursors.HSplit;
                        cursorWasSet = true;
                    }
                    else
                    {
                        Cursor.Current = null;
                        cursorWasSet = false;
                    }
                }
            }
            else if (cursorWasSet)
            {
                Cursor.Current = null;
                cursorWasSet = false;
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
                Panel1.Enabled = panel1WasEnabled;
                Panel2.Enabled = panel2WasEnabled;
                splitterMoving = false;
                cursorWasSet = false;
                Cursor.Current = null;
            }            
        }
        private void UpdateSplitter()
        {
            if (Orientation == Orientation.Vertical)
                splitterDistance = MathHelper.Clamp(splitterDistance, panel1MinSize, Width - panel2MinSize - splitterWidth);
            else if (Orientation == Orientation.Horizontal)
                splitterDistance = MathHelper.Clamp(splitterDistance, panel1MinSize, Height - panel2MinSize - splitterWidth);
            
            Panel1.SuspendLayout();
            Panel2.SuspendLayout();

            if (Panel1Collapsed == false && Panel2Collapsed == false)
            {
                if (Orientation == Orientation.Vertical)
                {
                    Panel1.Location = new Point();
                    Panel1.Size = new Size(splitterDistance, Height);
                    Panel2.Location = new Point(splitterDistance + splitterWidth, 0);
                    Panel2.Size = new Size(Width - splitterDistance - splitterWidth, Height);
                }
                else
                {
                    Panel1.Location = new Point();
                    Panel1.Size = new Size(Width, splitterDistance);
                    Panel2.Location = new Point(0, splitterDistance + splitterWidth);
                    Panel2.Size = new Size(Width, Height - splitterDistance - splitterWidth);
                }
            }
            else
            {
                if (Panel1Collapsed)
                {
                    Panel2.Location = new Point();
                    Panel2.Size = Size;
                }
                else if (Panel2Collapsed)
                {
                    Panel1.Location = new Point();
                    Panel1.Size = Size;
                }
            }

            Panel1.ResumeLayout();
            Panel2.ResumeLayout();
        }
    }
}