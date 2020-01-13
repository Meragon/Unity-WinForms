namespace System.Windows.Forms
{
    using System.Drawing;

    public class TextBox : Control
    {
        private bool customBackColor;
        private bool customForeColor;
        
        private Color backColor;
        private Color foreColor;
        private Color borderSingleColor = Color.FromArgb(100, 100, 100);
        
        private readonly Pen borderPen = new Pen(Color.White);
        private readonly bool styleInitialized;
        
        private char passwordChar = (char)0;
        private string passwordText;
        private string text;
        
        public TextBox()
        {
            text = string.Empty;
            
            BackColor = SystemColors.Window;
            BorderStyle = BorderStyle.Fixed3D;
            Padding = new Padding(2, 0, 2, 0);
            TextAlign = HorizontalAlignment.Left;
            
            uwfBorderColor = SystemColors.ActiveBorder;
            uwfBorderFocusedColor = Color.FromArgb(86, 157, 229);
            uwfBorderHoverColor = Color.FromArgb(126, 180, 234);

            styleInitialized = true;
        }

        public BorderStyle BorderStyle { get; set; }
        public bool Multiline { get; set; }
        public char PasswordChar
        {
            get { return passwordChar; }
            set
            {
                passwordChar = value;
                UpdatePasswordText();
            }
        }
        public bool ReadOnly { get; set; }
        public override string Text
        {
            get { return text; }
            set
            {
                if (text == value)
                    return;
                
                text = value;
                if (text == null)
                    text = string.Empty;
                
                UpdatePasswordText();
                OnTextChanged(EventArgs.Empty);
            }
        }
        public HorizontalAlignment TextAlign { get; set; }
        
        internal Color uwfBorderColor { get; set; }
        internal Color uwfBorderFocusedColor { get; set; }
        internal Color uwfBorderHoverColor { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(100, 24); }
        }

        public override string ToString()
        {
            // In future: move to TextBoxBase.
            var bs = base.ToString();
            var txt = Text;

            if (txt == null)
                txt = "";

            if (txt.Length > 40)
                txt = txt.Substring(0, 40) + "...";

            return bs + ", Text: " + txt;
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            if (styleInitialized)
                customBackColor = true;
            
            backColor = BackColor;
        }
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            backColor = !customBackColor && !Enabled ? SystemColors.Control : BackColor;
            foreColor = !customForeColor && !Enabled ? SystemColors.GrayText : ForeColor;
        }
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);

            if (styleInitialized)
                customForeColor = true;

            foreColor = ForeColor;
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (Enabled && !ReadOnly)
                Cursor.Current = Cursors.IBeam;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Cursor.Current = Cursors.Default;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            var g = e.Graphics;
            var textX = Padding.Left;
            var textY = Padding.Top;
            var textW = Width - Padding.Horizontal;
            var textH = Height - Padding.Vertical;

            if (Enabled && Focused)
            {
                string tempText;

                if (shouldFocus)
                    g.uwfFocusNext();

                if (passwordChar == 0)
                {
                    if (!Multiline)
                        tempText = g.uwfDrawTextField(Text, Font, foreColor, textX, textY, textW, textH, TextAlign);
                    else
                        tempText = g.uwfDrawTextArea(Text, Font, foreColor, textX, textY, textW, textH);
                }
                else
                    tempText = g.uwfDrawPasswordField(Text, Font, foreColor, textX, textY, textW, textH, TextAlign);

                if (shouldFocus)
                {
                    shouldFocus = false;
                    g.uwfFocus();
                }

                if (!ReadOnly)
                    Text = tempText;
            }
            else
            {
                var paintText = Text;

                if (passwordChar != 0)
                    paintText = passwordText;

                if (Multiline)
                    g.uwfDrawString(paintText, Font, foreColor, textX, textY, textW, textH, ContentAlignment.TopLeft);
                else
                    g.uwfDrawString(paintText, Font, foreColor, textX, textY, textW, textH, TextAlign);
            }

            switch (BorderStyle)
            {
                case BorderStyle.None:
                    break;
                
                case BorderStyle.FixedSingle:
                    borderPen.Color = borderSingleColor;
                    g.DrawRectangle(borderPen, 0, 0, Width, Height);
                    break;
                
                case BorderStyle.Fixed3D:
                    borderPen.Color = Color.White;
                    g.DrawRectangle(borderPen, 1, 1, Width - 2, Height - 2);
                    
                    borderPen.Color = Focused ? uwfBorderFocusedColor : uwfHovered ? uwfBorderHoverColor : uwfBorderColor;
                    g.DrawRectangle(borderPen, 0, 0, Width, Height);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.uwfFillRectangle(backColor, 0, 0, Width, Height);
        }

        private static string GetPasswordString(string from)
        {
            return string.IsNullOrEmpty(from) ? string.Empty : new string('*', from.Length);
        }
        private void UpdatePasswordText()
        {
            if (passwordChar != 0)
                passwordText = GetPasswordString(Text);
        }
    }
}
