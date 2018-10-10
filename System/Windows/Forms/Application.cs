namespace System.Windows.Forms
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;

    internal delegate void DragDropRenderHandler(Graphics g);

    public class Application
    {
        internal static Control activeResizeControl;
        internal static CultureInfo currentCulture = ApiHolder.System.CurrentCulture;
        internal static bool dragndrop;
        internal static object dragData;
        internal readonly List<Control> Contexts = new List<Control>();
        internal readonly FormCollection Forms = new FormCollection();
        internal readonly List<Form> ModalForms = new List<Form>();
        internal Control hoveredControl;
        internal AppGdiImages Resources;

        private static DragDropEffects dragControlEffects;
        private static DragDropRenderHandler dragRender;
        private static MouseEvents mouseEvent = 0;
        private static MouseButtons mouseButton = 0;
        private static MouseButtons mouseButtonLastPressed;
        private static Control mouseLastClickControl;
        private static float mouseWheelDelta;
        private static bool mousePositionChanged;
        private static float scaleX = 1f;
        private static float scaleY = 1f;
        private static bool updateHoveredControl;
        private readonly PaintEventArgs paintEventArgs;
        private float mousePositionX;
        private float mousePositionY;
        
        public Application()
        {
            TabSwitching = true;
            
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
                if (TabSwitching && args.KeyCode == Keys.Tab && keyEventType == KeyEvents.Down)
                {
                    var containerControl = GetRootControl(keyControl) as ContainerControl;
                    if (containerControl != null)
                    {
                        if (args.Modifiers == Keys.None)
                            containerControl.RaiseProcessTabKey(true, keyControl);
                        else if (args.Shift)
                            containerControl.RaiseProcessTabKey(false, keyControl);
                    }
                }

                var parentForm = GetParentForm(Control.lastSelected);
                if (parentForm != null && parentForm.KeyPreview)
                    RaiseKeyEvent(args, keyEventType, parentForm); // Raise key event if keyPreview is used.

                RaiseKeyEvent(args, keyEventType, keyControl);
            }
        }
        public void ProccessMouse(MouseEvents mE, float mX, float mY, MouseButtons mButton, int clicks, int delta)
        {
            if (scaleX != 1f || scaleY != 1f)
            {
                mX /= scaleX;
                mY /= scaleY;
            }

            mouseEvent = mE;
            mouseButton = mButton;
            mouseWheelDelta = delta;

            mousePositionChanged = mousePositionX != mX || mousePositionY != mY;
            if (mousePositionChanged)
                updateHoveredControl = true;

            mousePositionX = mX;
            mousePositionY = mY;

            switch (mE)
            {
                case MouseEvents.None:
                    if (mousePositionChanged == false)
                        return;
                    break;
                
                case MouseEvents.Down:
                    mouseButtonLastPressed = mButton;
                    break;
                
                case MouseEvents.Up:
                    if (mouseButtonLastPressed == mButton)
                        mouseButtonLastPressed = MouseButtons.None;
                    break;
            }


            // Dispose context first.
            for (int i = Contexts.Count - 1; i >= 0; i--) // We want to dispose child context first.
            {
                var contextControl = Contexts[i];
                if (!contextControl.uwfContext) continue;

                if (Contains(contextControl, hoveredControl)) continue;
                if (mE != MouseEvents.Down) continue;

                contextControl.Dispose();
            }

            if (hoveredControl == null && mE == MouseEvents.Up)
            {
                dragndrop = false;
                dragData = null;
                dragRender = null;
            }

            if (hoveredControl != null)
                _ProcessControl(new PointF(mX, mY), hoveredControl, false);

            if (mE == MouseEvents.Down)
            {
                var downArgs = new MouseEventArgs(mouseButton, clicks, (int) mX, (int) mY, delta);
                MouseHook.RaiseMouseDown(hoveredControl, downArgs);
            }

            if (mE == MouseEvents.Up)
            {
                var upArgs = new MouseEventArgs(mouseButton, clicks, (int) mX, (int) mY, delta);
                MouseHook.RaiseMouseUp(hoveredControl, upArgs);
            }
        }
        /// <summary>
        /// Redrawing the whole screen.
        /// </summary>
        public void Redraw()
        {
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
            if (hoveredControl != null/* && dragndrop == false*/)
            {
                var mclient = hoveredControl.PointToClient(Control.MousePosition);
                var hargs = new MouseEventArgs(mouseButtonLastPressed, 0, mclient.X, mclient.Y, 0);
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
                    hoveredControl.RaiseOnMouseLeave(new MouseEventArgs(mouseButtonLastPressed, 0, 0, 0, 0));
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
                        controlAtMouse.RaiseOnMouseEnter(new MouseEventArgs(mouseButtonLastPressed, 0, mclient.X, mclient.Y, 0));
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
                            case ControlResizeTypes.DownUp:
                                Cursor.CurrentSystem = Cursors.SizeNS;
                                break;

                            case ControlResizeTypes.Left:
                            case ControlResizeTypes.Right:
                            case ControlResizeTypes.LeftRight:
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
                    if (dragData != null)
                        dragndrop = true;
                }

                if (!contains && mouseEvent != MouseEvents.Up)
                    return true;
                switch (mouseEvent)
                {
                    case MouseEvents.Down:
                        var md_args = new MouseEventArgs(mouseButton, 1, client_mpos.X, client_mpos.Y, 0);
                        control.RaiseOnMouseDown(md_args);
                        mouseLastClickControl = control;
                        return true;
                    case MouseEvents.Up:
                        if (dragndrop)
                        {
                            if (control.AllowDrop)
                            {
                                DataObject dnd_data = new DataObject(dragData);
                                DragEventArgs dnd_args = new DragEventArgs(dnd_data, 0, client_mpos.X, client_mpos.Y, DragDropEffects.None, dragControlEffects);
                                control.RaiseOnDragDrop(dnd_args);
                            }
                            dragData = null;
                            dragndrop = false;
                            return true;
                        }
                        var mu_args = new MouseEventArgs(mouseButton, 1, client_mpos.X, client_mpos.Y, 0);
                        control.RaiseOnMouseUp(mu_args);
                        if (mouseLastClickControl == control)
                            control.RaiseOnMouseClick(mu_args);
                        if (mouseLastClickControl != null && control != mouseLastClickControl)
                            mouseLastClickControl.RaiseOnMouseUp(mu_args);
                        return true;
                    case MouseEvents.DoubleClick:
                        var mdc_args = new MouseEventArgs(mouseButton, 2, client_mpos.X, client_mpos.Y, 0);
                        control.RaiseOnMouseDoubleClick(mdc_args);
                        return true;
                    case MouseEvents.Wheel:
                        var mw_args = new MouseEventArgs(MouseButtons.None, 0, client_mpos.X, client_mpos.Y, (int)(-mouseWheelDelta * 4));
                        control.RaiseOnMouseWheel(mw_args);
                        return true;
                }
            }
            if (!contains)
                control.RaiseOnMouseLeave(null);
            return false;
        }
        private static void RaiseKeyEvent(KeyEventArgs args, KeyEvents keyEventType, Control keyControl)
        {
            switch (keyEventType)
            {
                case KeyEvents.Down:
                    keyControl.RaiseOnKeyDown(args);

                    var lastChar = KeyHelper.GetLastInputChar();
                    if (args.KeyCode == Keys.Space || args.KeyCode == Keys.Back || char.IsControl(lastChar) == false)
                        keyControl.RaiseOnKeyPress(new KeyPressEventArgs(lastChar));

                    break;
                case KeyEvents.Up:
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
