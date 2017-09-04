namespace System.Windows.Forms
{
    public class LinkLabelLinkClickedEventArgs : EventArgs
    {
        public LinkLabelLinkClickedEventArgs(LinkLabel.Link link)
        {
            Link = link;
            Button = MouseButtons.Left;
        }
        public LinkLabelLinkClickedEventArgs(LinkLabel.Link link, MouseButtons button) : this(link)
        {
            Button = button;
        }

        public MouseButtons Button { get; private set; }
        public LinkLabel.Link Link { get; private set; }
    }
}
