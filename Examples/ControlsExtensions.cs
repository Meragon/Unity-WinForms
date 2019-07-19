namespace UnityWinForms.Examples
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    [Flags]
    public enum AnchorStylesExtended
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,

        BottomTop = TopBottom,
        BottomTopLeft = TopBottomLeft,
        BottomTopRight = TopBottomRight,
        BottomLeft = Bottom | Left,
        BottomLeftRight = BottomLeft | Right,
        BottomLeftTop = BottomTopLeft,
        BottomRight = Bottom | Right,
        BottomRightLeft = BottomRight | Left,
        BottomRightTop = BottomRight | Top,

        LeftBottom = BottomLeft,
        LeftBottomRight = BottomLeftRight,
        LeftBottomTop = BottomLeftTop,
        LeftRight = Left | Right,
        LeftRightBottom = BottomLeftRight,
        LeftRightTop = TopLeftRight,
        LeftTop = TopLeft,
        LeftTopBottom = BottomLeftTop,
        LeftTopRight = TopLeftRight,

        RightBottom = BottomRight,
        RightBottomLeft = BottomRightLeft,
        RightBottomTop = TopRightBottom,
        RightLeft = LeftRight,
        RightLeftBottom = LeftBottomRight,
        RightLeftTop = LeftTopRight,
        RightTop = TopRight,
        RightTopBottom = TopBottomRight,
        RightTopLeft = TopLeftRight,

        TopBottom = Top | Bottom,
        TopBottomLeft = TopBottom | Left,
        TopBottomRight = TopBottom | Right,
        TopLeft = Top | Left,
        TopLeftBottom = TopLeft | Bottom,
        TopLeftRight = TopLeft | Right,
        TopRight = Top | Right,
        TopRightBottom = TopRight | Bottom,
        TopRightLeft = TopLeftRight,

        All = TopBottomLeft | Right,
    }

    public static class ControlsExtensions
    {
        public static T Create<T>(this Control parent, int margin = 8, int lineHeight = 24) where T : Control, new()
        {
            return Create<T>(parent, AnchorStylesExtended.LeftTop, string.Empty, false, margin, lineHeight);
        }
        public static T Create<T>(this Control parent, AnchorStylesExtended anchor, bool placeToRight = false, int margin = 8, int lineHeight = 24) where T : Control, new()
        {
            return Create<T>(parent, anchor, null, placeToRight, margin, lineHeight);
        }
        public static T Create<T>(this Control parent, string text, bool placeToRight = false, int margin = 8, int lineHeight = 24) where T : Control, new()
        {
            return Create<T>(parent, AnchorStylesExtended.LeftTop, text, placeToRight, margin, lineHeight);
        }
        public static T Create<T>(
            this Control parent,
            AnchorStylesExtended anchor,
            string text,
            bool placeToRight = false,
            int margin = 8,
            int lineHeight = 24)
            where T : Control, new()
        {
            if (parent == null)
                throw new ArgumentNullException("parent");


            var topOffset = margin;
            var leftOffset = margin;

            var parentForm = parent as Form;
            if (parentForm != null)
                topOffset += parentForm.uwfHeaderHeight;

            var parentControls = parent.Controls;
            if (parentControls.Count > 0)
            {
                Control lastChild = null;
                for (int i = parentControls.Count - 1; i >= 0; i--)
                {
                    var child = parentControls[i];
                    if (child.uwfSystem)
                        continue;

                    lastChild = child;
                    break;
                }

                if (lastChild != null)
                {
                    var lastChildY = lastChild.Location.Y;
                    {
                        if (placeToRight)
                        {
                            leftOffset = lastChild.Location.X + lastChild.Width + margin;
                            topOffset = lastChildY;
                        }
                        else if (lastChildY >= topOffset)
                            topOffset = lastChildY + lineHeight;
                    }
                }
            }

            var control = new T();
            control.Anchor = (AnchorStyles)anchor;
            control.Location = new Point(leftOffset, topOffset);
            control.Text = text;

            parentControls.Add(control);

            return control;
        }
        public static T FillRight<T>(this T control, int margin = 8) where T : Control
        {
            var parent = control.Parent;
            if (parent == null)
                throw new ArgumentException("parent");

            control.Width = parent.Width - margin - control.Location.X;

            return control;
        }
        public static T SetHeight<T>(this T control, int height) where T : Control
        {
            control.Height = height;

            return control;
        }
        public static T SetWidth<T>(this T control, int width) where T : Control
        {
            control.Width = width;

            return control;
        }
    }
}
