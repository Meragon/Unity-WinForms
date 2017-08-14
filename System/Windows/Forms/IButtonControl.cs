namespace System.Windows.Forms
{
    public interface IButtonControl
    {
        DialogResult DialogResult { get; set; }

        void NotifyDefault(bool value);
        void PerformClick();
    }
}
