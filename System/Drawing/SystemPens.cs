namespace System.Drawing
{
    public static class SystemPens
    {
        private static Pen activeBorder;
        private static Pen activeCaption;
        private static Pen activeCaptionText;
        private static Pen appWorkspace;
        private static Pen buttonFace;
        private static Pen buttonHighlight;
        private static Pen buttonShadow;
        private static Pen control;
        private static Pen controlText;
        private static Pen controlDark;
        private static Pen controlDarkDark;
        private static Pen controlLight;
        private static Pen controlLightLight;
        private static Pen desktop;
        private static Pen gradientActiveCaption;
        private static Pen gradientInactiveCaption;
        private static Pen grayText;
        private static Pen highlight;
        private static Pen highlightText;
        private static Pen hotTrack;
        private static Pen inactiveBorder;
        private static Pen inactiveCaption;
        private static Pen inactiveCaptionText;
        private static Pen info;
        private static Pen infoText;
        private static Pen menu;
        private static Pen menuBar;
        private static Pen menuHighlight;
        private static Pen menuText;
        private static Pen scrollBar;
        private static Pen window;
        private static Pen windowFrame;
        private static Pen windowText;

        public static Pen ActiveBorder
        {
            get
            {
                if (activeBorder == null)
                    activeBorder = new Pen(SystemColors.ActiveBorder);

                return activeBorder;
            }
        }
        public static Pen ActiveCaption
        {
            get
            {
                if (activeCaption == null)
                    activeCaption = new Pen(SystemColors.ActiveCaption);

                return activeCaption;
            }
        }
        public static Pen ActiveCaptionText
        {
            get
            {
                if (activeCaptionText == null)
                    activeCaptionText = new Pen(SystemColors.ActiveCaptionText);

                return activeCaptionText;
            }
        }
        public static Pen AppWorkspace
        {
            get
            {
                if (appWorkspace == null)
                    appWorkspace = new Pen(SystemColors.AppWorkspace);

                return appWorkspace;
            }
        }
        public static Pen ButtonFace
        {
            get
            {
                if (buttonFace == null)
                    buttonFace = new Pen(SystemColors.ButtonFace);

                return buttonFace;
            }
        }
        public static Pen ButtonHighlight
        {
            get
            {
                if (buttonHighlight == null)
                    buttonHighlight = new Pen(SystemColors.ButtonHighlight);

                return buttonHighlight;
            }
        }
        public static Pen ButtonShadow
        {
            get
            {
                if (buttonShadow == null)
                    buttonShadow = new Pen(SystemColors.ButtonShadow);

                return buttonShadow;
            }
        }
        public static Pen Control
        {
            get
            {
                if (control == null)
                    control = new Pen(SystemColors.Control);

                return control;
            }
        }
        public static Pen ControlText
        {
            get
            {
                if (controlText == null)
                    controlText = new Pen(SystemColors.ControlText);

                return controlText;
            }
        }
        public static Pen ControlDark
        {
            get
            {
                if (controlDark == null)
                    controlDark = new Pen(SystemColors.ControlDark);

                return controlDark;
            }
        }
        public static Pen ControlDarkDark
        {
            get
            {
                if (controlDarkDark == null)
                    controlDarkDark = new Pen(SystemColors.ControlDarkDark);

                return controlDarkDark;
            }
        }
        public static Pen ControlLight
        {
            get
            {
                if (controlLight == null)
                    controlLight = new Pen(SystemColors.ControlLight);

                return controlLight;
            }
        }
        public static Pen ControlLightLight
        {
            get
            {
                if (controlLightLight == null)
                    controlLightLight = new Pen(SystemColors.ControlLightLight);

                return controlLightLight;
            }
        }
        public static Pen Desktop
        {
            get
            {
                if (desktop == null)
                    desktop = new Pen(SystemColors.Desktop);

                return desktop;
            }
        }
        public static Pen GradientActiveCaption
        {
            get
            {
                if (gradientActiveCaption == null)
                    gradientActiveCaption = new Pen(SystemColors.GradientActiveCaption);

                return gradientActiveCaption;
            }
        }
        public static Pen GradientInactiveCaption
        {
            get
            {
                if (gradientInactiveCaption == null)
                    gradientInactiveCaption = new Pen(SystemColors.GradientInactiveCaption);

                return gradientInactiveCaption;
            }
        }
        public static Pen GrayText
        {
            get
            {
                if (grayText == null)
                    grayText = new Pen(SystemColors.GrayText);

                return grayText;
            }
        }
        public static Pen Highlight
        {
            get
            {
                if (highlight == null)
                    highlight = new Pen(SystemColors.Highlight);

                return highlight;
            }
        }
        public static Pen HighlightText
        {
            get
            {
                if (highlightText == null)
                    highlightText = new Pen(SystemColors.HighlightText);

                return highlightText;
            }
        }
        public static Pen HotTrack
        {
            get
            {
                if (hotTrack == null)
                    hotTrack = new Pen(SystemColors.HotTrack);

                return hotTrack;
            }
        }
        public static Pen InactiveBorder
        {
            get
            {
                if (inactiveBorder == null)
                    inactiveBorder = new Pen(SystemColors.InactiveBorder);

                return inactiveBorder;
            }
        }
        public static Pen InactiveCaption
        {
            get
            {
                if (inactiveCaption == null)
                    inactiveCaption = new Pen(SystemColors.InactiveCaption);

                return inactiveCaption;
            }
        }
        public static Pen InactiveCaptionText
        {
            get
            {
                if (inactiveCaptionText == null)
                    inactiveCaptionText = new Pen(SystemColors.InactiveCaptionText);

                return inactiveCaptionText;
            }
        }
        public static Pen Info
        {
            get
            {
                if (info == null)
                    info = new Pen(SystemColors.Info);

                return info;
            }
        }
        public static Pen InfoText
        {
            get
            {
                if (infoText == null)
                    infoText = new Pen(SystemColors.InfoText);

                return infoText;
            }
        }
        public static Pen Menu
        {
            get
            {
                if (menu == null)
                    menu = new Pen(SystemColors.Menu);

                return menu;
            }
        }
        public static Pen MenuBar
        {
            get
            {
                if (menuBar == null)
                    menuBar = new Pen(SystemColors.MenuBar);

                return menuBar;
            }
        }
        public static Pen MenuHighlight
        {
            get
            {
                if (menuHighlight == null)
                    menuHighlight = new Pen(SystemColors.MenuHighlight);

                return menuHighlight;
            }
        }
        public static Pen MenuText
        {
            get
            {
                if (menuText == null)
                    menuText = new Pen(SystemColors.MenuText);

                return menuText;
            }
        }
        public static Pen ScrollBar
        {
            get
            {
                if (scrollBar == null)
                    scrollBar = new Pen(SystemColors.ScrollBar);
                
                return scrollBar;
            }
        }
        public static Pen Window
        {
            get
            {
                if (window == null)
                    window = new Pen(SystemColors.Window);

                return window;
            }
        }
        public static Pen WindowFrame
        {
            get
            {
                if (windowFrame == null)
                    windowFrame = new Pen(SystemColors.WindowFrame);

                return windowFrame;
            }
        }
        public static Pen WindowText
        {
            get
            {
                if (windowText == null)
                    windowText = new Pen(SystemColors.WindowText);

                return windowText;
            }
        }
    }
}
