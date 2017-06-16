using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    [Serializable]
    public class Button : ButtonBase, IButtonControl
    {
        private DialogResult dialogResult;

        public virtual DialogResult DialogResult
        {
            get { return dialogResult; }
            set { dialogResult = value; }
        }

        public Button()
        {
            this.SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, false);
        }

        public void NotifyDefault(bool value)
        {
            // ?
        }
        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }

        protected override void OnClick(EventArgs e)
        {
            Form form = FindFormInternal();
            if (form != null)
            {
                form.DialogResult = dialogResult;
                if (form.AcceptButton == this && form.dialog)
                    form.Close();
            }

            base.OnClick(e);
        }

        public override string ToString()
        {
            return base.ToString() + ", Text: " + this.Text;
        }
    }
}
