namespace System.Drawing
{
    using System.Globalization;

    public interface IApiSystem
    {
        CultureInfo CurrentCulture { get; }
        Point MousePosition { get; }
    }
}
