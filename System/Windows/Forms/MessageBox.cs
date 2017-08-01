namespace System.Windows.Forms
{
    using System.Drawing;

    public class MessageBox
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
            Form form = new Form();
            form.Size = new Size(220, 96 + 32);
            form.Location = new Point(
                Screen.PrimaryScreen.WorkingArea.Width / 2 - form.Width / 2,
                Screen.PrimaryScreen.WorkingArea.Height / 2 - form.Height / 2);
            form.Text = caption;
            form.TopMost = true;

            GroupBox formGroup = new GroupBox();
            formGroup.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
            formGroup.BackColor = Color.White;
            formGroup.BorderColor = form.uwfBorderColor;
            formGroup.Size = new Size(form.Width - 16, form.Height - (int)form.uwfHeaderHeight - 8 - 28);
            formGroup.Location = new Point(8, (int)form.uwfHeaderHeight);

            form.Controls.Add(formGroup);

            TextBox formLabel = new TextBox();
            formLabel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
            formLabel.Font = textFont;
            formLabel.Size = formGroup.Size;
            formLabel.Text = text;
            formLabel.ReadOnly = true;
            formLabel.Multiline = true;

            formGroup.Controls.Add(formLabel);

            Button formButton_Ok = new Button();
            formButton_Ok.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            formButton_Ok.Location = new Point(form.Width - 8 - formButton_Ok.Width, form.Height - formButton_Ok.Height - 8);
            formButton_Ok.TabIndex = 0;
            formButton_Ok.Text = "Ok";
            formButton_Ok.Click += (object sender, EventArgs args) =>
            {
                form.Close();
            };

            form.Controls.Add(formButton_Ok);
            form.ShowDialog();

            Last = form;

            return DialogResult.OK;
        }
    }
}
