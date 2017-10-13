namespace System.Windows.Forms.Design
{
    public interface IObjectDesigner
    {
        object Value { get; }

        object Draw(int width, int height);
    }
}
