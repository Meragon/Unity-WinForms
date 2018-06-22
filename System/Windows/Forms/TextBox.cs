namespace System.Windows.Forms
{
    using System.Drawing;

    public class TextBox : Control
    {
        private static readonly Color defaultTextboxBackColor = Color.FromArgb(250, 250, 250);

        private readonly Pen borderPen = new Pen(Color.White);
        private char passwordChar = (char)0;
        public string passwordText;
        public string text;
        
        public TextBox()
        {
            text = string.Empty;

            BackColor = defaultTextboxBackColor;
            Padding = new Padding(2, 0, 2, 0);
            TextAlign = HorizontalAlignment.Left;
            
            uwfBorderColor = SystemColors.ActiveBorder;
            uwfBorderFocusedColor = Color.FromArgb(86, 157, 229);
            uwfBorderHoverColor = Color.FromArgb(126, 180, 234);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            borderPen.Color = Focused ? uwfBorderFocusedColor : uwfHovered ? uwfBorderHoverColor : uwfBorderColor;

            var g = e.Graphics;
            var textX = Padding.Left;
            var textY = Padding.Top;
            var textW = Width - Padding.Horizontal;
            var textH = Height - Padding.Vertical;

            g.uwfFillRectangle(BackColor, 0, 0, Width, Height);

            if (Enabled && Focused)
            {
                string tempText;

                if (shouldFocus)
                    g.uwfFocusNext();

                if (passwordChar == 0)
                {
                    if (Multiline == false)
                        tempText = g.uwfDrawTextField(Text, Font, ForeColor, textX, textY, textW, textH, TextAlign);
                    else
                        tempText = g.uwfDrawTextArea(Text, Font, ForeColor, textX, textY, textW, textH);
                }
                else
                    tempText = g.uwfDrawPasswordField(Text, Font, ForeColor, textX, textY, textW, textH, TextAlign);

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
                        g.uwfDrawString(Text, Font, ForeColor, textX, textY, textW, textH, ContentAlignment.TopLeft);
                    else
                        g.uwfDrawString(Text, Font, ForeColor, textX, textY, textW, textH, TextAlign);
                }
                else
                    g.uwfDrawString(passwordText, Font, ForeColor, textX, textY, textW, textH, TextAlign);
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
