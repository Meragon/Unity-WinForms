namespace System.Windows.Forms
{
    using System.Collections.Generic;
    using System.Drawing;

    public class ToolStripMenuItem : ToolStripDropDownItem
    {
        private static Dictionary<int, string> cachedKeysValues;
        private ContentAlignment shortcutKeysFormat = ContentAlignment.MiddleLeft;

        private Keys shortcutKeys;
        private string shortcutKeysString;
        private int shortcutKeysMaxPaintWidth = 64;

        public ToolStripMenuItem()
        {
        }
        public ToolStripMenuItem(string text)
        {
            Text = text;
            TextAlign = ContentAlignment.MiddleCenter;
        }

        public bool Checked { get; set; }
        public Keys ShortcutKeys
        {
            get { return shortcutKeys; }
            set
            {
                if (shortcutKeys == value)
                    return;

                shortcutKeys = value;
                shortcutKeysString = ToShortcutString(value);
            }
        }

        protected override Size DefaultSize
        {
            get { return new Size(32, 19); }
        }
        protected override Padding DefaultPadding
        {
            get
            {
                if (IsOnDropDown)
                    return new Padding(0, 1, 0, 1);
                return new Padding(4, 0, 4, 0);
            }
        }

        protected internal override int GetEstimatedWidth(Graphics graphics)
        {
            var baseWidth = base.GetEstimatedWidth(graphics);
            if (Owner != null && Owner.IsDropDown)
                baseWidth += OwnerShortcutKeysWidth(graphics) + shortcutKeysMaxPaintWidth; // not efficient.
            return baseWidth;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var rect = Bounds;

            if (Checked)
            {
                var checkedRect = new Rectangle(rect.X + 1, rect.Y + rect.Height / 2 - 10, 20, 20);
                var graphics = e.Graphics;
                
                graphics.uwfFillRectangle(hoverColor, checkedRect);
                graphics.DrawRectangle(selectPen, checkedRect);

                var checkedImage = ApplicationResources.Images.Checked;
                if (checkedImage != null)
                {
                    var checkedImageRect = new Rectangle( // Draw without streching (override this if u need).
                        checkedRect.X + checkedRect.Width / 2 - checkedImage.Width / 2,
                        checkedRect.Y + checkedRect.Height / 2 - checkedImage.Height / 2,
                        checkedImage.Width,
                        checkedImage.Height);
                    graphics.DrawImage(checkedImage, checkedImageRect);
                }
            }

            if (!string.IsNullOrEmpty(shortcutKeysString))
            {
                if (Owner.Orientation == Orientation.Vertical)
                    e.Graphics.uwfDrawString(
                        shortcutKeysString,
                        Font,
                        Enabled ? ForeColor : SystemColors.InactiveCaption,
                        rect.X + rect.Width - shortcutKeysMaxPaintWidth,
                        rect.Y,
                        shortcutKeysMaxPaintWidth,
                        rect.Height,
                        shortcutKeysFormat);
            }
        }

        private static string GetCachedString(Keys key)
        {
            if (cachedKeysValues == null)
            {
                // Fill cache.
                cachedKeysValues = new Dictionary<int, string>();
                string[] names;
                object[] values;

                GetEnumData(typeof(Keys), out names, out values);

                for (int i = 0; i < names.Length; i++)
                {
                    var val = (int)values[i];
                    if (cachedKeysValues.ContainsKey(val) == false)
                        cachedKeysValues.Add(val, names[i]);
                }
            }

            return cachedKeysValues[(int)key];
        }
        private static void GetEnumData(Type type, out string[] names, out object[] values)
        {
            // More efficient than Enum.GetNames(...).
            var fields = type.GetFields(
                Reflection.BindingFlags.Public |
                Reflection.BindingFlags.NonPublic |
                Reflection.BindingFlags.Static);

            var fieldsLength = fields.Length;

            names = new string[fieldsLength];
            values = new object[fieldsLength];

            for (int i = 0; i < fieldsLength; i++)
            {
                var field = fields[i];

                names[i] = field.Name;
                values[i] = field.GetRawConstantValue();
            }
        }
        private static Keys GetKeyCode(Keys keyData)
        {
            var keys = keyData & Keys.KeyCode;

            if (KeyEventArgs.IsKeyDefined(keys))
                return keys;

            return Keys.None;
        }
        private static string ToShortcutString(Keys key)
        {
            // TODO: move to KeysConverter.ConvertTo(object).
            var keyCode = GetKeyCode(key);
            var keyMods = key & Keys.Modifiers;
            var skeyMods = "";
            if (keyMods != Keys.None)
                skeyMods = GetCachedString(keyMods).Replace("Control", "Ctrl") + " + ";

            var str = skeyMods + GetCachedString(keyCode).Replace("Delete", "Del");
            return str;
        }

        private int OwnerShortcutKeysWidth(Graphics graphics)
        {
            var owner = Owner;
            if (owner == null)
                return 0;

            int keysWidth = 0;
            for (int i = 0; i < owner.Items.Count; i++)
            {
                var item = owner.Items[i];
                var menuItem = item as ToolStripMenuItem;
                if (menuItem == null)
                    continue;

                if (menuItem.shortcutKeys != Keys.None)
                    keysWidth = Math.Max((int)graphics.MeasureString(shortcutKeysString, Font).Width, keysWidth);
            }

            return keysWidth;
        }
    }
}