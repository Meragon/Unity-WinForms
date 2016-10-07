using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace System.Windows.Forms
{
    public abstract class ScrollBar : Control
    {
        private int largeChange = 10;
        private int maximum = 100;
        private int minimum = 0;
        protected ScrollOrientation scrollOrientation;
        private Rectangle scrollRect;
        private int smallChange = 1;
        private int value = 0;

        internal Button addButton;
        internal Button subtractButton;

        public int LargeChange
        {
            get { return largeChange; }
            set { largeChange = value; }
        }
        public int Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }
        public int Minimum
        {
            get { return minimum; }
            set { minimum = value; }
        }
        public Color ScrollColor { get; set; }
        public int SmallChange
        {
            get { return smallChange; }
            set { smallChange = value; }
        }
        public int Value
        {
            get { return value; }
            set
            {
                bool changed = this.value != value;
                this.value = value;
                if (changed)
                {
                    UpdateScrollRect();
                    OnValueChanged(null);
                }
            }
        }

        public ScrollBar()
        {
            ScrollColor = Color.FromArgb(205, 205, 205);

            var backColor = Color.Transparent;
            var backHoverColor = Color.FromArgb(218, 218, 218);
            var borderColor = Color.Transparent;
            var borderHoverColor = Color.Transparent;
            var imageColor = Color.FromArgb(96, 96, 96);
            var imageHoverColor = Color.Black;

            addButton = new RepeatButton();
            addButton.CanSelect = false;
            addButton.BorderHoverColor = borderHoverColor;
            addButton.HoverColor = backHoverColor;
            addButton.ImageColor = imageColor;
            addButton.ImageHoverColor = imageHoverColor;
            addButton.BorderColor = borderColor;
            addButton.BackColor = backColor;
            addButton.Click += (s, a) => { DoScroll(ScrollEventType.SmallIncrement); };
            Controls.Add(addButton);

            subtractButton = new RepeatButton();
            subtractButton.CanSelect = false;
            subtractButton.BorderHoverColor = borderHoverColor;
            subtractButton.HoverColor = backHoverColor;
            subtractButton.ImageColor = imageColor;
            subtractButton.ImageHoverColor = imageHoverColor;
            subtractButton.BorderColor = borderColor;
            subtractButton.BackColor = backColor;
            subtractButton.Click += (s, a) => { DoScroll(ScrollEventType.SmallDecrement); };
            Controls.Add(subtractButton);
        }

        private void DoScroll(ScrollEventType type)
        {
            int newValue = value;
            int oldValue = value;

            switch (type)
            {
                case ScrollEventType.First:
                    newValue = minimum;
                    break;

                case ScrollEventType.Last:
                    newValue = maximum - LargeChange + 1;
                    break;

                case ScrollEventType.SmallDecrement:
                    newValue = Math.Max(value - SmallChange, minimum);
                    break;

                case ScrollEventType.SmallIncrement:
                    newValue = Math.Min(value + SmallChange, maximum - LargeChange + 1);
                    break;

                case ScrollEventType.LargeDecrement:
                    newValue = Math.Max(value - LargeChange, minimum);
                    break;

                case ScrollEventType.LargeIncrement:
                    newValue = Math.Min(value + LargeChange, maximum - LargeChange + 1);
                    break;

                case ScrollEventType.ThumbPosition:
                case ScrollEventType.ThumbTrack:

                    Application.Log("not implemented yet");
                    break;
            }

            ScrollEventArgs se = new ScrollEventArgs(type, oldValue, newValue, this.scrollOrientation);
            OnScroll(se);
            Value = se.NewValue;
        }
        protected void UpdateScrollRect()
        {
            float sx = 0;
            float sy = 0;
            float sw = 0;
            float sh = 0;

            int scrollLength = 0;
            if (scrollOrientation == ScrollOrientation.HorizontalScroll)
                scrollLength = addButton.Location.X - subtractButton.Location.X - subtractButton.Width;
            else
                scrollLength = addButton.Location.Y - subtractButton.Location.Y - subtractButton.Height;

            int valueLength = maximum - minimum;
            float valueK = (float)(Value - minimum) / valueLength;
            float scrollPos = scrollLength * valueK;

            if (scrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                sx = subtractButton.Location.X + subtractButton.Width + scrollPos;
                sy = 0;
                sw = 4;
                sh = Height;
            }
            else
            {
                sx = 0;
                sy = subtractButton.Location.Y + subtractButton.Height + scrollPos;
                sw = Width;
                sh = 4;
            }

            scrollRect = new Rectangle((int)sx, (int)sy, (int)sw, (int)sh);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var wheelDelta = e.Delta;

            bool scrolled = false;

            while (wheelDelta != 0)
            {
                if (wheelDelta > 0)
                {
                    DoScroll(ScrollEventType.SmallDecrement);
                    wheelDelta -= smallChange;
                    scrolled = true;
                    if (wheelDelta <= 0)
                        break;
                }
                else
                {
                    DoScroll(ScrollEventType.SmallIncrement);
                    wheelDelta += smallChange;
                    scrolled = true;
                    if (wheelDelta >= 0)
                        break;
                }
            }

            if (scrolled)
                DoScroll(ScrollEventType.EndScroll);

            base.OnMouseWheel(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(ScrollColor, scrollRect.X, scrollRect.Y, scrollRect.Width, scrollRect.Height);
        }
        protected override object OnPaintEditor(float width)
        {
            var component = base.OnPaintEditor(width);
#if UNITY_EDITOR
            Editor.Label("Value", Value);
#endif
            return component;
        }
        protected virtual void OnScroll(ScrollEventArgs se)
        {
            Scroll(this, se);
        }
        protected virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged(this, e);
        }
        public override void Refresh()
        {
            base.Refresh();

            UpdateScrollRect();
        }

        public event ScrollEventHandler Scroll = delegate { };
        public event EventHandler ValueChanged = delegate { };
    }
}
