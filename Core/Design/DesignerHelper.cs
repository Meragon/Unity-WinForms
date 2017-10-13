namespace System.Windows.Forms.Design
{
    internal static class DesignerHelper
    {
        public static ObjectDesigner GetDesigner(this Control c)
        {
            return new ObjectDesigner(c);
        }
    }
}
