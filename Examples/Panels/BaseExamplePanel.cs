namespace UnityWinForms.Examples.Panels
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public abstract class BaseExamplePanel : Panel
    {
        protected int lineOffset = 36;

        protected BaseExamplePanel()
        {
            AutoScroll = true;
            BackColor = Color.FromArgb(239, 235, 233);
        }
        
        public abstract void Initialize();
    }
}
