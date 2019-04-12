namespace System.Windows.Forms
{
    using System.Drawing;

    public static class SystemInformation
    {
        private static Size border3DSize = new Size(2, 2);
        private static int captionHeight = 24; // 23 original + 1 px form border.
        private static int horizontalScrollBarArrowWidth = 17;
        private static int horizontalScrollBarHeight = 15; // 17 original (1px left border + 1px right padding?), same for vertical. 
        private static int mouseWheelScrollLines = 3;
        private static int verticalScrollBarArrowHeight = 17;
        private static int verticalScrollBarWidth = 15;
        
        public static Size Border3DSize
        {
            get { return border3DSize; }
            internal set { border3DSize = value; }
        }
        public static int CaptionHeight
        {
            get { return captionHeight; }
            internal set { captionHeight = value; }
        }
        public static bool HighContrast { get; internal set; }
        public static int HorizontalScrollBarArrowWidth
        {
            get { return horizontalScrollBarArrowWidth; }
            internal set { horizontalScrollBarArrowWidth = value; }
        }
        public static int HorizontalScrollBarHeight
        {
            get { return horizontalScrollBarHeight; }
            internal set { horizontalScrollBarHeight = value; }
        }
        public static int MouseWheelScrollLines
        {
            get { return mouseWheelScrollLines; }
            internal set { mouseWheelScrollLines = value; }
        }
        public static int VerticalScrollBarArrowHeight
        {
            get { return verticalScrollBarArrowHeight; }
            internal set { verticalScrollBarArrowHeight = value; }
        }
        public static int VerticalScrollBarWidth
        {
            get { return verticalScrollBarWidth; }
            internal set { verticalScrollBarWidth = value; }
        }
    }
}
