namespace System.Windows.Forms
{
    using System.Drawing;

    public static class MessageBox
    {
        private static Form last;

        internal static Form Last
        {
            get
            {
                if (last != null && (last.IsDisposed || last.Disposing))
                    last = null;
                return last;
            }
            private set { last = value; }
        }

        public static DialogResult Show(string text)
        {
            return Show(text, string.Empty);
        }
        public static DialogResult Show(string text, string caption)
        {
            return Show(text, caption, SystemFonts.uwfArial_12);
        }
        public static DialogResult Show(string text, string caption, Font textFont)
        {
            var form = new Form();
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
            form.MinimumSize = new Size(154, 140);
            form.Size = form.MinimumSize;
            form.SizeGripStyle = SizeGripStyle.Hide;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Text = caption;
            form.TopMost = true;

            var panel = new Panel();
            panel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            panel.BackColor = Color.White;
            panel.Location = new Point(0, 24);
            panel.Size = new Size(form.Width, form.Height - 24 - 48);

            var label = new Label();
            label.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            label.Font = textFont;
            label.Location = new Point(0, 24);
            label.Padding = new Padding(12, 24, 12, 24);
            label.Text = text;
            label.SizeChanged += (sender, args) =>
            {
                var newSize = new Size(label.Width, label.Height + 24 + 48);
                if (newSize.Width > form.MinimumSize.Width)
                    form.Width = newSize.Width;
                if (newSize.Height > form.MinimumSize.Height)
                    form.Height = newSize.Height;
            };

            form.Controls.Add(panel);
            form.Controls.Add(label);

            var buttonOk = new Button();
            buttonOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            buttonOk.TabIndex = 0;
            buttonOk.Text = "Ok";
            buttonOk.Size = new Size(86, 24);
            buttonOk.Location = new Point(form.Width - 8 - buttonOk.Width, form.Height - buttonOk.Height - 12);
            buttonOk.Click += (sender, args) => form.Close();

            form.AcceptButton = buttonOk;
            form.Controls.Add(buttonOk);
            form.ShowDialog();
            form.Focus(); // Disable button focus to prevent invoking Click event from the OnKeyDown event.

            Last = form;

            return DialogResult.OK;
        }
    }
}
