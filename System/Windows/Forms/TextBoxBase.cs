namespace System.Windows.Forms
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;

    [Obsolete("WIP")]
    public class TextBoxBase : Control
    {
        internal readonly TextCursor cursor;

        /*
         * --------------------line start |
         * -----top offset                |
         *   text                         | item height
         * -----bottom offset             |
         * --------------------line end   |
         */
        internal int itemHeight = 16;
        internal int itemTextBottomOffset = 4;
        internal int itemTextTopOffset = 2;
        internal char[] nextWordStopAtChars = { ' ' }; // Characters that will prevent moving cursor with Ctrl + Arrows.

        private readonly Pen cursorPen = new Pen(Color.Black);
        private readonly SolidBrush backBrush = new SolidBrush(Color.Transparent);
        private readonly SolidBrush textBrush = new SolidBrush(Color.Black);

        private ScrollBar vscroll;
        private Color selectionColor = Color.FromArgb(173, 214, 255);
        private int selectionStart;
        private int selectionLength;
        private int selectionLine;
        private int selectionRx1, selectionRx2;
        private List<string> lines = new List<string>();

        public TextBoxBase()
        {
            BackColor = Color.FromArgb(245, 245, 245);

            cursor = new TextCursor(this);
            cursor.ResetCursorPaintMode();

            vscroll = new VScrollBar();
            vscroll.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            vscroll.Location = new Point(Width - vscroll.Width, 0);
            vscroll.Height = Height;

            Controls.Add(vscroll);
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
                for (int i = 0; i < lines.Count; i++)
                    builder.Append(lines[i] + Environment.NewLine);
                return builder.ToString();
            }
            set
            {
                lines = new List<string>();
                if (string.IsNullOrEmpty(value))
                {
                    this.lines.Add(string.Empty);
                    return;
                }
                var vlines = value.Replace("\r", "").Split('\n');
                lines.AddRange(vlines);

                PerformLayout();
            }
        }

        protected internal virtual void OnPaintCursor(PaintEventArgs e)
        {
            var itemTextHeight = itemHeight + itemTextTopOffset + itemTextBottomOffset;

            // Paint cursor.
            if (Focused && cursor.renderMode)
            {
                cursorPen.Color = ForeColor;
                var cursorRenderY = GetLineRenderY(cursor.y);
                e.Graphics.DrawLine(cursorPen, cursor.renderX, cursorRenderY, cursor.renderX, cursorRenderY + itemHeight - itemTextTopOffset);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            cursor.ResetCursorPaintMode();

            base.OnGotFocus(e);
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            UpdateScrolls();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            var keyCode = e.KeyCode;
            var ctrlMod = e.Control;
            var shiftMod = e.Shift;
            switch (keyCode)
            {
                // Basic navigation.
                case Keys.Down: CursorMoveDown(ctrlMod, shiftMod); return;
                case Keys.Left: CursorMoveLeft(ctrlMod, shiftMod); return;
                case Keys.Right: CursorMoveRight(ctrlMod, shiftMod); return;
                case Keys.Up: CursorMoveUp(ctrlMod, shiftMod); return;

                case Keys.End: cursor.MoveToLineEnd(); return;
                case Keys.Home: cursor.MoveToLineStart(); return;

                case Keys.Back: CursorRemoveTextToLeft(); return;
                case Keys.Delete: CursorRemoveTextToRight(); return;
                case Keys.Enter:
                    lines.Insert(cursor.y + 1, "");
                    cursor.MoveDown();
                    return;
            }

            var lastChar = KeyHelper.GetLastInputChar();

            CursorInsertText(lastChar.ToString());
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            CursorSetAt(e.X, e.Y);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Right)
            {
                var itemCut = new ToolStripMenuItem("Cut");
                itemCut.Enabled = false;

                var itemCopy = new ToolStripMenuItem("Copy");
                itemCopy.Enabled = false;

                var itemPaste = new ToolStripMenuItem("Paste");
                itemPaste.Enabled = false;

                var context = new ContextMenuStrip();

                context.Items.Add(itemCut);
                context.Items.Add(itemCopy);
                context.Items.Add(itemPaste);

                context.Show(this, e.Location);
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            vscroll.Value -= e.Delta * 2;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var height = Height;
            var width = Width;
            var itemTextHeight = itemHeight + itemTextTopOffset + itemTextBottomOffset;

            g.FillRectangle(backBrush, 0, 0, width, height);

            // Selection.
            if (selectionLength != 0)
            {
                selectionRx1 = (int)g.MeasureString(SubstringSafe(lines[selectionLine], 0, selectionStart), Font).Width;
                selectionRx2 = (int)g.MeasureString(SubstringSafe(lines[selectionLine], 0, selectionStart + selectionLength), Font).Width;
                g.uwfFillRectangle(selectionColor, selectionRx1, GetLineRenderY(selectionLine), selectionRx2 - selectionRx1, itemTextHeight - itemTextBottomOffset);
            }

            // Cursor.
            cursor.UpdateCursorPaintMode();
            cursor.UpdateRenderLocation(g);

            OnPaintCursor(e);

            // Text.
            var scrollIndex = vscroll.Value;
            var startLineIndex = scrollIndex / itemHeight - 1;
            var endLineIndex = (scrollIndex + vscroll.LargeChange) / itemHeight + 1;

            if (startLineIndex < 0) startLineIndex = 0;

            for (int i = startLineIndex; i < lines.Count && i <= endLineIndex; i++)
                g.uwfDrawString(lines[i], Font, textBrush, 0, GetLineRenderY(i), Width, itemTextHeight);
        }

        private static string SubstringSafe(string s, int start, int length)
        {
            if (start + length > s.Length)
                length = s.Length - start;

            return s.Substring(start, length);
        }

        private int GetLineRenderY(int line)
        {
            return line * itemHeight - itemTextBottomOffset - vscroll.Value;// * vscroll.SmallChange;
        }
        private void CursorEnsureVisible()
        {
            var cursorRx = cursor.y * itemHeight;
            if (cursorRx < vscroll.Value) // To top.
            {
                vscroll.Value = cursorRx;
                return;
            }

            if (cursorRx + vscroll.SmallChange > vscroll.Value + vscroll.LargeChange) // To bottom.
                vscroll.Value = cursorRx - vscroll.LargeChange + vscroll.SmallChange;
        }
        private void CursorInsertText(string value, bool moveCursor = true)
        {
            lines[cursor.y] = lines[cursor.Y].Insert(cursor.x, value);

            if (moveCursor)
            {
                cursor.x += value.Length;
                cursor.renderXRecalculate = true;
                cursor.ResetCursorPaintMode();
            }
        }
        private void CursorMoveDown(bool ctrlMod, bool shiftMod)
        {
            if (ctrlMod)
            {
                vscroll.Value += vscroll.SmallChange;
                return;
            }

            if (shiftMod)
            {
            }
            else
                ResetSelection();

            cursor.MoveDown();
            CursorEnsureVisible();
        }
        private void CursorMoveLeft(bool ctrlMod, bool shiftMod)
        {
            var amount = 1;
            if (ctrlMod)
            {
                var str = lines[cursor.y].Substring(0, cursor.x).TrimEnd(' ');
                for (int i = str.Length - 1; i >= 0; i--, amount++)
                    if (nextWordStopAtChars.Contains(str[i]))
                        break;
            }

            if (shiftMod)
            {
                if (selectionLength == 0)
                {
                    selectionStart = cursor.x;
                    selectionLine = cursor.y;
                }

                selectionLength -= amount;
            }
            else
                ResetSelection();

            cursor.MoveLeft(amount);
            CursorEnsureVisible();
        }
        private void CursorMoveRight(bool ctrlMod, bool shiftMod)
        {
            var amount = 1;
            if (ctrlMod)
            {
                var str = lines[cursor.y].Substring(cursor.x).TrimStart(' ');
                for (int i = 0; i < str.Length; i++, amount++)
                    if (nextWordStopAtChars.Contains(str[i]))
                        break;
            }

            if (shiftMod) // Add chars to select.
            {
                if (selectionLength == 0)
                {
                    selectionStart = cursor.x;
                    selectionLine = cursor.y;
                }

                selectionLength += amount;
            }
            else
                ResetSelection();

            cursor.MoveRight(amount);
            CursorEnsureVisible();
        }
        private void CursorMoveUp(bool ctrlMod, bool shiftMod)
        {
            if (ctrlMod)
            {
                vscroll.Value -= vscroll.SmallChange;
                return;
            }

            if (shiftMod)
            {
            }
            else
                ResetSelection();

            cursor.MoveUp();
            CursorEnsureVisible();
        }
        private void CursorRemoveTextToLeft(int amount = 1) // default 'Backspace' action.
        {
            if (cursor.x < amount)
                amount = cursor.x;

            if (amount == 0)
            {
                // Remove line.
                if (cursor.y > 0)
                {
                    var str = lines[cursor.y];

                    lines.RemoveAt(cursor.Y);
                    cursor.MoveLeft();
                    CursorInsertText(str, false);
                }
                return;
            }

            lines[cursor.Y] = lines[cursor.y].Remove(cursor.x - amount, amount);

            cursor.x -= amount;
            cursor.renderXRecalculate = true;
            cursor.ResetCursorPaintMode();
        }
        private void CursorRemoveTextToRight(int amount = 1) // default 'Delete' action.
        {
            var line = lines[cursor.y];
            if (cursor.x >= line.Length)
            {
                // Copy next line and concat to current one.
                if (cursor.y + 1 < lines.Count)
                {
                    lines[cursor.y] = line + lines[cursor.y + 1];
                    lines.RemoveAt(cursor.y + 1);
                }

                return;
            }

            lines[cursor.y] = line.Remove(cursor.x, amount);
        }
        private void CursorSetAt(int renderX, int renderY)
        {
            ResetSelection();

            cursor.y = (vscroll.Value + renderY) / itemHeight;

            cursor.renderXRecalculate = true;
            cursor.FixLocation();
            cursor.ResetCursorPaintMode();
            cursor.ResolveRenderLocation(renderX);
        }
        private void ResetSelection()
        {
            selectionLength = 0;
        }
        private void UpdateScrolls()
        {
            vscroll.Maximum = itemHeight * lines.Count;
            vscroll.LargeChange = Height;
            vscroll.SmallChange = itemHeight;
        }

        internal class TextCursor
        {
            public bool renderMode;
            public float renderWaitTime = 1f;
            public float renderWaitTimeCurrent;
            public int renderX;
            public bool renderXRecalculate = true;

            internal int x;
            internal int y;

            private readonly TextBoxBase owner;

            private bool resolveX;
            private int resolveXAt;

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
                var checkBotttom = y < owner.lines.Count;
                if (checkBotttom == false)
                    Y = owner.lines.Count - 1;

                var checkTop = y >= 0;
                if (checkTop == false)
                    Y = 0;

                var line = owner.lines[y];

                var checkRight = x < line.Length + 1;
                if (checkRight == false)
                    X = line.Length;

                var checkLeft = x >= 0;
                if (checkLeft == false)
                    X = 0;
            }
            public void MoveDown(int amount = 1)
            {
                y += amount;

                FixLocation();
                ResetCursorPaintMode();
            }
            public void MoveLeft(int amount = 1)
            {
                x -= amount;
                if (x < 0 && y > 0)
                {
                    // Move up.
                    y--;
                    MoveToLineEnd();
                }

                renderXRecalculate = true;
                FixLocation();
                ResetCursorPaintMode();
            }
            public void MoveRight(int amount = 1)
            {
                var nx = amount + x;
                var line = owner.lines[y];
                if (nx >= line.Length + 1) // Try move to next line.
                {
                    if (y + 1 < owner.lines.Count)
                    {
                        MoveToLineStart();
                        MoveDown();
                    }
                }
                else
                {
                    x = nx;
                    renderXRecalculate = true;
                    FixLocation();
                    ResetCursorPaintMode();
                }
            }
            public void MoveUp(int amount = 1)
            {
                y -= amount;

                FixLocation();
                ResetCursorPaintMode();
            }
            public void MoveToLineEnd()
            {
                var estIndex = owner.lines[y].Length;
                if (x == estIndex)
                    return;

                x = estIndex;
                renderXRecalculate = true;
                ResetCursorPaintMode();
            }
            public void MoveToLineStart()
            {
                if (x == 0)
                    return;

                x = 0;
                renderXRecalculate = true;
                ResetCursorPaintMode();
            }
            public void ResetCursorPaintMode()
            {
                renderWaitTimeCurrent = 0;
                renderMode = false;
            }
            public void ResolveRenderLocation(int rx)
            {
                resolveX = true;
                resolveXAt = rx;
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
                if (resolveX)
                {
                    var line = owner.lines[y];
                    var charAt = line.Length;
                    for (int i = 0; i < line.Length; i++)
                    {
                        var str = line.Substring(0, i);
                        var textSize = g.MeasureString(str, owner.Font);
                        if (resolveXAt > textSize.Width - 1)
                            continue;

                        charAt = i - 1;
                        break;
                    }

                    if (charAt < 0)
                        charAt = 0;

                    x = charAt;
                    renderXRecalculate = true;
                    resolveX = false;
                    FixLocation();
                }

                if (renderXRecalculate == false)
                    return;

                if (x == 0)
                {
                    renderX = 0;
                    renderXRecalculate = false;
                }
                else
                {
                    var line = owner.lines[y];
                    var cursorText = line.Substring(0, x);
                    var textSize = g.MeasureString(cursorText, owner.Font);
                    renderX = (int)textSize.Width;
                    renderXRecalculate = false;
                }
            }
        }
    }
}
