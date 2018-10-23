namespace System.Windows.Forms
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Simple implementation of DataGridView.
    /// </summary>
    public class TableView : Control
    {
        public int ColumnsDefaultWidth = 100;

        internal TableColumn lastSortedColumn;

        protected int topLeftButtonWidth = 40;

        private readonly Pen borderPen = new Pen(Color.Black);
        private bool columnHeadersHidden;
        private HScrollBar hScroll;
        private bool hScrollHidden;
        private bool rowHeadersHidden;
        private TableColumnButton topLeftButton;
        private VScrollBar vScroll;
        private bool vScrollHidden;

        public TableView()
        {
            BackColor = Color.FromArgb(171, 171, 171);
            CellPadding = 1;
            ColumnsStyle = new TableButtonStyle();
            Padding = new Padding(2);
            SkipControlsInitializations = false;

            Columns = new TableColumnCollection(this);
            Rows = new TableRowCollection(this);
            
            MouseHook.MouseUp += MouseHookOnMouseUp;
        }
        
        public delegate void RowClickHandler(object sender, TableRow row, MouseEventArgs mArgs);

        public event RowClickHandler RowClick;

        public Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; }
        }
        public int CellPadding { get; set; }
        public int ColumnCount { get { return Columns.Count; } }
        public TableColumnCollection Columns { get; private set; }
        public TableButtonStyle ColumnsStyle { get; set; }
        public TableRowCollection Rows { get; private set; }
        public bool SkipControlsInitializations { get; set; }

        protected override Size DefaultSize
        {
            get { return new Size(240, 150); }
        }

        private float maxScrollHeight
        {
            get
            {
                if (Rows.Count == 0) return 1;

                var h = Rows.Last().control.Location.Y + Rows.Last().control.Height;
                if (hScroll != null)
                    h += hScroll.Height;
                return h;
            }
        }
        private float maxScrollWidth
        {
            get
            {
                if (Columns.Count == 0) return 1;

                var w = Columns.Last().control.Location.X + Columns.Last().control.Width;
                if (vScroll != null)
                    w += vScroll.Width;
                return w;
            }
        }

        public void HideColumnHeaders()
        {
            columnHeadersHidden = true;

            UpdateColumns();
        }
        public void HideRowHeaders()
        {
            rowHeadersHidden = true;

            UpdateRows();
        }
        public void HideScrolls()
        {
            hScrollHidden = true;
            vScrollHidden = true;

            if (hScroll != null) hScroll.Visible = !hScrollHidden;
            if (vScroll != null) vScroll.Visible = !vScrollHidden;
        }
        public override void Refresh()
        {
            base.Refresh();

            AlignColumns();
            AlignRows();
        }
        public void ShowColumnHeaders()
        {
            columnHeadersHidden = false;

            UpdateColumns();
        }
        public void ShowRowHeaders()
        {
            rowHeadersHidden = false;

            UpdateRows();
        }
        public void ShowScrolls()
        {
            hScrollHidden = false;
            vScrollHidden = false;

            if (hScroll != null) hScroll.Visible = !hScrollHidden;
            if (vScroll != null) vScroll.Visible = !vScrollHidden;
        }
        public virtual void Sort(TableColumn column, ListSortDirection direction)
        {
            if (column == null) return;

            int columnIndex = Columns.FindIndex(column);
            Dictionary<TableRow, object[]> items = new Dictionary<TableRow, object[]>();
            for (int i = 0; i < Rows.Count; i++)
            {
                var r = Rows[i];
                items.Add(r, r.Items);
            }

            var itemsList = items.ToList();
            if (direction == ListSortDirection.Ascending)
                itemsList.Sort((x, y) =>
                {
                    var v1 = x.Value[columnIndex];
                    var v2 = y.Value[columnIndex];

                    if (v1 == null && v2 == null)
                        return 0;

                    if (v1 == null)
                        return -1;
                    if (v2 == null)
                        return 1;

                    var we = v1.ToString().CompareTo(v2.ToString());
                    return we;
                });
            else
                itemsList.Sort((x, y) =>
                {
                    var v1 = x.Value[columnIndex];
                    var v2 = y.Value[columnIndex];

                    if (v1 == null && v2 == null)
                        return 0;

                    if (v1 == null)
                        return 1;
                    if (v2 == null)
                        return -1;

                    var we = v1.ToString().CompareTo(v2.ToString());
                    return -we;
                });

            Rows.ClearList();

            for (int i = 0; i < itemsList.Count; i++)
                Rows.Add(itemsList[i].Key);

            AlignRows();

            if (lastSortedColumn != null)
                lastSortedColumn.control.Padding = new Padding(8, 0, 8, 0);
            lastSortedColumn = column;
            lastSortedColumn.control.Padding = new Padding(24, 0, 8, 0);
            
            if (SkipControlsInitializations)
                TryInitializeDefferedControls();
        }
        public void TryInitializeDefferedControls()
        {
            var controlAdded = false;
            var offsetX = 0;
            var offsetY = 0;

            if (hScroll != null) offsetX = -(int) (maxScrollWidth * hScroll.Value / hScroll.Maximum);
            if (vScroll != null) offsetY = -(int) (maxScrollHeight * vScroll.Value / vScroll.Maximum);

            // Prepare columns X and Width.
            var columnsCount = ColumnCount;
            var columnsX = new int[columnsCount];
            var columnsWidth = new int[columnsCount];
            
            for (int columnIndex = 0; columnIndex < columnsCount; columnIndex++)
            {
                var column = Columns[columnIndex];

                columnsX[columnIndex] = column.control.Location.X;
                columnsWidth[columnIndex] = column.Width;
            }

            // Prepare scrolls values.
            var hScrollMin = 0;
            var hScrollMax = Width;
            var vScrollMin = 0;
            var vScrollMax = Height;
            
            if (hScroll != null && hScroll.Visible)
            {
                hScrollMin = hScroll.Value;
                hScrollMax = hScroll.Height + hScroll.Value + hScroll.LargeChange;
            }

            if (vScroll != null && vScroll.Visible)
            {
                vScrollMin = vScroll.Value;
                vScrollMax = vScroll.Width + vScroll.Value + vScroll.LargeChange;
            }
            
            // Update controls.
            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
            {
                var row = Rows[rowIndex];
                var rowY = row.control.Location.Y;
                var rowHeight = row.control.Height;
                
                for (int columnIndex = 0; columnIndex < columnsCount; columnIndex++)
                {
                    var cellRect = new Rectangle(columnsX[columnIndex], rowY, columnsWidth[columnIndex], rowHeight);
                    var cellRectX = cellRect.X;
                    var cellRectY = cellRect.Y;
                    var cellIsVisible = 
                            cellRectX + cellRect.Width > hScrollMin && cellRectX < hScrollMax &&
                            cellRectY + cellRect.Height > vScrollMin && cellRectY < vScrollMax;

                    var rowControls = row.ItemsControls;
                    if (rowControls.CreateControlOnCellVisible != null)
                    {
                        var cellControl = rowControls[columnIndex];
                        if (cellIsVisible)
                        {
                            if (cellControl == null)
                            {
                                var newCellControl = rowControls.CreateControlOnCellVisible(columnIndex, rowIndex);
                                newCellControl.Location = cellRect.Location;
                                newCellControl.Size = cellRect.Size;
                                newCellControl.uwfOffset = new Point(offsetX, offsetY);

                                rowControls[columnIndex] = newCellControl;
                                controlAdded = true;
                            }
                        }
                        else
                        {
                            if (cellControl != null)
                            {
                                rowControls[columnIndex] = null;
                            }
                        }
                    }
                }
            }

            if (controlAdded)
            {
                if (vScroll != null)
                    vScroll.BringToFront();
                if (hScroll != null)
                    hScroll.BringToFront();
            }
        }
        
        internal void AlignColumns()
        {
            int cX = Padding.Left;
            if (topLeftButton != null)
            {
                topLeftButton.Location = new Point(Padding.Left, Padding.Top);
                cX = topLeftButton.Location.X + topLeftButton.Width + CellPadding;
            }
            for (int i = 0; i < Columns.Count; i++)
            {
                var column = Columns[i];
                column.control.Location = new Point(cX, Padding.Top);
                cX += column.Width + CellPadding;
            }

            UpdateScrolls();
        }
        internal void AlignRows()
        {
            int cY = Padding.Top;
            if (topLeftButton != null && columnHeadersHidden == false)
            {
                topLeftButton.Location = new Point(Padding.Left, Padding.Top);
                cY = topLeftButton.Location.Y + topLeftButton.Height + CellPadding;
            }
            
            for (int i = 0, cellIndex = 1; i < Rows.Count; i++)
            {
                int cX = Padding.Left;

                var row = Rows[i];
                var rowControls = row.ItemsControls;
                
                row.control.Location = new Point(cX, cY);
                row.UpdateHeaderText();
                
                if (topLeftButton != null)
                    row.control.Width = topLeftButton.Width;

                if (rowHeadersHidden == false)
                    cX += row.control.Width + CellPadding;
                
                for (int k = 0; k < rowControls.Length; k++)
                {
                    var rowIC = rowControls[k];
                    if (rowIC == null) continue;

                    rowIC.Location = new Point(cX, cY);
                    rowIC.TabIndex = cellIndex;
                    rowIC.Size = new Size(Columns[k].Width, row.Height);

                    cellIndex++;
                    cX += rowIC.Width + CellPadding;
                }

                cY += row.control.Height + CellPadding;
            }

            UpdateScrolls();
        }
        internal void RaiseOnRowClick(TableRow row, MouseEventArgs mArgs)
        {
            OnRowClick(row, mArgs);
        }
        internal void UpdateColumn(TableColumn column)
        {
            CreateTopLeftButton();

            if (column.control == null)
            {
                var cButton = new TableColumnButton(this, ColumnsStyle);
                cButton.column = column;
                cButton.EnableHorizontalResizing = true;
                cButton.Name = column.Name;
                cButton.table = this;
                cButton.Text = column.HeaderText;

                column.control = cButton;
                Controls.Add(cButton);
            }

            column.control.Visible = !columnHeadersHidden;
        }
        internal void UpdateColumns()
        {
            for (int i = 0; i < Columns.Count; i++)
                UpdateColumn(Columns[i]);
            AlignColumns();
        }
        internal void UpdateRow(TableRow row)
        {
            CreateTopLeftButton();

            if (row.control == null)
            {
                var rButton = new TableRowButton(ColumnsStyle);
                rButton.row = row;
                rButton.Size = new Size(40, row.Height);

                row.control = rButton;
                row.UpdateHeaderText();
                Controls.Add(rButton);
            }

            row.control.Visible = !rowHeadersHidden;

            if (row.Items.Length != Columns.Count)
                row.AddjustItemsCountTo(Columns.Count);


            if (row.ItemsControls == null)
                row.ItemsControls = new TableRow.TableRowControlsCollection(row, Columns.Count);
            if (row.ItemsControls.Length != row.Items.Length)
            {
                var newControls = new Control[Columns.Count];
                if (row.ItemsControls.Length > newControls.Length) // Dispose unnecessary controls.
                {
                    Array.Copy(row.ItemsControls.items, 0, newControls, 0, newControls.Length);
                    for (int i = newControls.Length; i < row.ItemsControls.Length; i++)
                    {
                        var rowC = row.ItemsControls[i];
                        if (rowC != null)
                            rowC.Dispose();
                    }
                }
                else
                    Array.Copy(row.ItemsControls.items, 0, newControls, 0, row.ItemsControls.Length);
                row.ItemsControls.items = newControls;
            }

            if (SkipControlsInitializations == false)
                for (int i = 0; i < row.Items.Length; i++)
                {
                    if (row.ItemsControls[i] != null) continue;

                    int controlColumn = i;
                    TextBox itemControl = new TextBox();
                    itemControl.uwfBorderColor = Color.Transparent;
                    itemControl.Size = new Size(Columns[i].Width, row.Height);
                    itemControl.TextChanged += (s, a) =>
                    {
                        row.Items[controlColumn] = itemControl.Text;
                    };
                    if (row.Items[i] != null)
                        itemControl.Text = row.Items[i].ToString();

                    row.ItemsControls[i] = itemControl;
                }
        }
        internal void UpdateRows()
        {
            for (int i = 0; i < Rows.Count; i++)
                UpdateRow(Rows[i]);
            AlignRows();
        }

        protected internal override void uwfOnLatePaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(borderPen, 0, 0, Width, Height);
        }
        protected internal override void uwfChildGotFocus(Control child)
        {
            base.uwfChildGotFocus(child);

            EnsureVisibleChild(child);
        }

        protected override void Dispose(bool release_all)
        {
            base.Dispose(release_all);
            
            MouseHook.MouseUp -= MouseHookOnMouseUp;
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (vScroll != null)
                vScroll.RaiseOnMouseWheel(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.uwfFillRectangle(BackColor, 0, 0, Width, Height);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            UpdateScrolls();
            
            if (SkipControlsInitializations)
                TryInitializeDefferedControls();
        }
        protected virtual void OnRowClick(TableRow row, MouseEventArgs e)
        {
            var handler = RowClick;
            if (handler != null)
                handler(this, row, e);
        }
        
        private void CreateTopLeftButton()
        {
            if (rowHeadersHidden)
            {
                if (topLeftButton != null && topLeftButton.IsDisposed == false)
                {
                    topLeftButton.Dispose();
                    topLeftButton = null;
                }
                return;
            }

            if (topLeftButton == null)
            {
                topLeftButton = new TableColumnButton(this, ColumnsStyle);
                topLeftButton.Name = "topLeftButton";
                topLeftButton.Size = new Size(topLeftButtonWidth, 20);
                Controls.Add(topLeftButton);
            }

            topLeftButton.Visible = !rowHeadersHidden && !columnHeadersHidden;
        }
        private void EnsureVisibleChild(Control child)
        {
            var childOffset = child.uwfOffset;
            bool horizontalScroll_Left = child.Location.X + childOffset.X < 0;
            bool horizontalScroll_Right = child.Location.X + childOffset.X + child.Width > Width;
            bool verticalScroll_Top = child.Location.Y + childOffset.Y < 0;
            bool verticalScroll_Bottom = child.Location.Y + childOffset.Y + child.Height > Height && child.Height < Height;

            if (hScroll != null && Width > 0 && (horizontalScroll_Left || horizontalScroll_Right))
            {
                var hRange = hScroll.Maximum - hScroll.Minimum;
                int estimatedPos;
                if (horizontalScroll_Right)
                    estimatedPos = child.Location.X + child.Width - Width;
                else
                    estimatedPos = child.Location.X;

                hScroll.Value = (int)((float)hRange * estimatedPos / maxScrollWidth);
            }

            if (vScroll != null && Height > 0 && (verticalScroll_Top || verticalScroll_Bottom))
            {
                var vRange = vScroll.Maximum - vScroll.Minimum;
                int estimatedPos;
                if (verticalScroll_Bottom)
                {
                    estimatedPos = child.Location.Y + child.Height - Height;
                    if (hScroll != null)
                        estimatedPos += hScroll.Height;
                }
                else
                    estimatedPos = child.Location.Y;

                vScroll.Value = (int)((float)vRange * estimatedPos / maxScrollHeight);
            }
        }
        private Rectangle GetCellRectangle(int rowIndex, int columnIndex)
        {
            if (rowIndex > Rows.Count || columnIndex > ColumnCount)
                return Rectangle.Empty;
            
            var column = Columns[columnIndex];
            var row = Rows[rowIndex];
            var x = column.control.Location.X;
            var y = row.control.Location.Y;
            var width = column.Width;
            var height = row.control.Height;
            
            return new Rectangle(x, y, width, height);
        }
        private bool IsRectIsVisible(Rectangle rect)
        {
            return IsRectIsVisible(rect.X, rect.Y, rect.Width, rect.Height);
        }
        private bool IsRectIsVisible(int x, int y, int width, int height)
        {
            if (hScroll != null && hScroll.Visible)
            {
                if (x + width < hScroll.Value)
                    return false;

                if (x > hScroll.Height + hScroll.Value + hScroll.LargeChange)
                    return false;
            }

            if (vScroll != null && vScroll.Visible)
            {
                if (y + height < vScroll.Value)
                    return false;

                if (y > vScroll.Width + vScroll.Value + vScroll.LargeChange)
                    return false;
            }
            
            return true;
        }
        private void HScroll_ValueChanged(object sender, EventArgs e)
        {
            int offsetX = -(int)(maxScrollWidth * hScroll.Value / hScroll.Maximum);
            var controlsCount = Controls.Count;
            for (int i = 0; i < controlsCount; i++)
            {
                var c = Controls[i];
                if (c == null || c is ScrollBar) continue;

                c.uwfOffset = new Point(offsetX, c.uwfOffset.Y);
            }
            
            if (SkipControlsInitializations)
                TryInitializeDefferedControls();
        }
        private void MouseHookOnMouseUp(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                var columnButton = Columns[i].control;
                
                // Reset resize.
                columnButton.resizing = false;
                columnButton.resizeType = TableColumnButton.resizeTypes.None;
            }
        }
        private void ResetHOffset()
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                var c = Controls[i];
                var co = c.uwfOffset;
                c.uwfOffset = new Point(0, co.Y);
            }
        }
        private void ResetVOffset()
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                var c = Controls[i];
                var co = c.uwfOffset;
                c.uwfOffset = new Point(co.X, 0);
            }
        }
        private void VScroll_ValueChanged(object sender, EventArgs e)
        {
            int offsetY = -(int)(maxScrollHeight * vScroll.Value / vScroll.Maximum);
            var controlsCount = Controls.Count;
            for (int i = 0; i < controlsCount; i++)
            {
                var c = Controls[i];
                if (c == null || c is ScrollBar) continue;

                c.uwfOffset = new Point(c.uwfOffset.X, offsetY);
            }
            
            if (SkipControlsInitializations)
                TryInitializeDefferedControls();
        }
        private void UpdateScrolls()
        {
            // Create or dispose scrolls.
            if (Rows.Count > 0)
            {
                if (Height < maxScrollHeight)
                {
                    if (vScroll == null)
                    {
                        vScroll = new VScrollBar();
                        vScroll.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
                        vScroll.Height = Height;
                        vScroll.Location = new Point(Width - vScroll.Width, 0);
                        vScroll.ValueChanged += VScroll_ValueChanged;
                        vScroll.Visible = !vScrollHidden;
                        Controls.Add(vScroll);
                    }
                }
                else if (vScroll != null)
                {
                    vScroll.ValueChanged -= VScroll_ValueChanged;
                    vScroll.Dispose();
                    vScroll = null;
                    ResetVOffset();
                }
            }
            else if (vScroll != null)
            {
                vScroll.ValueChanged -= VScroll_ValueChanged;
                vScroll.Dispose();
                vScroll = null;
                ResetVOffset();
            }

            if (Columns.Count > 0)
            {
                if (Width < maxScrollWidth)
                {
                    if (hScroll == null)
                    {
                        hScroll = new HScrollBar();
                        hScroll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                        hScroll.Width = Width;
                        hScroll.Location = new Point(0, Height - hScroll.Height);
                        hScroll.ValueChanged += HScroll_ValueChanged;
                        hScroll.Visible = !hScrollHidden;
                        Controls.Add(hScroll);
                    }
                }
                else if (hScroll != null)
                {
                    hScroll.ValueChanged -= HScroll_ValueChanged;
                    hScroll.Dispose();
                    hScroll = null;
                    ResetHOffset();
                }
            }
            else if (vScroll != null)
            {
                vScroll.ValueChanged -= VScroll_ValueChanged;
                vScroll.Dispose();
                vScroll = null;
                ResetHOffset();
            }

            // Update properties.
            if (vScroll != null)
            {
                vScroll.Maximum = (int)maxScrollHeight;
                vScroll.LargeChange = vScroll.Height;
                vScroll.BringToFront();
            }
            if (hScroll != null)
            {
                if (vScroll != null)
                    hScroll.Width = Width - vScroll.Width;
                else
                    hScroll.Width = Width;

                hScroll.Maximum = (int)maxScrollWidth;
                hScroll.LargeChange = hScroll.Width;
                hScroll.BringToFront();
            }
        }

        internal class TableColumnButton : Button, IResizableControl
        {
            internal TableColumn column;
            internal ListSortDirection lastSortDirection;
            internal TableView table;

            internal resizeTypes resizeType = resizeTypes.None;
            internal bool        resizing   = false;
            
            private Control prevButton;
            private Point resizeStartMouseLocation;
            private Point resizeStartLocation;
            private int resizeStartWidth;

            public TableColumnButton(TableView t, TableButtonStyle style)
            {
                BackColor = style.BackColor;
                uwfHoverColor = style.HoverColor;
                uwfBorderColor = style.BorderColor;
                uwfBorderHoverColor = style.BorderHoverColor;
                uwfBorderSelectColor = style.BorderSelectColor;
                Padding = new Padding(8, 0, 8, 0);
                ResizeWidth = 8;
                Size = new Size(t.ColumnsDefaultWidth, 20);
                TabStop = false;
                TextAlign = ContentAlignment.MiddleLeft;
            }

            internal enum resizeTypes
            {
                None,
                Down,
                Left,
                Right,
                Up
            }

            public bool EnableHorizontalResizing { get; set; }
            public int ResizeWidth { get; set; }

            public ControlResizeTypes GetResizeAt(Point mclient)
            {
                if (EnableHorizontalResizing)
                {
                    if (mclient.X < ResizeWidth)
                        return ControlResizeTypes.Left;
                    else if (mclient.X > Width - ResizeWidth)
                        return ControlResizeTypes.Right;
                }

                return ControlResizeTypes.None;
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        if (resizeType != resizeTypes.None)
                        {
                            resizing = true;
                            resizeStartMouseLocation = PointToScreen(e.Location);
                            resizeStartLocation = Location;
                            resizeStartWidth = Width;

                            // Find prev button.
                            var table = Parent as TableView;
                            var button = table.topLeftButton;
                            if (column.Index > 0)
                                button = table.Columns[column.Index - 1].control as TableColumnButton;
                            prevButton = button;
                        }
                        break;
                }
            }
            protected override void OnMouseHover(EventArgs e)
            {
                base.OnMouseHover(e);

                var mclient = PointToClient(MousePosition);
                if (EnableHorizontalResizing)
                {
                    if (mclient.X < ResizeWidth)
                        resizeType = resizeTypes.Left;
                    else if (mclient.X > Width - ResizeWidth)
                        resizeType = resizeTypes.Right;
                    else
                        resizeType = resizeTypes.None;
                }
            }
            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);

                if (resizing == false)
                    resizeType = resizeTypes.None;
            }
            protected override void OnMouseMove(MouseEventArgs e)
            {
                if (resizing)
                {
                    var dif = PointToScreen(e.Location).Subtract(resizeStartMouseLocation);
                    switch (resizeType)
                    {
                        case resizeTypes.Left:
                            if (prevButton != null)
                            {
                                var newX = resizeStartLocation.X + dif.X;
                                Location = new Point(newX, Location.Y);
                                prevButton.Width = Location.X - prevButton.Location.X - (Parent as TableView).CellPadding;
                                (Parent as TableView).AlignColumns();
                                (Parent as TableView).AlignRows();
                            }
                            break;
                        case resizeTypes.Right:
                            Width = resizeStartWidth + dif.X;
                            (Parent as TableView).AlignColumns();
                            (Parent as TableView).AlignRows();
                            break;
                    }
                }
            }
            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);

                if (resizeType != resizeTypes.None) return;
                if (table == null) return;

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        // Sort.
                        lastSortDirection = GetNextSortDirection();
                        table.Sort(column, lastSortDirection);
                        break;
                    case MouseButtons.Right:
                        // Create context menu.
                        ContextMenuStrip contextMenu = new ContextMenuStrip();

                        var itemSort = new ToolStripMenuItem("Sort");
                        contextMenu.Items.Add(itemSort);

                        var itemSortAsc = new ToolStripMenuItem("Ascending");
                        itemSortAsc.Click += (s, a) =>
                        {
                            lastSortDirection = ListSortDirection.Ascending;
                            table.Sort(column, lastSortDirection);
                        };
                        var itemSortDesc = new ToolStripMenuItem("Descending");
                        itemSortDesc.Click += (s, a) =>
                        {
                            lastSortDirection = ListSortDirection.Descending;
                            table.Sort(column, lastSortDirection);
                        };

                        itemSort.DropDownItems.Add(itemSortAsc);
                        itemSort.DropDownItems.Add(itemSortDesc);

                        contextMenu.Show(null, MousePosition);
                        break;

                }
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                if (table != null)
                    if (column == table.lastSortedColumn)
                    {
                        switch (lastSortDirection)
                        {
                            case ListSortDirection.Ascending:
                                e.Graphics.uwfDrawImage(uwfAppOwner.Resources.ArrowUp, Color.Gray, 8, Height / 2 - 4, 8, 8);
                                break;
                            case ListSortDirection.Descending:
                                e.Graphics.uwfDrawImage(uwfAppOwner.Resources.ArrowDown, Color.Gray, 8, Height / 2 - 4, 8, 8);
                                break;
                        }
                    }
            }

            private ListSortDirection GetNextSortDirection()
            {
                if (lastSortDirection == ListSortDirection.Ascending)
                    return ListSortDirection.Descending;
                return ListSortDirection.Ascending;
            }
        }
        internal class TableRowButton : Button
        {
            internal TableRow row;

            public TableRowButton(TableButtonStyle style)
            {
                BackColor = style.BackColor;
                uwfHoverColor = style.HoverColor;
                uwfBorderColor = style.BorderColor;
                uwfBorderHoverColor = style.BorderHoverColor;
                uwfBorderSelectColor = style.BorderSelectColor;
                Padding = new Padding(8, 0, 8, 0);
                Size = new Size(100, 20);
                TabStop = false;
                TextAlign = ContentAlignment.MiddleRight;
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnMouseClick(e);

                var ne = new MouseEventArgs(e.Button, e.Clicks, e.X + Location.X, e.Y + Location.Y, e.Delta);

                var table = Parent as TableView;
                table.RaiseOnRowClick(row, ne);
            }
        }
    }

    public class TableRow
    {
        internal Button control;
        internal TableRowCollection rowCollection;

        public string CustomHeaderText { get; set; }
        public int Index { get { return rowCollection.FindIndex(this); } }
        public object[] Items { get; internal set; }
        public TableRowControlsCollection ItemsControls { get; internal set; }
        public int Height { get; set; }

        public object this[int column]
        {
            get { return Items[column]; }
            set { Items[column] = value; }
        }

        public TableRow(TableRowCollection owner)
        {
            rowCollection = owner;

            ItemsControls = new TableRowControlsCollection(this, 0);
            Height = 22;
        }

        internal void AddjustItemsCountTo(int n)
        {
            if (Items == null)
                Items = new object[n];
            else if (Items.Length != n)
            {
                var newItems = new object[n];
                if (Items.Length > n)
                    Array.Copy(Items, 0, newItems, 0, newItems.Length);
                else
                    Array.Copy(Items, 0, newItems, 0, Items.Length);
                Items = newItems;
            }
        }
        internal string GetHeaderText()
        {
            if (string.IsNullOrEmpty(CustomHeaderText) == false) return CustomHeaderText;
            if (control == null) return null;

            return (Index + 1).ToString();
        }
        internal void UpdateHeaderText()
        {
            if (control == null) return;
            control.Text = GetHeaderText();
        }

        public class TableRowControlsCollection
        {
            private readonly TableRow row;
            internal Control[] items;

            public delegate Control GetControl(int columnIndex, int rowIndex);
            
            /// <summary>
            /// Sets control when it's became visible.
            /// </summary>
            public GetControl CreateControlOnCellVisible { get; set; }
            public int Length { get { return items.Length; } }

            public Control this[int index]
            {
                get
                {
                    return items[index];
                }
                set
                {
                    if (items[index] != null && items[index].Disposing == false)
                        items[index].Dispose();
                    items[index] = value;
                    
                    row.rowCollection.table.Controls.Add(value);
                    row.rowCollection.table.UpdateRow(row);
                }
            }

            public TableRowControlsCollection(TableRow row, int count)
            {
                this.row = row;
                items = new Control[count];
            }
        }
    }
    public class TableRowCollection
    {
        internal TableView table;

        private readonly List<TableRow> items = new List<TableRow>();

        public TableRowCollection(TableView table)
        {
            this.table = table;
        }

        public int Count { get { return items.Count; } }

        public TableRow this[int index]
        {
            get { return items[index]; }
        }

        public virtual int Add()
        {
            TableRow row = new TableRow(this);
            row.Items = new object[table.Columns.Count];
            return Add(row);
        }
        public virtual int Add(TableRow row)
        {
            items.Add(row);
            table.UpdateRow(row);
            return items.Count - 1;
        }
        public virtual int Add(int count)
        {
            for (int i = 0; i < count; i++)
                Add();
            return items.Count - 1;
        }
        public virtual int Add(params object[] values)
        {
            TableRow row = new TableRow(this);
            row.Items = values;
            return Add(row);
        }
        public void Clear()
        {
            for (; items.Count > 0;)
                RemoveInternal(items[0]);
            
            table.UpdateRows();
        }
        public int FindIndex(TableRow row)
        {
            return items.FindIndex(x => x == row);
        }
        public int Insert(int rowIndex, params object[] values)
        {
            var row = new TableRow(this);
            row.Items = values;

            items.Insert(rowIndex, row);
            table.UpdateRow(row);
            return rowIndex;
        }
        public TableRow Last()
        {
            return items.Last();
        }
        public void Remove(TableRow row)
        {
            RemoveInternal(row);
            table.UpdateRows();
        }

        internal void ClearList()
        {
            items.Clear();
        }
        
        private void RemoveInternal(TableRow row)
        {
            if (row.control != null)
                row.control.Dispose();
            
            if (row.ItemsControls != null)
                for (int i = 0; i < row.ItemsControls.Length; i++)
                {
                    var rowIC = row.ItemsControls[i];
                    if (rowIC != null) rowIC.Dispose();
                }

            items.Remove(row);
        }
    }
    public class TableColumn
    {
        internal TableView.TableColumnButton control;
        private TableColumnCollection owner;

        public int Index { get { return owner.FindIndex(this); } }
        public string HeaderText { get; set; }
        public string Name { get; set; }
        public int Width
        {
            get
            {
                if (control != null)
                    return control.Width;
                return owner.owner.ColumnsDefaultWidth;
            }
            set
            {
                if (control != null)
                {
                    control.Width = value;
                    owner.owner.UpdateColumn(this);
                }
            }
        }

        public TableColumn(TableColumnCollection o)
        {
            owner = o;
        }
    }
    public class TableColumnCollection
    {
        private List<TableColumn> items = new List<TableColumn>();
        internal TableView owner;

        public int Count { get { return items.Count; } }

        public TableColumn this[int index]
        {
            get { return items[index]; }
        }

        public TableColumnCollection(TableView table)
        {
            owner = table;
        }

        public void Add(TableColumn column)
        {
            items.Add(column);
        }
        public void Add(string columnName, string headerText)
        {
            TableColumn column = new TableColumn(this);
            column.Name = columnName;
            column.HeaderText = headerText;
            Add(column);

            owner.UpdateColumn(column);
        }
        public void Clear()
        {
            for (; items.Count > 0;)
                RemoveInternal(items[0]);
            
            owner.UpdateColumns();
        }
        public int FindIndex(TableColumn column)
        {
            return items.FindIndex(x => x == column);
        }
        public TableColumn Last()
        {
            return items.Last();
        }
        public void Remove(TableColumn column)
        {
            RemoveInternal(column);

            owner.UpdateColumns();
        }

        private void RemoveInternal(TableColumn column)
        {
            if (column.control != null)
                column.control.Dispose();
            items.Remove(column);
        }
    }
    public class TableButtonStyle
    {
        public Color BackColor { get; set; }
        public Color BorderColor { get; set; }
        public Color BorderHoverColor { get; set; }
        public Color BorderSelectColor { get; set; }
        public Color HoverColor { get; set; }

        public TableButtonStyle()
        {
            BackColor = Color.FromArgb(252, 252, 252);
            BorderColor = Color.FromArgb(232, 241, 251);
            BorderHoverColor = Color.FromArgb(252, 253, 254);
            BorderSelectColor = BorderHoverColor;
            HoverColor = Color.FromArgb(243, 248, 254);
        }
    }
}
