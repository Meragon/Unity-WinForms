namespace System.Windows.Forms
{
    [Serializable]
    public class ContainerControl : ScrollableControl, IContainerControl
    {
        public Control ActiveControl { get; set; }

        public bool ActivateControl(Control active)
        {
            throw new NotImplementedException();
        }
    }
}
