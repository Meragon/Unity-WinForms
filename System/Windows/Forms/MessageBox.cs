using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    public class MessageBox
    {
        private static Form _last;
        public static Form Last
        {
            get
            {
                if (_last != null && (_last.IsDisposed || _last.Disposing))
                    _last = null;
                return _last;
            }
            private set { _last = value; }
        }

        public static DialogResult Show(string text)
        {
            return Show(text, "");
        }
        public static DialogResult Show(string text, string caption)
        {
            return Show(text, caption, new Font("Arial", 12));
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
            formGroup.BorderColor = form.BorderColor;
            formGroup.Size = new Size(form.Width - 16, form.Height - (int)form.HeaderHeight - 8 - 28);
            formGroup.Location = new Point(8, (int)form.HeaderHeight);

            form.Controls.Add(formGroup);

            Label formLabel = new Label();
            formLabel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
            formLabel.Font = textFont;
            formLabel.Size = formGroup.Size;
            formLabel.Text = text;
            formLabel.TextAlign = ContentAlignment.TopLeft;

            formGroup.Controls.Add(formLabel);

            Button formButton_Ok = new Button();
            formButton_Ok.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            formButton_Ok.Location = new Point(form.Width - 8 - formButton_Ok.Width, form.Height - formButton_Ok.Height - 8);
            formButton_Ok.Text = "Ok";
            formButton_Ok.Click += (object sender, EventArgs args) =>
            {
                form.Close();
            };

            form.Controls.Add(formButton_Ok);
            form.Show();

            Last = form;

            return DialogResult.OK;
        }
    }
}
