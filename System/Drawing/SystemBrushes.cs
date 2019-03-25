namespace System.Drawing
{
    using System.Collections.Generic;
    
    public static class SystemBrushes
    {
        private static readonly Dictionary<Color, Brush> items = new Dictionary<Color, Brush>();
        
        public static Brush ActiveBorder
        {
            get { return FromSystemColor(SystemColors.ActiveBorder); }
        }
        public static Brush ActiveCaption
        {
            get { return FromSystemColor(SystemColors.ActiveCaption); }
        }
        public static Brush ActiveCaptionText
        {
            get { return FromSystemColor(SystemColors.ActiveCaptionText); }
        }
        public static Brush AppWorkspace
        {
            get { return FromSystemColor(SystemColors.AppWorkspace); }
        }
        public static Brush ButtonFace
        {
            get { return FromSystemColor(SystemColors.ButtonFace); }
        }
        public static Brush ButtonHighlight
        {
            get { return FromSystemColor(SystemColors.ButtonHighlight); }
        }
        public static Brush ButtonShadow
        {
            get { return FromSystemColor(SystemColors.ButtonShadow); }
        }
        public static Brush Control
        {
            get { return FromSystemColor(SystemColors.Control); }
        }
        public static Brush ControlLightLight
        {
            get { return FromSystemColor(SystemColors.ControlLightLight); }
        }
        public static Brush ControlLight
        {
            get { return FromSystemColor(SystemColors.ControlLight); }
        }
        public static Brush ControlDark
        {
            get { return FromSystemColor(SystemColors.ControlDark); }
        }
        public static Brush ControlDarkDark
        {
            get { return FromSystemColor(SystemColors.ControlDarkDark); }
        }
        public static Brush ControlText
        {
            get { return FromSystemColor(SystemColors.ControlText); }
        }
        public static Brush Desktop
        {
            get { return FromSystemColor(SystemColors.Desktop); }
        }
        public static Brush GradientActiveCaption
        {
            get { return FromSystemColor(SystemColors.GradientActiveCaption); }
        }
        public static Brush GradientInactiveCaption
        {
            get { return FromSystemColor(SystemColors.GradientInactiveCaption); }
        }
        public static Brush GrayText
        {
            get { return FromSystemColor(SystemColors.GrayText); }
        }
        public static Brush Highlight
        {
            get { return FromSystemColor(SystemColors.Highlight); }
        }
        public static Brush HighlightText
        {
            get { return FromSystemColor(SystemColors.HighlightText); }
        }
        public static Brush HotTrack
        {
            get { return FromSystemColor(SystemColors.HotTrack); }
        }
        public static Brush InactiveCaption
        {
            get { return FromSystemColor(SystemColors.InactiveCaption); }
        }
        public static Brush InactiveBorder
        {
            get { return FromSystemColor(SystemColors.InactiveBorder); }
        }
        public static Brush InactiveCaptionText
        {
            get { return FromSystemColor(SystemColors.InactiveCaptionText); }
        }
        public static Brush Info
        {
            get { return FromSystemColor(SystemColors.Info); }
        }
        public static Brush InfoText
        {
            get { return FromSystemColor(SystemColors.InfoText); }
        }
        public static Brush Menu
        {
            get { return FromSystemColor(SystemColors.Menu); }
        }
        public static Brush MenuBar
        {
            get { return FromSystemColor(SystemColors.MenuBar); }
        }
        public static Brush MenuHighlight
        {
            get { return FromSystemColor(SystemColors.MenuHighlight); }
        }
        public static Brush MenuText
        {
            get { return FromSystemColor(SystemColors.MenuText); }
        }
        public static Brush ScrollBar
        {
            get { return FromSystemColor(SystemColors.ScrollBar); }
        }
        public static Brush Window
        {
            get { return FromSystemColor(SystemColors.Window); }
        }
        public static Brush WindowFrame
        {
            get { return FromSystemColor(SystemColors.WindowFrame); }
        }
        public static Brush WindowText
        {
            get { return FromSystemColor(SystemColors.WindowText); }
        }

        public static Brush FromSystemColor(Color c)
        {
            if (!c.IsSystemColor)
                throw new ArgumentException("Color is not system color.");

            if (items.ContainsKey(c))
                return items[c];

            var brush = new SolidBrush(c);
            
            items.Add(c, brush);
            return brush;
        }
    }
}