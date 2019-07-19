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
        internal readonly List<Control> Contexts = new List<Control>();
        internal readonly FormCollection Forms = new FormCollection();
        internal readonly List<Form> ModalForms = new List<Form>();
        internal Control hoveredControl;
        internal AppGdiImages Resources;

        private static DragDropEffects dragControlEffects;
        private static DragDropRenderHandler dragRender;
        private static float scaleX = 1f;
        private static float scaleY = 1f;
        private readonly PaintEventArgs paintEventArgs;

        private MouseEvents  mouseEvent  = 0;
        private MouseButtons mouseButton = 0;
        private MouseButtons mouseButtonLastPressed;
        private Control      mouseLastClickControl;
        private float        mouseWheelDelta;
        private bool         mousePositionChanged;
        private float        mousePositionX;
        private float        mousePositionY;
        
        private bool updateHoveredControl;

        static Application()
        {
            if (ApiHolder.System != null)
                CurrentCulture = ApiHolder.System.CurrentCulture;
        }
        
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

        internal static CultureInfo CurrentCulture { get; set; }
        internal static object DraggingData { get; private set; }
        internal static bool IsDragging { get; private set; }
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

        public void ProcessKeys(KeyEventArgs args, KeyEvents keyEventType)
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
            if (Control.lastSelected != null && !Control.lastSelected.IsDisposed)
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
        public void ProcessMouse(MouseEvents mE, float mX, float mY, MouseButtons mButton, int clicks, int delta)
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

            switch (mouseEvent)
            {
                case MouseEvents.None:
                    if (!mousePositionChanged)
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
                if (mouseEvent != MouseEvents.Down) continue;

                contextControl.Dispose();
            }

            if (hoveredControl == null && mouseEvent == MouseEvents.Up)
            {
                IsDragging = false;
                DraggingData = null;
                dragRender = null;
            }

            if (hoveredControl != null)
                RaiseMouseEvents(new PointF(mX, mY), hoveredControl, false);

            if (mouseEvent == MouseEvents.Down)
            {
                var downArgs = new MouseEventArgs(mouseButton, clicks, (int) mX, (int) mY, delta);
                MouseHook.RaiseMouseDown(hoveredControl, downArgs);
            }

            if (mouseEvent == MouseEvents.Up)
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

            if (dragRender != null && IsDragging)
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
        }
        public void Update()
        {
            UpdateHoveredControl();
            UpdateResizeCursor();

            var updateEventHandler = UpdateEvent;
            if (updateEventHandler != null)
                updateEventHandler();
        }
        
        internal static bool ControlIsVisible(Control control)
        {
            if (!control.Visible) return false;

            var co = control.uwfOffset;
            var controlLocation = control.Location;
            var controlLocationX = controlLocation.X;
            var controlLocationY = controlLocation.Y;
            var controlOffsetX = co.X;
            var controlOffsetY = co.Y;
            var controlWidth = control.Width;
            var controlHeight = control.Height;

            if (controlLocationX + controlOffsetX + controlWidth < 0) return false;
            if (controlLocationY + controlOffsetY + controlHeight < 0) return false;

            var controlParent = control.Parent;
            if (controlParent != null)
            {
                if (controlLocationX + controlOffsetX > controlParent.Width) return false;
                if (controlLocationY + controlOffsetY > controlParent.Height) return false;
            }
            return true;
        }
        internal static void DoDragDrop(object data, DragDropEffects effect, DragDropRenderHandler render = null)
        {
            DraggingData = data;
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
        internal void LostFocus()
        {
            if (hoveredControl != null)
                hoveredControl.RaiseOnLostFocus(EventArgs.Empty);
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
            // Parent bounds.
            var minX = 0;
            var minY = 0;
            var maxX = currentControl.Width;
            var maxY = currentControl.Height;
            
            // Should fix clicking on forms header issue.
            var currentForm = currentControl as Form;
            if (currentForm != null)
                minY = currentForm.uwfHeaderHeight;
            
            var mouseRelativePosition = currentControl.PointToClient(position);
            
            for (int i = currentControl.Controls.Count - 1; i >= 0; i--)
            {
                var child = currentControl.Controls[i];
                if (!child.Visible || !child.Enabled) continue;
            
                // Base child bounds.
                var childOffset = child.uwfOffset;
                var childX1 = child.Location.X + childOffset.X;
                var childX2 = childX1 + child.Width;
                var childY1 = child.Location.Y + childOffset.Y;
                var childY2 = childY1 + child.Height;

                if (!child.uwfSystem)
                {
                    // Skip if out of bounds.
                    if (childX1 > maxX ||
                        childY1 > maxY ||
                        childX2 < minX ||
                        childY2 < minY) continue;

                    // Fix.
                    childX1 = Math.Max(minX, childX1);
                    childX2 = Math.Min(maxX, childX2);
                    childY1 = Math.Max(minY, childY1);
                    childY2 = Math.Min(maxY, childY2);

                    if (childX2 < childX1 ||
                        childY2 < childY1) continue;
                }
                
                var childRect = new Rectangle(childX1, childY1, childX2 - childX1, childY2 - childY1);
                if (childRect.Contains(mouseRelativePosition))
                {
                    currentControl = child;
                    return FindControlAt(currentControl, position);
                }
            }

            return currentControl;
        }
        private void RaiseMouseEvents(PointF mousePosition, Control control, bool ignoreRect)
        {
            var mousePositionInt = new Point((int) mousePosition.X, (int) mousePosition.Y);
            var mouseRelativePosition = control.PointToClient(mousePositionInt);
            var controlContainsMousePosition = control.ClientRectangle.Contains(mouseRelativePosition);

            if (ignoreRect || controlContainsMousePosition)
            {
                if (mousePositionChanged)
                {
                    if (DraggingData != null)
                        IsDragging = true;
                }
                
                if (!controlContainsMousePosition && mouseEvent != MouseEvents.Up)
                    return;
                
                switch (mouseEvent)
                {
                    case MouseEvents.Down:
                    {
                        var mouseEventArgs = new MouseEventArgs(mouseButton, 1, mouseRelativePosition.X, mouseRelativePosition.Y, 0);
                        control.RaiseOnMouseDown(mouseEventArgs);
                        mouseLastClickControl = control;
                        return;
                    }

                    case MouseEvents.Up:
                    {
                        if (IsDragging)
                        {
                            if (control.AllowDrop)
                            {
                                var dndData = new DataObject(DraggingData);
                                var dndArgs = new DragEventArgs(dndData,
                                    0,
                                    mouseRelativePosition.X,
                                    mouseRelativePosition.Y,
                                    DragDropEffects.None,
                                    dragControlEffects);
                                control.RaiseOnDragDrop(dndArgs);
                            }

                            DraggingData = null;
                            IsDragging = false;
                            return;
                        }

                        var mouseEventArgs = new MouseEventArgs(mouseButton, 1, mouseRelativePosition.X, mouseRelativePosition.Y, 0);
                        control.RaiseOnMouseUp(mouseEventArgs);
                        
                        if (mouseLastClickControl == control)
                            control.RaiseOnMouseClick(mouseEventArgs);
                        
                        if (mouseLastClickControl != null && control != mouseLastClickControl)
                            mouseLastClickControl.RaiseOnMouseUp(mouseEventArgs);
                        return;
                    }

                    case MouseEvents.DoubleClick:
                    {
                        var mouseEventArgs = new MouseEventArgs(mouseButton, 2, mouseRelativePosition.X, mouseRelativePosition.Y, 0);
                        control.RaiseOnMouseDoubleClick(mouseEventArgs);
                        return;
                    }

                    case MouseEvents.Wheel:
                    {
                        var mouseWheelDeltaCorrected = (int) (-mouseWheelDelta * 4); 
                        var mw_args = new MouseEventArgs(MouseButtons.None,
                            0,
                            mouseRelativePosition.X,
                            mouseRelativePosition.Y,
                            mouseWheelDeltaCorrected);
                        control.RaiseOnMouseWheel(mw_args);
                        updateHoveredControl = true;
                        return;
                    }
                }
            }
            
            if (!controlContainsMousePosition)
                control.RaiseOnMouseLeave(null);
        }
        private void RaiseKeyEvent(KeyEventArgs args, KeyEvents keyEventType, Control keyControl)
        {
            switch (keyEventType)
            {
                case KeyEvents.Down:
                    keyControl.RaiseOnKeyDown(args);

                    var lastChar = KeyHelper.GetLastInputChar();
                    if (args.KeyCode == Keys.Space || args.KeyCode == Keys.Back || !char.IsControl(lastChar))
                        keyControl.RaiseOnKeyPress(new KeyPressEventArgs(lastChar));

                    break;
                case KeyEvents.Up:
                    keyControl.RaiseOnKeyUp(args);
                    break;
            }
        }
        private Control ControlAt(Point mousePosition)
        {
            Control control = null;

            if (Contexts.Count > 0)
            {
                for (int i = 0; i < Contexts.Count; i++)
                {
                    var contextControl = Contexts[i];
                    var cRect = new Rectangle(contextControl.Location.X, contextControl.Location.Y, contextControl.Width, contextControl.Height);
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
                    var formRect = new Rectangle(lastModalForm.Location.X, lastModalForm.Location.Y, lastModalForm.Width, lastModalForm.Height);
                    if (formRect.Contains(mousePosition))
                        control = lastModalForm;
                }
            }
            else
            {
                var FormAt = new Func<FormCollection, Predicate<Form>, Point, Form>((collection, match, position) =>
                {
                    for (int i = collection.Count - 1; i >= 0; i--)
                    {
                        var form = collection[i];
                        if (!match(form)) continue;
                
                        var formRect = new Rectangle(form.Location.X, form.Location.Y, form.Width, form.Height);
                        if (formRect.Contains(position))
                            return form;
                    }

                    return null;                    
                });
                
                if (control == null)
                {
                    var formAt = FormAt(Forms, form => form.TopMost && form.Visible && form.Enabled, mousePosition);
                    if (formAt != null)
                        control = formAt;
                }

                if (control == null)
                {
                    var formAt = FormAt(Forms, form => !form.TopMost && form.Visible && form.Enabled, mousePosition);
                    if (formAt != null)
                        control = formAt;
                }
            }

            if (control != null)
                control = FindControlAt(control, mousePosition);

            return control;
        }
        private void UpdateHoveredControl()
        {
            if (hoveredControl != null /* && dragndrop == false*/)
            {
                var mclient = hoveredControl.PointToClient(Control.MousePosition);
                var hargs = new MouseEventArgs(mouseButtonLastPressed, 0, mclient.X, mclient.Y, 0);
                hoveredControl.RaiseOnMouseHover(hargs);
                if (updateHoveredControl)
                    hoveredControl.RaiseOnMouseMove(hargs);
            }

            if (updateHoveredControl)
            {
                var controlAtMouse = ControlAt(Control.MousePosition);
                if (hoveredControl != controlAtMouse && hoveredControl != null)
                {
                    hoveredControl.hovered = false;
                    hoveredControl.mouseEntered = false;
                    hoveredControl.RaiseOnMouseLeave(new MouseEventArgs(mouseButtonLastPressed, 0, 0, 0, 0));
                    if (IsDragging)
                        hoveredControl.RaiseOnDragLeave(EventArgs.Empty);
                }

                if (controlAtMouse == null)
                    hoveredControl = null;
                else
                {
                    hoveredControl = controlAtMouse;
                    if (!controlAtMouse.mouseEntered)
                    {
                        var mclient = controlAtMouse.PointToClient(Control.MousePosition);

                        controlAtMouse.hovered = true;
                        controlAtMouse.mouseEntered = true;
                        controlAtMouse.RaiseOnMouseEnter(new MouseEventArgs(mouseButtonLastPressed, 0, mclient.X, mclient.Y, 0));
                        if (IsDragging)
                            controlAtMouse.RaiseOnDragEnter(new DragEventArgs(new DataObject(DraggingData), 0, mclient.X, mclient.Y,
                                DragDropEffects.None, dragControlEffects));
                    }
                }

                updateHoveredControl = false;
            }
        }
        private void UpdateResizeCursor()
        {
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
        }
    }
}
