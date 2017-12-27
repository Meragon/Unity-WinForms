namespace System.Windows.Forms
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;

    using UnityEngine;

    using Graphics = System.Drawing.Graphics;

    internal delegate void DragDropRenderHandler(Graphics g);

    public class Application
    {
        internal static Control activeResizeControl;
        internal static CultureInfo currentCulture;
        internal static bool dragndrop;
        internal static object dragData;
        internal readonly List<Control> Contexts = new List<Control>();
        internal readonly FormCollection Forms = new FormCollection();
        internal readonly List<Control> HoveredControls = new List<Control>();
        internal readonly List<Form> ModalForms = new List<Form>();
        internal Control hoveredControl;
        internal AppGdiImages Resources;

        private static DragDropEffects dragControlEffects;
        private static DragDropRenderHandler dragRender;
        private static Keys currentKeyDown = Keys.None;
        private static MouseEvents mouseEvent = 0;
        private static MouseButtons mouseButton = 0;
        private static Control mouseLastClickControl;
        private static float mouseWheelDelta;
        private static bool mousePositionChanged;
        private static float scaleX = 1f;
        private static float scaleY = 1f;
        private static bool updateHoveredControl;
        private readonly PaintEventArgs paintEventArgs;
        private float mousePositionX;
        private float mousePositionY;
        private MouseEvents userMouseEvent;
        private MouseEventArgs userMouseArgs;

        public Application()
        {
            TabSwitching = true;

            currentCulture = ApiHolder.System.CurrentCulture;
            paintEventArgs = new PaintEventArgs();
            paintEventArgs.Graphics = new Graphics();

            Cursor.Current = Cursors.Default;
        }

        public delegate void UpdateEventDelegate();

        internal static event UpdateEventDelegate UpdateEvent;

        public enum MouseEvents
        {
            None,
            Down,
            Up,
            Wheel,
            DoubleClick,

            Drag,
            Drop
        }
        public enum KeyEvents
        {
            None,
            Down,
            Up
        }

        internal static bool IsDraging { get { return dragndrop; } }
        internal static bool IsStandalone
        {
            get
            {
#if UNITY_STANDALONE
                return true;
#else
                return false;
#endif
            }
        }
        internal static float ScaleX
        {
            get { return scaleX; }
            set
            {
                scaleX = value;
                if (value < .1f)
                    scaleX = 1f;
            }
        }
        internal static float ScaleY
        {
            get { return scaleY; }
            set
            {
                scaleY = value;
                if (value < .1f)
                    scaleY = 1f;
            }
        }
        internal bool TabSwitching { get; set; }

        public void ProccessKeys(KeyEventArgs args, KeyEvents keyEventType)
        {
            // Raise hook events.
            switch (keyEventType)
            {
                case KeyEvents.Down:
                    KeyboardHook.RaiseKeyDown(this, args);
                    break;
                case KeyEvents.Up:
                    KeyboardHook.RaiseKeyUp(this, args);
                    break;
            }

            // Close context if possible.
            if (args.KeyCode == Keys.Escape && keyEventType == KeyEvents.Down)
            {
                if (Contexts.Count > 0)
                {
                    Contexts[0].Dispose();
                    return;
                }
            }

            // Raise keys on selected controls if possible.
            if (Control.lastSelected != null && Control.lastSelected.IsDisposed == false)
            {
                var keyControl = Control.lastSelected;

                // Tab switching through controls.
                if (TabSwitching && Event.current.keyCode == KeyCode.Tab && keyEventType == KeyEvents.Down)
                {
                    if (Event.current.modifiers == EventModifiers.None)
                        NextTabControl(keyControl);
                    else if (Event.current.modifiers == EventModifiers.Shift)
                        PrevTabControl(keyControl);
                }

                var parentForm = GetParentForm(Control.lastSelected);
                if (parentForm != null && parentForm.KeyPreview)
                    RaiseKeyEvent(args, keyEventType, parentForm); // Raise key event if keyPreview is used.

                RaiseKeyEvent(args, keyEventType, keyControl);
            }

            if (keyEventType == KeyEvents.Down)
                currentKeyDown = args.KeyCode;
        }
        public void ProccessMouse(float mouseX, float mouseY)
        {
            if (scaleX != 1f || scaleY != 1f)
            {
                mouseX /= scaleX;
                mouseY /= scaleY;
            }

            mouseEvent = MouseEvents.None;
            mouseButton = MouseButtons.None;

            mousePositionChanged = mousePositionX != mouseX || mousePositionY != mouseY;
            if (mousePositionChanged)
                updateHoveredControl = true;

            mousePositionX = mouseX;
            mousePositionY = mouseY;

            #region Set events.

            int eventButton = -1;
            int eventClicks = 0;
            float eventDelta = 0;
            EventType eventType = EventType.Ignore;

            if (userMouseArgs != null)
            {
                switch (userMouseArgs.Button)
                {
                    case MouseButtons.Left: eventButton = 0; break;
                    case MouseButtons.Right: eventButton = 1; break;
                    case MouseButtons.Middle: eventButton = 2; break;
                }
                eventClicks = userMouseArgs.Clicks;
                eventDelta = userMouseArgs.Delta;
                switch (userMouseEvent)
                {
                    case MouseEvents.Down: eventType = EventType.MouseDown; break;
                    case MouseEvents.Up: eventType = EventType.MouseUp; break;
                    case MouseEvents.Wheel: eventType = EventType.ScrollWheel; break;
                }
                userMouseArgs = null;
            }
            else
            {
                eventButton = Event.current.button;
                eventClicks = Event.current.clickCount;
                eventDelta = Event.current.delta.y;
                eventType = Event.current.type;
            }

            switch (eventType)
            {
                case EventType.MouseDown:
                    switch (eventButton)
                    {
                        case 0:
                            mouseButton = MouseButtons.Left;
                            mouseEvent = MouseEvents.Down;
                            if (eventClicks > 1)
                                mouseEvent = MouseEvents.DoubleClick;
                            break;
                        case 1:
                            mouseButton = MouseButtons.Right;
                            mouseEvent = MouseEvents.Down;
                            break;
                        case 2:
                            mouseButton = MouseButtons.Middle;
                            mouseEvent = MouseEvents.Down;
                            break;
                    }
                    break;
                case EventType.MouseUp:
                    switch (eventButton)
                    {
                        case 0:
                            mouseButton = MouseButtons.Left;
                            mouseEvent = MouseEvents.Up;
                            break;
                        case 1:
                            mouseButton = MouseButtons.Right;
                            mouseEvent = MouseEvents.Up;
                            break;
                        case 2:
                            mouseButton = MouseButtons.Middle;
                            mouseEvent = MouseEvents.Up;
                            break;
                    }
                    break;
                case EventType.ScrollWheel:
                    mouseEvent = MouseEvents.Wheel;
                    mouseWheelDelta = eventDelta;
                    break;
            }

            #endregion

            //if (_mouseLastClickControl != null && _mouseEvent == MouseEvents.None && _mouseMovePosition != mousePosition)
            //    _ProcessControl(mousePosition, _mouseLastClickControl, true);

            if (mouseEvent == MouseEvents.None && !mousePositionChanged) return;

            // Dispose context first.
            for (int i = Contexts.Count - 1; i >= 0; i--) // We want to dispose child context first.
            {
                var contextControl = Contexts[i];
                if (!contextControl.uwfContext) continue;

                if (Contains(contextControl, hoveredControl)) continue;
                if (mouseEvent != MouseEvents.Down) continue;

                contextControl.Dispose();
            }

            if (hoveredControl == null && mouseEvent == MouseEvents.Up)
            {
                dragndrop = false;
                dragData = null;
                dragRender = null;
            }

            if (hoveredControl != null)
                _ProcessControl(new PointF(mouseX, mouseY), hoveredControl, false);

            if (mouseEvent == MouseEvents.Down)
            {
                var downArgs = new MouseEventArgs(mouseButton, 1, (int)mouseX, (int)mouseY, 0);
                MouseHook.RaiseMouseDown(hoveredControl, downArgs);
            }

            if (mouseEvent == MouseEvents.Up)
            {
                var upArgs = new MouseEventArgs(mouseButton, 1, (int)mouseX, (int)mouseY, 0);
                MouseHook.RaiseMouseUp(hoveredControl, upArgs);
            }
        }
        public void ProccessMouse(PointF mousePosition)
        {
            ProccessMouse(mousePosition.X, mousePosition.Y);
        }
        public void RaiseMouseEvent(MouseEvents mEv, MouseEventArgs mArgs)
        {
            userMouseEvent = mEv;
            userMouseArgs = mArgs;

            ProccessMouse(new Drawing.PointF(mArgs.X, mArgs.Y));
        }
        /// <summary>
        /// Redrawing the whole screen.
        /// </summary>
        public void Redraw()
        {
            // Scale if needed.
            if (scaleX != 1f || scaleY != 1f)
                UnityEngine.GUI.matrix = UnityEngine.Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(0, Vector3.up), new Vector3(scaleX, scaleY, 1));

            paintEventArgs.Graphics.Clear(System.Drawing.Color.White);

            for (int i = 0; i < Forms.Count; i++)
            {
                var form = Forms[i];
                if (form.Visible) form.RaiseOnPaint(paintEventArgs);
            }

            for (int i = 0; i < ModalForms.Count; i++)
            {
                var form = ModalForms[i];
                if (form.Visible) form.RaiseOnPaint(paintEventArgs);
            }

            for (int i = 0; i < Contexts.Count; i++)
            {
                var context = Contexts[i];
                if (context.Visible) context.RaiseOnPaint(paintEventArgs);
            }

            if (dragRender != null && dragndrop)
            {
                var g = new Graphics();
                var dragRenderControl = new Control();
                dragRender.Invoke(g);
                dragRenderControl.Dispose();
            }

            // ToolTip.
            ToolTip.OnPaint(paintEventArgs);

            var cursor = Cursor.CurrentSystem ?? Cursor.Current;
            var cursorSize = cursor.Size;
            var mousePosition = Control.MousePosition;
            cursor.Draw(
                paintEventArgs.Graphics,
                new Drawing.Rectangle(
                    mousePosition.X,
                    mousePosition.Y,
                    (int)(cursorSize.Width / scaleX),
                    (int)(cursorSize.Height / scaleY)));
        }
        public void Run(Control control)
        {
            control.uwfAppOwner = this;
            //this.Controls.Add(control);
        }
        public void Update()
        {
            // Update hovered control.
            if (hoveredControl != null && dragndrop == false)
            {
                var mclient = hoveredControl.PointToClient(Control.MousePosition);
                var hargs = new MouseEventArgs(MouseButtons.None, 0, mclient.X, mclient.Y, 0);
                hoveredControl.RaiseOnMouseHover(hargs);
                if (updateHoveredControl)
                    hoveredControl.RaiseOnMouseMove(hargs);
            }

            if (updateHoveredControl)
            {
                var controlAtMouse = _ControlAt(Control.MousePosition);
                if (hoveredControl != controlAtMouse && hoveredControl != null)
                {
                    hoveredControl.hovered = false;
                    hoveredControl.mouseEntered = false;
                    hoveredControl.RaiseOnMouseLeave(new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
                    if (dragndrop)
                        hoveredControl.RaiseOnDragLeave(EventArgs.Empty);
                }
                if (controlAtMouse == null)
                    hoveredControl = null;
                else
                {
                    hoveredControl = controlAtMouse;
                    if (controlAtMouse.mouseEntered == false)
                    {
                        var mclient = controlAtMouse.PointToClient(Control.MousePosition);

                        controlAtMouse.hovered = true;
                        controlAtMouse.mouseEntered = true;
                        controlAtMouse.RaiseOnMouseEnter(new MouseEventArgs(MouseButtons.None, 0, mclient.X, mclient.Y, 0));
                        if (dragndrop)
                            controlAtMouse.RaiseOnDragEnter(new DragEventArgs(new DataObject(dragData), 0, mclient.X, mclient.Y, DragDropEffects.None, dragControlEffects));
                    }
                }

                updateHoveredControl = false;
            }

            // Update cursor for resize events.
            if (activeResizeControl == null)
            {
                if (hoveredControl == null)
                    Cursor.CurrentSystem = null;
                else
                {
                    var iResizableControl = hoveredControl as IResizableControl;
                    var resizableControl = iResizableControl as Control;
                    if (iResizableControl != null && resizableControl != null)
                    {
                        var formClientPosition = resizableControl.PointToClient(Control.MousePosition);
                        var hoveredFormResize = iResizableControl.GetResizeAt(formClientPosition);
                        switch (hoveredFormResize)
                        {
                            default:
                            case ControlResizeTypes.None:
                                Cursor.CurrentSystem = null;
                                break;

                            case ControlResizeTypes.Down:
                            case ControlResizeTypes.Up:
                                Cursor.CurrentSystem = Cursors.SizeNS;
                                break;

                            case ControlResizeTypes.Left:
                            case ControlResizeTypes.Right:
                                Cursor.CurrentSystem = Cursors.SizeWE;
                                break;

                            case ControlResizeTypes.LeftDown:
                            case ControlResizeTypes.RightUp:
                                Cursor.CurrentSystem = Cursors.SizeNESW;
                                break;

                            case ControlResizeTypes.LeftUp:
                            case ControlResizeTypes.RightDown:
                                Cursor.CurrentSystem = Cursors.SizeNWSE;
                                break;
                        }
                    }
                    else
                        Cursor.CurrentSystem = null;
                }
            }

            var updateEventHandler = UpdateEvent;
            if (updateEventHandler != null)
                updateEventHandler();
        }

        internal static bool ControlIsVisible(Control control)
        {
            if (control.Visible == false) return false;

            var co = control.uwfOffset;
            var controlLocation = control.Location;
            var controlLocationX = controlLocation.X;
            var controlLocationY = controlLocation.Y;
            var controluwfOffsetX = co.X;
            var controluwfOffsetY = co.Y;
            var controlWidth = control.Width;
            var controlHeight = control.Height;

            if (controlLocationX + controluwfOffsetX + controlWidth < 0) return false;
            if (controlLocationY + controluwfOffsetY + controlHeight < 0) return false;

            var controlParent = control.Parent;
            if (controlParent != null)
            {
                if (controlLocationX + controluwfOffsetX > controlParent.Width) return false;
                if (controlLocationY + controluwfOffsetY > controlParent.Height) return false;
            }
            return true;
        }
        internal static void DoDragDrop(object data, DragDropEffects effect, DragDropRenderHandler render = null)
        {
            dragData = data;
            dragControlEffects = effect;
            dragRender = render;
        }
        internal static Form GetParentForm(Control control)
        {
            if (control == null) return null;
            var form = control.Parent as Form;
            if (form != null) return form;

            return GetParentForm(control.Parent);
        }
        internal static Control GetRootControl(Control control)
        {
            if (control == null) return null;

            if (control.Parent != null)
                return GetRootControl(control.Parent);

            return control;
        }
        internal static void NextTabControl(Control control)
        {
            var controlForm = GetRootControl(control) as Form;
            if (controlForm == null || controlForm.Controls.Count <= 0) return;

            var formControls = new List<Control>();
            _FillListWithVisibleControls(controlForm, formControls);

            var possibleControls = formControls.FindAll(x => x.IsDisposed == false && x.CanSelect && x.TabStop);
            if (possibleControls.Count == 0) return;

            possibleControls.Sort(TabComparison);

            int controlIndex = possibleControls.FindIndex(x => x == control);

            var nextControlIndex = controlIndex + 1;
            if (nextControlIndex >= possibleControls.Count)
                nextControlIndex = 0;
            possibleControls[nextControlIndex].Focus();
        }
        internal static void PrevTabControl(Control control)
        {
            var controlForm = GetRootControl(control) as Form;
            if (controlForm == null || controlForm.Controls.Count <= 0) return;

            var formControls = new List<Control>();
            _FillListWithVisibleControls(controlForm, formControls);

            var possibleControls = formControls.FindAll(x => x.Visible && x.IsDisposed == false && x.CanSelect && x.TabStop);
            if (possibleControls.Count == 0) return;

            possibleControls.Sort(TabComparison);

            int controlIndex = possibleControls.FindIndex(x => x == control);

            var nextControlIndex = controlIndex - 1;
            if (nextControlIndex < 0)
                nextControlIndex = possibleControls.Count - 1;
            possibleControls[nextControlIndex].Focus();
        }
        internal void UpdatePaintClipRect()
        {
            paintEventArgs.ClipRectangle = new Rectangle(0, 0, Screen.width, Screen.height);
        }

        private static bool Contains(Control parent, Control child)
        {
            if (parent == child) return true;

            for (int i = 0; i < parent.Controls.Count; i++)
            {
                var c = parent.Controls[i];
                if (c == child) return true;

                if (Contains(c, child))
                    return true;
            }

            return false;
        }
        private static Control FindControlAt(Control currentControl, Point position)
        {
            for (int i = currentControl.Controls.Count - 1; i >= 0; i--)
            {
                var child = currentControl.Controls[i];
                if (child.Visible == false || child.Enabled == false) continue;

                var childMClient = child.PointToClient(position);
                if (child.DisplayRectangle.Contains(childMClient))
                {
                    currentControl = child;
                    return FindControlAt(currentControl, position);
                }
            }

            return currentControl;
        }
        private static void _FillListWithVisibleControls(Control control, List<Control> list)
        {
            for (int i = 0; i < control.Controls.Count; i++)
            {
                var c = control.Controls[i];
                if (c.Visible)
                {
                    list.Add(c);
                    _FillListWithVisibleControls(c, list);
                }
            }
        }
        private static Control _ParentContains(Control control, PointF mousePosition, Control currentControl, ref bool ok)
        {
            //Application.Log(control.Name);
            if (control == null || control.Parent == null) return currentControl;

            var parentLocation = control.Parent.PointToScreen(System.Drawing.Point.Empty);
            var parentRect = new System.Drawing.RectangleF(parentLocation.X, parentLocation.Y, control.Parent.Width, control.Parent.Height);
            if (parentRect.Contains(mousePosition) == true)
                currentControl = control.Parent;
            else
                ok = false; // Control is not visible due to groups;

            return _ParentContains(control.Parent, mousePosition, currentControl, ref ok);
        }
        private static bool _ProcessControl(PointF mousePosition, Control control, bool ignore_rect)
        {
            // ignore_rect will call mouse_up & mouse_move in any case.
            var c_location = control.PointToScreen(System.Drawing.Point.Empty);
            var clientRect = new System.Drawing.RectangleF(c_location.X, c_location.Y, control.Width, control.Height);
            var contains = clientRect.Contains(mousePosition);

            if (contains && (mouseEvent == MouseEvents.Down) || mouseEvent == MouseEvents.Up)
            {
                if (control.Parent != null)
                {
                    bool ok = true;
                    var clickedControl = _ParentContains(control, mousePosition, control, ref ok);
                    if (clickedControl != null && ok == false)
                        control = clickedControl;
                }
            }

            if (ignore_rect || contains)
            {
                var client_mpos = control.PointToClient(new Point((int)mousePosition.X, (int)mousePosition.Y));
                if (mousePositionChanged)
                {
                    var m_args = new MouseEventArgs(MouseButtons.None, 0, (int)client_mpos.X, (int)client_mpos.Y, 0);
                    if (dragData != null)
                        dragndrop = true;
                    //else
                    //control.RaiseOnMouseMove(m_args);
                }

                if (!contains && mouseEvent != MouseEvents.Up)
                    return true;
                switch (mouseEvent)
                {
                    case MouseEvents.Down:
                        var md_args = new MouseEventArgs(mouseButton, 1, (int)client_mpos.X, (int)client_mpos.Y, 0);
                        control.RaiseOnMouseDown(md_args);
                        mouseLastClickControl = control;
                        return true;
                    case MouseEvents.Up:
                        if (dragndrop)
                        {
                            if (control.AllowDrop)
                            {
                                DataObject dnd_data = new DataObject(dragData);
                                DragEventArgs dnd_args = new DragEventArgs(dnd_data, 0, (int)client_mpos.X, (int)client_mpos.Y, DragDropEffects.None, dragControlEffects);
                                control.RaiseOnDragDrop(dnd_args);
                            }
                            dragData = null;
                            dragndrop = false;
                            return true;
                        }
                        var mu_args = new MouseEventArgs(mouseButton, 1, (int)client_mpos.X, (int)client_mpos.Y, 0);
                        control.RaiseOnMouseUp(mu_args);
                        if (mouseLastClickControl == control)
                            control.RaiseOnMouseClick(mu_args);
                        if (mouseLastClickControl != null && control != mouseLastClickControl)
                            mouseLastClickControl.RaiseOnMouseUp(mu_args);
                        return true;
                    case MouseEvents.DoubleClick:
                        var mdc_args = new MouseEventArgs(mouseButton, 2, (int)client_mpos.X, (int)client_mpos.Y, 0);
                        control.RaiseOnMouseDoubleClick(mdc_args);
                        return true;
                    case MouseEvents.Wheel:
                        var mw_args = new MouseEventArgs(MouseButtons.Middle, 0, (int)client_mpos.X, (int)client_mpos.Y, (int)(-mouseWheelDelta * 4));
                        control.RaiseOnMouseWheel(mw_args);
                        return true;
                }
            }
            if (!contains)
                control.RaiseOnMouseLeave(null);
            return false;
        }
        private static int TabComparison(Control c1, Control c2)
        {
            if (c1.TabIndex >= 0 || c2.TabIndex >= 0)
                return c1.TabIndex.CompareTo(c2.TabIndex);

            var c1Location = c1.Location;
            var c2Location = c2.Location;

            if (c1Location.Y != c2Location.Y)
                return c1Location.Y.CompareTo(c2Location.Y);
            if (c1Location.X == c2Location.X)
                return 0;

            return c1Location.X.CompareTo(c2Location.X);
        }
        private static void RaiseKeyEvent(KeyEventArgs args, KeyEvents keyEventType, Control keyControl)
        {
            switch (keyEventType)
            {
                case KeyEvents.Down:
                    if (currentKeyDown == Keys.None || currentKeyDown != args.KeyCode)
                        keyControl.RaiseOnKeyDown(args);

                    var pressArgs = new KeyPressEventArgs(KeyHelper.GetLastInputChar());
                    pressArgs.uwfKeyArgs = args;

                    keyControl.RaiseOnKeyPress(pressArgs);
                    break;
                case KeyEvents.Up:
                    currentKeyDown = Keys.None;
                    keyControl.RaiseOnKeyUp(args);
                    break;
            }
        }
        private Control _ControlAt(Point mousePosition)
        {
            Control control = null;

            if (Contexts.Count > 0)
            {
                for (int i = 0; i < Contexts.Count; i++)
                {
                    var contextControl = Contexts[i];
                    var cRect = new System.Drawing.Rectangle(contextControl.Location.X, contextControl.Location.Y, contextControl.Width, contextControl.Height);
                    if (cRect.Contains(mousePosition))
                    {
                        control = contextControl;
                        break;
                    }
                }
            }
            if (ModalForms.Count > 0)
            {
                if (control == null)
                {
                    var lastModalForm = ModalForms.Last();
                    var formRect = new System.Drawing.Rectangle(lastModalForm.Location.X, lastModalForm.Location.Y, lastModalForm.Width, lastModalForm.Height);
                    if (formRect.Contains(mousePosition))
                        control = lastModalForm;
                }
            }
            else
            {
                if (control == null)
                    for (int i = Forms.Count - 1; i >= 0; i--)
                    {
                        var form = Forms[i];
                        if (form.TopMost && form.Visible && form.Enabled)
                        {
                            var formRect = new System.Drawing.Rectangle(form.Location.X, form.Location.Y, form.Width, form.Height);
                            if (formRect.Contains(mousePosition))
                            {
                                control = form;
                                break;
                            }
                        }
                    }

                if (control == null)
                    for (int i = Forms.Count - 1; i >= 0; i--)
                    {
                        var form = Forms[i];
                        if (form.TopMost == false && form.Visible && form.Enabled)
                        {
                            var formRect = new System.Drawing.Rectangle(form.Location.X, form.Location.Y, form.Width, form.Height);
                            if (formRect.Contains(mousePosition))
                            {
                                control = form;
                                break;
                            }
                        }
                    }
            }

            if (control != null)
                control = FindControlAt(control, mousePosition);

            return control;
        }
    }
}
