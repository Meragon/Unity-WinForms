namespace System.Windows.Forms
{
    using System.Drawing;

    public class TextBox : Control
    {
        private bool customBackColor;
        private bool customForeColor;
        
        private Color backColor;
        private Color foreColor;
        
        private readonly Pen borderPen = new Pen(Color.White);
        private char passwordChar = (char)0;
        private string passwordText;
        private string text;
        private bool styleInitialized;
        
        public TextBox()
        {
            text = string.Empty;
            
            BackColor = SystemColors.Window;
            Padding = new Padding(2, 0, 2, 0);
            TextAlign = HorizontalAlignment.Left;
            
            uwfBorderColor = SystemColors.ActiveBorder;
            uwfBorderFocusedColor = Color.FromArgb(86, 157, 229);
            uwfBorderHoverColor = Color.FromArgb(126, 180, 234);

            styleInitialized = true;
        }

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

            backColor = customBackColor == false && Enabled == false ? SystemColors.Control : BackColor;
            foreColor = customForeColor == false && Enabled == false ? SystemColors.GrayText : ForeColor;
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

            if (Enabled && ReadOnly == false)
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
            
            borderPen.Color = Focused ? uwfBorderFocusedColor : uwfHovered ? uwfBorderHoverColor : uwfBorderColor;

            var g = e.Graphics;
            var textX = Padding.Left;
            var textY = Padding.Top;
            var textW = Width - Padding.Horizontal;
            var textH = Height - Padding.Vertical;

            // NOTE: There is also should be 1px rectangle with SystemColors.Window color between background color and border.
            // And it will affect performance a little. 
            g.uwfFillRectangle(backColor, 0, 0, Width, Height);

            if (Enabled && Focused)
            {
                string tempText;

                if (shouldFocus)
                    g.uwfFocusNext();

                if (passwordChar == 0)
                {
                    if (Multiline == false)
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

                if (ReadOnly == false)
                    Text = tempText;
            }
            else
            {
                if (passwordChar == 0)
                {
                    if (Multiline)
                        g.uwfDrawString(Text, Font, foreColor, textX, textY, textW, textH, ContentAlignment.TopLeft);
                    else
                        g.uwfDrawString(Text, Font, foreColor, textX, textY, textW, textH, TextAlign);
                }
                else
                    g.uwfDrawString(passwordText, Font, foreColor, textX, textY, textW, textH, TextAlign);
            }

            g.DrawRectangle(borderPen, 0, 0, Width, Height);
        }

        private static string GetPasswordString(string from)
        {
            if (string.IsNullOrEmpty(from)) return string.Empty;
            return new string('*', from.Length);
        }
        private void UpdatePasswordText()
        {
            if (passwordChar != 0)
                passwordText = GetPasswordString(Text);
        }
    }
}
