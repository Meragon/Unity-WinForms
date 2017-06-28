namespace System.Windows.Forms.Design
{
    public interface IControlDesigner
    {
        Control Control { get; }

        object Draw(int width, int height);
    }
}
