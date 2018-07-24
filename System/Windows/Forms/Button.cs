namespace System.Windows.Forms
{
    [Serializable]
    public class Button : ButtonBase, IButtonControl
    {
        private DialogResult dialogResult;

        public Button()
        {
            SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, false);
        }

        public virtual DialogResult DialogResult
        {
            get { return dialogResult; }
            set { dialogResult = value; }
        }

        public void NotifyDefault(bool value)
        {
            // ?
        }
        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }
        public override string ToString()
        {
            return base.ToString() + ", Text: " + Text;
        }

        protected override void OnClick(EventArgs e)
        {
            var form = FindFormInternal();
            if (form != null)
                form.DialogResult = dialogResult;

            base.OnClick(e);
        }
    }
}
