using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    [Serializable]
    public class ScrollableControl : Control
    {
        protected const int ScrollStateAutoScrolling = 1;
        protected const int ScrollStateFullDrag = 16;
        protected const int ScrollStateHScrollVisible = 2;
        protected const int ScrollStateUserHasScrolled = 8;
        protected const int ScrollStateVScrollVisible = 4;

        private HScrollProperties _horizontalScroll;
        private Point _scrollOffset { get; set; }
        private VScrollProperties _verticalScroll;

        public ScrollableControl()
        {

            _horizontalScroll = new HScrollProperties(this);
            _verticalScroll = new VScrollProperties(this);
        }

        public event ScrollEventHandler Scroll = delegate { };

        public virtual bool AutoScroll { get; set; }
        public override Rectangle DisplayRectangle
        {
            get
            {
                return base.DisplayRectangle;
            }
        }
        public HScrollProperties HorizontalScroll { get { return _horizontalScroll; } }
        protected bool HScroll { get; set; }
        public VScrollProperties VerticalScroll { get { return _verticalScroll; } }
        protected bool VScroll { get; set; }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            return;
            //OnScroll(new ScrollEventArgs(ScrollEventType.EndScroll, _verticalScroll.Value, _verticalScroll.Value + e.Delta, ScrollOrientation.VerticalScroll));
        }
        protected virtual void OnScroll(ScrollEventArgs se)
        {
            int _scrollMinX = 0;
            int _scrollMinY = 0;
            int _scrollMaxX = 0;
            int _scrollMaxY = 0;

            bool init = false;
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].Visible == false) continue;
                if (!init)
                {
                    _scrollMinX = Controls[i].Offset.X + Controls[i].Location.X;
                    _scrollMaxX = Controls[i].Offset.X + Controls[i].Location.X + Controls[i].Width;
                    _scrollMinY = Controls[i].Offset.Y + Controls[i].Location.Y;
                    _scrollMaxY = Controls[i].Offset.Y + Controls[i].Location.Y + Controls[i].Height;
                    init = true;
                }

                if (Controls[i].Offset.X + Controls[i].Location.X < _scrollMinX)
                    _scrollMinX = Controls[i].Offset.X + Controls[i].Location.X;
                if (Controls[i].Offset.Y + Controls[i].Location.Y < _scrollMinY)
                    _scrollMinY = Controls[i].Offset.Y + Controls[i].Location.Y;
                if (Controls[i].Offset.X + Controls[i].Location.X + Controls[i].Width > _scrollMaxX)
                    _scrollMaxX = Controls[i].Offset.X + Controls[i].Location.X + Controls[i].Width;
                if (Controls[i].Offset.Y + Controls[i].Location.Y + Controls[i].Height > _scrollMaxY)
                    _scrollMaxY = Controls[i].Offset.Y + Controls[i].Location.Y + Controls[i].Height;
            }

            _scrollRect = new Rectangle(_scrollMinX, _scrollMinY, _scrollMaxX - _scrollMinX, _scrollMaxY - _scrollMinY);
            //UnityEngine.Debug.Log("sr: " + _scrollRect.ToString());
            //UnityEngine.Debug.Log("cr: " + ClientRectangle.ToString());
            if (ClientRectangle.Contains(_scrollRect)) return;
            int _scrollDelta = se.OldValue - se.NewValue;

            switch (se.ScrollOrientation)
            {
                case ScrollOrientation.HorizontalScroll:
                    //if (ScrollOffset.X + _scrollDelta > _scrollMinX &&
                    //    ScrollOffset.X + _scrollDelta < _scrollMaxX)
                    _scrollOffset += new int[] { _scrollDelta, 0 };
                    break;
                case ScrollOrientation.VerticalScroll:
                    bool scrollDown = _scrollDelta < 0;
                    if (scrollDown)
                    {
                        //UnityEngine.Debug.Log(_scrollDelta.ToString());
                        if (_scrollRect.Y + _scrollRect.Height + _scrollDelta < Height - 8)
                            return;
                        //if (_scrollDelta + _scrollMinY + ScrollOffset.Y < 0)
                        //  return;

                        //UnityEngine.Debug.Log("y");
                        /*UnityEngine.Debug.Log("smy: " + _scrollMaxY.ToString() + " sd: " + _scrollDelta.ToString() + " soy: " + ScrollOffset.Y.ToString() +
                            " h: " + Height.ToString());*/
                        //if (_scrollMaxY + ScrollOffset.Y * -1 + _scrollDelta > Height)
                        //  return;
                    }
                    else
                    {
                        if (_scrollRect.Y + _scrollDelta > 8)
                            return;
                        //if (_scrollRect.Y + _scrollRect.Height + _scrollDelta > Height)
                          //  return;
                        //UnityEngine.Debug.Log(_scrollDelta + _scrollMaxY + ScrollOffset.Y);
                        //if (_scrollDelta + _scrollMaxY + ScrollOffset.Y > Height)
                        //  return;
                    }
                    //if (ScrollOffset.Y + _scrollDelta > _scrollMinY &&
                    //    ScrollOffset.Y + _scrollDelta < _scrollMaxY)
                    
                    _scrollOffset -= new int[] { 0, _scrollDelta };
                    break;
            }
            //UnityEngine.Debug.Log(ScrollOffset);
            _SetOffset(_scrollOffset);
            Scroll(this, se);
        }

        private Rectangle _scrollRect;

        protected override void OnLatePaint(PaintEventArgs e)
        {
            base.OnLatePaint(e);
            //var g = e.Graphics;
            //g.DrawRectangle(new Pen(Color.Red), _scrollRect);

            //g.FillRectangle(new SolidBrush(Color.Gray), Width - 12, 24, 8, 32);
        }
        private void _SetOffset(Point offset)
        {
            for (int i = 0; i < Controls.Count; i++)
                Controls[i].Offset = offset;
        }
    }
}
