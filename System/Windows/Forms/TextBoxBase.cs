namespace System.Windows.Forms
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;

    [Obsolete("WIP")]
    public class TextBoxBase : Control
    {
        internal readonly TextCursor cursor;
        internal int itemHeight = 16;
        internal int itemTextBottom = 4;
        internal int itemTextTop = 2;

        private readonly SolidBrush backBrush = new SolidBrush(Color.Transparent);
        private readonly SolidBrush textBrush = new SolidBrush(Color.Black);

        private int selectionStart;
        private int selectionLength;
        private List<string> text = new List<string>();

        public TextBoxBase()
        {
            cursor = new TextCursor(this);
            cursor.ResetCursorPaintMode();

            BackColor = Color.FromArgb(245, 245, 245);

            Text = "test text \r\nnew line was there";
        }

        public override Color BackColor
        {
            get { return backBrush.Color; }
            set { backBrush.Color = value; }
        }
        public override Color ForeColor
        {
            get { return textBrush.Color; }
            set { textBrush.Color = value; }
        }
        public override string Text
        {
            get
            {
                var builder = new StringBuilder();
                for (int i = 0; i < text.Count; i++)
                    builder.Append(text[i] + Environment.NewLine);
                return builder.ToString();
            }
            set
            {
                text = new List<string>();
                if (string.IsNullOrEmpty(value))
                {
                    text.Add(string.Empty);
                    return;
                }
                var lines = value.Replace("\r", "").Split('\n');
                text.AddRange(lines);
            }
        }

        protected internal virtual void OnPaintCursor(PaintEventArgs e)
        {
            var itemTextHeight = itemHeight + itemTextTop + itemTextBottom;

            // Paint cursor.
            if (Focused && cursor.renderMode)
                e.Graphics.uwfDrawString(cursor.renderSymbol, Font, textBrush, cursor.renderX, -itemTextBottom, 12, itemTextHeight);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            cursor.ResetCursorPaintMode();

            base.OnGotFocus(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            var keyCode = e.uwfKeyArgs.KeyCode;
            switch (keyCode)
            {
                case Keys.Right: cursor.MoveRight(); return;
            }

            var lastChar = KeyHelper.GetLastInputChar();
            text[0] += lastChar;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var height = Height;
            var width = Width;
            var itemTextHeight = itemHeight + itemTextTop + itemTextBottom;

            g.FillRectangle(backBrush, 0, 0, width, height);

            // Cursor.
            cursor.UpdateCursorPaintMode();
            cursor.UpdateRenderLocation(g);

            OnPaintCursor(e);

            // Text.
            for (int i = 0; i < text.Count; i++)
                g.uwfDrawString(text[i], Font, textBrush, 0, -itemTextBottom + i * 16, Width, itemTextHeight);
        }

        internal class TextCursor
        {
            public bool renderMode;
            public string renderSymbol = "|";
            public float renderWaitTime = 1f;
            public float renderWaitTimeCurrent;
            public int renderX;
            public bool renderXRecalculate;

            internal int x;
            internal int y;

            private readonly TextBoxBase owner;

            public TextCursor(TextBoxBase owner)
            {
                this.owner = owner;
            }

            public int X
            {
                get { return x; }
                set
                {
                    if (x == value)
                        return;
                    x = value;
                    renderXRecalculate = true;
                }
            }
            public int Y
            {
                get { return y; }
                set { y = value; }
            }

            public void FixLocation()
            {
                var checkTop = y >= 0;
                if (checkTop == false)
                    Y = 0;

                var checkBotttom = y < owner.text.Count;
                if (checkBotttom == false)
                    Y = owner.text.Count - 1;

                var line = owner.text[y];
                var checkLeft = x >= 0;
                if (checkLeft == false)
                    X = 0;

                var checkRight = x < line.Length;
                if (checkRight == false)
                    X = line.Length - 1;
            }
            public void MoveRight(int amount = 1)
            {
                var nx = amount + x;
                var line = owner.text[y];
                if (nx >= line.Length) // Try move to next line.
                {
                }
            }
            public void ResetCursorPaintMode()
            {
                renderWaitTimeCurrent = 0;
                renderMode = false;
            }
            public void Set(int nx, int ny)
            {
                X = nx;
                Y = ny;
            }
            public void UpdateCursorPaintMode()
            {
                renderWaitTimeCurrent -= swfHelper.GetDeltaTime();
                if (!(renderWaitTimeCurrent <= 0)) return;

                renderMode = !renderMode;
                renderWaitTimeCurrent = renderWaitTime;
            }
            public void UpdateRenderLocation(Graphics g)
            {
                if (renderXRecalculate == false)
                    return;

                var line = owner.text[y];
                var cursorText = line.Substring(x);
                var textSize = g.MeasureString(cursorText, owner.Font);
                renderX = (int)textSize.Width;
                renderXRecalculate = false;
            }
        }
    }
}
