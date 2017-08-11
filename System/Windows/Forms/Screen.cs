namespace System.Windows.Forms
{
    using System.Drawing;

    public class Screen
    {
        internal static int height;
        internal static int width;

        private static Screen primaryScreen;
        private Rectangle workingArea;

        public static Screen PrimaryScreen
        {
            get
            {
                if (primaryScreen == null)
                {
                    primaryScreen = new Screen();
                    primaryScreen.workingArea = new Rectangle(0, 0, width, height);
                }
                else
                {
                    if (primaryScreen.workingArea.Width != width ||
                        primaryScreen.workingArea.Height != height)
                        primaryScreen.workingArea = new Rectangle(0, 0, width, height);
                }
                return primaryScreen;
            }
        }
        public Rectangle WorkingArea { get { return workingArea; } }
    }
}
