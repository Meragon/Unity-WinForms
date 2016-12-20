using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class Screen
    {
        private static Screen primaryScreen;
        private Rectangle workingArea;

        public static Screen PrimaryScreen
        {
            get
            {
                if (primaryScreen == null)
                {
                    primaryScreen = new Screen();
                    primaryScreen.workingArea = new Rectangle(0, 0, (int)(UnityEngine.Screen.width / Application.ScaleX), (int)(UnityEngine.Screen.height / Application.ScaleY));
                }
                else
                {
                    if (primaryScreen.workingArea.Width != (UnityEngine.Screen.width / Application.ScaleX) ||
                        primaryScreen.workingArea.Height != (UnityEngine.Screen.height / Application.ScaleY))
                        primaryScreen.workingArea = new Rectangle(0, 0, (int)(UnityEngine.Screen.width / Application.ScaleX), (int)(UnityEngine.Screen.height / Application.ScaleY));
                }
                return primaryScreen;
            }
        }
        public Rectangle WorkingArea { get { return workingArea; } }
    }
}
