namespace System.Windows.Forms
{
    using System.Drawing.API;

    using Unity.API;

    /// <summary>
    /// Replace with your own implementation if needed.
    /// </summary>
    public static class ApiHolder
    {
        public static IApiGraphics Graphics = new UnityGdi(UnityWinForms.DefaultSprite);
        public static IApiTiming Timing = new UnityTiming();
    }
}