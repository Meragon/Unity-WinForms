namespace System.Windows.Forms
{
    public abstract class ListControl : Control
    {
        public event EventHandler SelectedValueChanged;

        public abstract int SelectedIndex { get; set; }
        public object SelectedValue { get; set; }

        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            OnSelectedValueChanged(EventArgs.Empty);    
        }
        protected virtual void OnSelectedValueChanged(EventArgs e)
        {
            var handler = SelectedValueChanged;
            if (handler != null)
                handler(this, e);
        }
    }
}
