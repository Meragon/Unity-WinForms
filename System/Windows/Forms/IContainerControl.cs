namespace System.Windows.Forms
{
    public interface IContainerControl
    {
        Control ActiveControl { get; set; }

        bool ActivateControl(Control active);
    }
}
