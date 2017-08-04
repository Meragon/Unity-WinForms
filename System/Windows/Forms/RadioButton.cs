namespace System.Windows.Forms
{
    using System.Drawing;

    [Obsolete]
    public class RadioButton : ButtonBase
    {
        public bool Checked { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(104, 24); }
        }
    }
}
