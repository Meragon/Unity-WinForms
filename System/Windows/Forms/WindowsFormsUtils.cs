namespace System.Windows.Forms
{
    using System.Drawing;

    internal sealed class WindowsFormsUtils
    {
        public static bool ContainsMnemonic(string text)
        {
            if (text == null)
                return false;

            int length = text.Length;
            int firstAmpersand = text.IndexOf('&', 0);
            if (firstAmpersand < 0 || firstAmpersand > length - 2)
                return false;

            return text.IndexOf('&', firstAmpersand + 1) == -1;
        }
        public static int RotateLeft(int value, int nBits) {
            nBits = nBits % 32;
            return value << nBits | (value >> (32 - nBits));
        }
        public static bool SafeCompareStrings(string string1, string string2, bool ignoreCase)
        {
            return string1 != null && string2 != null && string1.Length == string2.Length && string.Compare(string1, string2, ignoreCase) == 0;
        }
        
        internal static Rectangle ConstrainToBounds(Rectangle constrainingBounds, Rectangle bounds)
        {
            if (constrainingBounds.Contains(bounds)) return bounds;

            bounds.Size = new Size(
                Math.Min(constrainingBounds.Width - 2, bounds.Width),
                Math.Min(constrainingBounds.Height - 2, bounds.Height));
                
            if (bounds.Right > constrainingBounds.Right)
                bounds.X = constrainingBounds.Right - bounds.Width;
            else if (bounds.Left < constrainingBounds.Left)
                bounds.X = constrainingBounds.Left;

            if (bounds.Bottom > constrainingBounds.Bottom)
                bounds.Y = constrainingBounds.Bottom - 1 - bounds.Height;
            else if (bounds.Top < constrainingBounds.Top)
                bounds.Y = constrainingBounds.Top;
            return bounds;
        }

        internal class ReadOnlyControlCollection : Control.ControlCollection
        {
            private readonly bool _isReadOnly;
            
            public ReadOnlyControlCollection(Control owner, bool isReadOnly) : base(owner)
            {
                _isReadOnly = isReadOnly;
            }

            public override bool IsReadOnly
            {
                get { return _isReadOnly; }
            }

            public override void Add(Control value)
            {
                if (IsReadOnly)
                    throw new NotSupportedException("readonly collection");

                AddInternal(value);
            }
            public override void Clear()
            {
                if (IsReadOnly)
                    throw new NotSupportedException("readonly collection");

                base.Clear();
            }
            public override void RemoveByKey(string key)
            {
                if (IsReadOnly)
                    throw new NotSupportedException("readonly collection");

                base.RemoveByKey(key);
            }

            internal virtual void AddInternal(Control value)
            {
                base.Add(value);
            }
            internal virtual void RemoveInternal(Control value)
            {
                Remove(value);
            }
        }
    }
}
