using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;
using Color = UnityEngine.Color;

namespace System.Windows.Forms
{
    public class Application
    {
        private static bool _dragndrop;
        private static object _dragData;
        private static DragDropEffects _dragControlEffects;
        private static DragDropRenderHandler _dragRender;
        private static KeyCode _currentKeyDown = KeyCode.None;
        private static readonly List<_HotKey> _hotKeys = new List<_HotKey>();
        private static MouseEvents _mouseEvent = 0;
        private static MouseButtons _mouseButton = 0;
        private static Control _mouseLastClickControl;
        private static float _mouseWheelDelta;
        private static bool mousePositionChanged;
        private float mousePositionX;
        private float mousePositionY;
        private readonly PaintEventArgs _paintEventArgs;
        private static float _scaleX = 1f;
        private static float _scaleY = 1f;
        private static bool updateHoveredControl;
        private MouseEvents _userMouseEvent;
        private MouseEventArgs _userMouseArgs;

        internal static Control activeResizeControl;
        internal readonly List<Control> Contexts = new List<Control>();
        internal readonly FormCollection Forms = new FormCollection();
        internal readonly List<Control> HoveredControls = new List<Control>();
        internal Control hoveredControl;
        internal readonly List<Form> ModalForms = new List<Form>();

        public static bool Debug { get; set; }
        public static float DeltaTime { get { return UnityEngine.Time.deltaTime; } }
        public float FillRate { get; set; }
        public static bool IsDraging { get { return _dragndrop; } }
        public static bool IsStandalone
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
        public static float ScaleX
        {
            get { return _scaleX; }
            set
            {
                _scaleX = value;
                if (value < .1f)
                {
                    _scaleX = 1f;
                    Log("Minimum scale value should be >= .1f");
                }
            }
        }
        public static float ScaleY
        {
            get { return _scaleY; }
            set
            {
                _scaleY = value;
                if (value < .1f)
                {
                    _scaleY = 1f;
                    Log("Minimum scale value should be >= .1f");
                }
            }
        }
        public static Action<Control> ShowCallback { get; set; }
        public bool TabSwitching { get; set; }

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
        private static Control FindControlAt(Control currentControl, System.Drawing.Point position)
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

        internal static bool ControlIsVisible(Control control)
        {
            if (control.Visible == false) return false;
            if (control.Location.X + control.Offset.X + control.Width < 0) return false;
            if (control.Location.Y + control.Offset.Y + control.Height < 0) return false;
            if (control.Parent != null)
            {
                if (control.Location.X + control.Offset.X > control.Parent.Width) return false;
                if (control.Location.Y + control.Offset.Y > control.Parent.Height) return false;
            }
            return true;
        }
        private static bool _ControlWParentVisible(Control control)
        {
            if (ControlIsVisible(control) == false) return false;

            if (control.Parent == null)
                return control.Visible;
            if (control.Parent.Visible)
                return _ControlWParentVisible(control.Parent);
            return false;
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
        private Control _ControlAt(System.Drawing.Point mousePosition)
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
        private static Control _ParentContains(Control control, System.Drawing.PointF mousePosition, Control currentControl, ref bool ok)
        {
            //Application.Log(control.Name);
            if (control == null || control.Parent == null) return currentControl;

            var parentLocation = control.Parent.PointToScreen(System.Drawing.Point.Zero);
            var parentRect = new System.Drawing.RectangleF(parentLocation.X, parentLocation.Y, control.Parent.Width, control.Parent.Height);
            if (parentRect.Contains(mousePosition) == true)
                currentControl = control.Parent;
            else
                ok = false; // Control is not visible due to groups;

            return _ParentContains(control.Parent, mousePosition, currentControl, ref ok);
        }
        private static bool _ProcessControl(System.Drawing.PointF mousePosition, Control control, bool ignore_rect)
        {
            // ignore_rect will call mouse_up & mouse_move in any case.
            var c_location = control.PointToScreen(System.Drawing.Point.Zero);
            var clientRect = new System.Drawing.RectangleF(c_location.X, c_location.Y, control.Width, control.Height);
            var contains = clientRect.Contains(mousePosition);

            if (contains && (_mouseEvent == MouseEvents.Down) || _mouseEvent == MouseEvents.Up)
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
                var client_mpos = control.PointToClient(mousePosition);
                if (mousePositionChanged)
                {
                    MouseEventArgs m_args = new MouseEventArgs(MouseButtons.None, 0, (int)client_mpos.X, (int)client_mpos.Y, 0);
                    control.RaiseOnMouseMove(m_args);
                    if (_dragData != null)
                        _dragndrop = true;
                }

                if (!contains && _mouseEvent != MouseEvents.Up)
                    return true;
                switch (_mouseEvent)
                {
                    case MouseEvents.Down:
                        MouseEventArgs md_args = new MouseEventArgs(_mouseButton, 1, (int)client_mpos.X, (int)client_mpos.Y, 0);
                        control.RaiseOnMouseDown(md_args);
                        _mouseLastClickControl = control;
                        return true;
                    case MouseEvents.Up:
                        if (_dragndrop)
                        {
                            if (control.AllowDrop)
                            {
                                DataObject dnd_data = new DataObject(_dragData);
                                DragEventArgs dnd_args = new DragEventArgs(dnd_data, 0, (int)client_mpos.X, (int)client_mpos.Y, DragDropEffects.None, _dragControlEffects);
                                control.RaiseOnDragDrop(dnd_args);
                            }
                            _dragData = null;
                            _dragndrop = false;
                        }
                        MouseEventArgs mu_args = new MouseEventArgs(_mouseButton, 1, (int)client_mpos.X, (int)client_mpos.Y, 0);
                        control.RaiseOnMouseUp(mu_args);
                        if (_mouseLastClickControl == control)
                            control.RaiseOnMouseClick(mu_args);
                        if (_mouseLastClickControl != null && control != _mouseLastClickControl)
                            _mouseLastClickControl.RaiseOnMouseUp(mu_args);
                        return true;
                    case MouseEvents.DoubleClick:
                        MouseEventArgs mdc_args = new MouseEventArgs(_mouseButton, 2, (int)client_mpos.X, (int)client_mpos.Y, 0);
                        control.RaiseOnMouseDoubleClick(mdc_args);
                        return true;
                    case MouseEvents.Wheel:
                        MouseEventArgs mw_args = new MouseEventArgs(MouseButtons.Middle, 0, (int)client_mpos.X, (int)client_mpos.Y, (int)(-_mouseWheelDelta * 4));
                        control.RaiseOnMouseWheel(mw_args);
                        return true;
                }
            }
            if (!contains)
                control.RaiseOnMouseLeave(null);
            return false;
        }
        private static Keys _UKeyToKey(UnityEngine.KeyCode ukey)
        {
            Keys key = Keys.None;
            Keys mod = Keys.None; // TODO: return KeyEventArgs with this mod

            switch (ukey)
            {
                case KeyCode.Backspace: key = Keys.Back; break;
                case KeyCode.Tab: key = Keys.Tab; break;
                case KeyCode.Clear: key = Keys.Clear; break;
                case KeyCode.Return: key = Keys.Return; break;
                case KeyCode.Pause: key = Keys.Pause; break;
                case KeyCode.Escape: key = Keys.Escape; break;
                case KeyCode.Space: key = Keys.Space; break;

                case KeyCode.Exclaim: key = Keys.D1; mod = Keys.Shift; break;
                case KeyCode.DoubleQuote: key = Keys.D2; mod = Keys.Shift; break;
                case KeyCode.Hash: key = Keys.D3; mod = Keys.Shift; break;
                case KeyCode.Dollar: key = Keys.D4; mod = Keys.Shift; break;
                case KeyCode.Ampersand: key = Keys.D5; mod = Keys.Shift; break;
                case KeyCode.Quote: key = Keys.OemQuotes; break;
            }

            Application.Log(key.ToString() + " mod: " + mod.ToString());

            return key;
        }
        internal void UpdatePaintClipRect()
        {
            _paintEventArgs.ClipRectangle = new Drawing.Rectangle(0, 0, Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
        }

        public Application()
        {
            TabSwitching = true;

            _paintEventArgs = new PaintEventArgs();
            _paintEventArgs.Graphics = new Drawing.Graphics();

            Cursor.Current = Cursors.Default;
        }

        public void Draw()
        {
            // Scale if needed.
            if (ScaleX != 1f || ScaleY != 1f)
                UnityEngine.GUI.matrix = UnityEngine.Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(0, Vector3.up), new Vector3(ScaleX, ScaleY, 1));

            GUI.color = Color.white;

            _paintEventArgs.Graphics.FillRate = 0;

            for (int i = 0; i < Forms.Count; i++)
            {
                var form = Forms[i];
                if (form.Visible) form.RaiseOnPaint(_paintEventArgs);
            }

            for (int i = 0; i < ModalForms.Count; i++)
            {
                var form = ModalForms[i];
                if (form.Visible) form.RaiseOnPaint(_paintEventArgs);
            }

            for (int i = 0; i < Contexts.Count; i++)
            {
                var context = Contexts[i];
                if (context.Visible) context.RaiseOnPaint(_paintEventArgs);
            }

            FillRate = _paintEventArgs.Graphics.FillRate;

            if (_dragRender != null && _dragndrop)
            {
                var g = new System.Drawing.Graphics();
                var _dragRenderControl = new Control();
                g.Control = _dragRenderControl;
                _dragRender.Invoke(g);
                _dragRenderControl.Dispose();
            }

            // ToolTip.
            ToolTip.OnPaint(_paintEventArgs);

            var cursor = Cursor.CurrentSystem ?? Cursor.Current;
            cursor.Draw(_paintEventArgs.Graphics,
                new Drawing.Rectangle(
                    Control.MousePosition.X,
                    Control.MousePosition.Y,
                    (int)(cursor.Size.Width / ScaleX),
                    (int)(cursor.Size.Height / ScaleY)));
        }
        public void ProccessKeys()
        {
            // Close context if possible.
            if (Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyDown)
            {
                if (Contexts.Count > 0)
                {
                    Contexts[0].Dispose();
                    return;
                }
            }

            if (Event.current.keyCode != KeyCode.None)
            {
                KeyEventArgs args = new KeyEventArgs();
                args.KeyCode = Event.current.keyCode;
                args.Modifiers = Event.current.modifiers;
                if ((args.Modifiers & EventModifiers.FunctionKey) != 0)
                    args.Modifiers &= ~EventModifiers.FunctionKey;

                //_UKeyToKey(args.KeyCode); // testing...

                // HotKey for control.
                var _hotKey = _hotKeys.Find(x => x.hWnd != null && x.Key == args.KeyCode && x.Modifier == args.Modifiers);
                if (_hotKey != null)
                {
                    switch (Event.current.type)
                    {
                        case EventType.KeyDown: // not working with mods (?).
                            if (_currentKeyDown == KeyCode.None || _currentKeyDown != args.KeyCode)
                                _hotKey.hWnd.RaiseOnKeyDown(args);
                            _hotKey.hWnd.RaiseOnKeyPress(args);
                            break;
                        case EventType.KeyUp:
                            _hotKey.hWnd.RaiseOnKeyUp(args);
                            break;
                    }
                }

                // Raise keys on selected controls if possible.
                if (Control.lastSelected != null && Control.lastSelected.IsDisposed == false)
                {
                    var keyControl = Control.lastSelected;
                    var parentForm = GetParentForm(Control.lastSelected);
                    if (parentForm != null && parentForm.KeyPreview)
                        keyControl = parentForm;

                    switch (Event.current.type)
                    {
                        case EventType.KeyDown:
                            // Tab switching through controls.
                            if (TabSwitching && Event.current.keyCode == KeyCode.Tab)
                            {
                                if (Event.current.modifiers == EventModifiers.None)
                                    NextTabControl(Control.lastSelected);
                                else if (Event.current.modifiers == EventModifiers.Shift)
                                    PrevTabControl(Control.lastSelected);
                                break;
                            }

                            if (_currentKeyDown == KeyCode.None || _currentKeyDown != args.KeyCode)
                                keyControl.RaiseOnKeyDown(args);

                            keyControl.RaiseOnKeyPress(args);

                            break;
                        case EventType.KeyUp:
                            _currentKeyDown = KeyCode.None;
                            keyControl.RaiseOnKeyUp(args);
                            break;
                        case EventType.Layout:
                            break;
                    }
                }

                if (Event.current.type == EventType.keyDown)
                    _currentKeyDown = args.KeyCode;
            }
        }
        public void ProccessMouse(float mouseX, float mouseY)
        {
            if (ScaleX != 1f || ScaleY != 1f)
            {
                mouseX /= ScaleX;
                mouseY /= ScaleY;
            }

            _mouseEvent = MouseEvents.None;
            _mouseButton = MouseButtons.None;

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

            if (_userMouseArgs != null)
            {
                switch (_userMouseArgs.Button)
                {
                    case MouseButtons.Left: eventButton = 0; break;
                    case MouseButtons.Right: eventButton = 1; break;
                    case MouseButtons.Middle: eventButton = 2; break;
                }
                eventClicks = _userMouseArgs.Clicks;
                eventDelta = _userMouseArgs.Delta;
                switch (_userMouseEvent)
                {
                    case MouseEvents.Down: eventType = EventType.MouseDown; break;
                    case MouseEvents.Up: eventType = EventType.MouseUp; break;
                    case MouseEvents.Wheel: eventType = EventType.ScrollWheel; break;
                }
                _userMouseArgs = null;
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
                            _mouseButton = MouseButtons.Left;
                            _mouseEvent = MouseEvents.Down;
                            if (eventClicks > 1)
                                _mouseEvent = MouseEvents.DoubleClick;
                            break;
                        case 1:
                            _mouseButton = MouseButtons.Right;
                            _mouseEvent = MouseEvents.Down;
                            break;
                        case 2:
                            _mouseButton = MouseButtons.Middle;
                            _mouseEvent = MouseEvents.Down;
                            break;
                    }
                    break;
                case EventType.MouseUp:
                    switch (eventButton)
                    {
                        case 0:
                            _mouseButton = MouseButtons.Left;
                            _mouseEvent = MouseEvents.Up;
                            break;
                        case 1:
                            _mouseButton = MouseButtons.Right;
                            _mouseEvent = MouseEvents.Up;
                            break;
                        case 2:
                            _mouseButton = MouseButtons.Middle;
                            _mouseEvent = MouseEvents.Up;
                            break;
                    }
                    break;
                case EventType.ScrollWheel:
                    _mouseEvent = MouseEvents.Wheel;
                    _mouseWheelDelta = eventDelta;
                    break;
            }

            #endregion

            //if (_mouseLastClickControl != null && _mouseEvent == MouseEvents.None && _mouseMovePosition != mousePosition)
            //    _ProcessControl(mousePosition, _mouseLastClickControl, true);

            if (_mouseEvent == MouseEvents.None && !mousePositionChanged) return;

            // Dispose context first.
            for (int i = 0; i < Contexts.Count; i++)
            {
                if (!Contexts[i].Context) continue;

                var contextControl = Contexts[i];
                if (Contains(contextControl, hoveredControl)) continue;
                if (_mouseEvent != MouseEvents.Down) continue;

                contextControl.Dispose();
                i--;
            }

            if (hoveredControl == null && _mouseEvent == MouseEvents.Up)
            {
                _dragndrop = false;
                _dragData = null;
            }

            if (hoveredControl != null)
                _ProcessControl(new PointF(mouseX, mouseY), hoveredControl, false);

            if (_mouseEvent == MouseEvents.Down)
                DownClick(hoveredControl, new MouseEventArgs(_mouseButton, 1, (int)mouseX, (int)mouseY, 0));

            if (_mouseEvent == MouseEvents.Up)
                UpClick(hoveredControl, new MouseEventArgs(_mouseButton, 1, (int)mouseX, (int)mouseY, 0));
        }
        public void ProccessMouse(System.Drawing.PointF mousePosition)
        {
            ProccessMouse(mousePosition.X, mousePosition.Y);
        }
        public void RaiseMouseEvent(MouseEvents mEv, MouseEventArgs mArgs)
        {
            _userMouseEvent = mEv;
            _userMouseArgs = mArgs;

            ProccessMouse(new Drawing.PointF(mArgs.X, mArgs.Y));
        }
        public void Run(Control control)
        {
            control.Owner = this;
            //this.Controls.Add(control);
        }
        public void Update()
        {
            // Update hovered control.
            if (hoveredControl != null)
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

            UpdateEvent();
        }
        
        internal static void DoDragDrop(object data, DragDropEffects effect, DragDropRenderHandler render = null)
        {
            _dragData = data;
            _dragControlEffects = effect;
            _dragRender = render;
        }
        public static Form GetParentForm(Control control)
        {
            if (control == null) return null;
            var form = control.Parent as Form;
            if (form != null) return form;

            return GetParentForm(control.Parent);
        }
        public static Control GetRootControl(Control control)
        {
            if (control == null) return null;

            if (control.Parent != null)
                return GetRootControl(control.Parent);

            return control;
        }
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }
        public static void NextTabControl(Control control)
        {
            var controlForm = GetRootControl(control) as Form;
            if (controlForm == null || controlForm.Controls.Count <= 0) return;

            var formControls = new List<Control>();
            _FillListWithVisibleControls(controlForm, formControls);

            var possibleControls = formControls.FindAll(x => x.IsDisposed == false && x.CanSelect);
            if (possibleControls.Find(x => x.TabIndex > 0) != null)
            {
                possibleControls.Sort((x, y) => x.TabIndex.CompareTo(y.TabIndex));
                //possibleControls.Reverse();
            }
            if (possibleControls.Count == 0) return;

            int controlIndex = possibleControls.FindIndex(x => x == control);

            var nextControlIndex = controlIndex + 1;
            if (nextControlIndex >= possibleControls.Count)
                nextControlIndex = 0;
            possibleControls[nextControlIndex].Focus();
        }
        public static void PrevTabControl(Control control)
        {
            var controlForm = GetRootControl(control) as Form;
            if (controlForm == null || controlForm.Controls.Count <= 0) return;

            var formControls = new List<Control>();
            _FillListWithVisibleControls(controlForm, formControls);

            var possibleControls = formControls.FindAll(x => x.Visible && x.IsDisposed == false && x.CanSelect);
            if (possibleControls.Find(x => x.TabIndex > 0) != null)
            {
                possibleControls.Sort((x, y) => x.TabIndex.CompareTo(y.TabIndex));
                //possibleControls.Reverse();
            }
            if (possibleControls.Count == 0) return;

            int controlIndex = possibleControls.FindIndex(x => x == control);

            var nextControlIndex = controlIndex - 1;
            if (nextControlIndex < 0)
                nextControlIndex = possibleControls.Count - 1;
            possibleControls[nextControlIndex].Focus();
        }
        public static bool RegisterHotKey(Control hWnd, int id, UnityEngine.EventModifiers modifier, UnityEngine.KeyCode key)
        {
            if (hWnd == null || hWnd.Disposing || hWnd.IsDisposed) return false;
            if (_hotKeys.Find(x => x.hWnd == hWnd && x.Id == id) != null)
                return false;

            _hotKeys.Add(new _HotKey(hWnd, id, modifier, key));

            return true;
        }
        public static bool UnregisterHotKey(Control hWnd, int id)
        {
            if (hWnd == null || hWnd.Disposing || hWnd.IsDisposed) return false;
            var _hotKeyIndex = _hotKeys.FindIndex(x => x.hWnd == hWnd && x.Id == id);
            if (_hotKeyIndex == -1) return false;

            _hotKeys.RemoveAt(_hotKeyIndex);
            return true;
        }

        public delegate void UpdateEventDelegate();

        internal event MouseEventHandler DownClick = delegate { };
        internal event MouseEventHandler UpClick = delegate { };
        public event UpdateEventDelegate UpdateEvent = delegate { };

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
            Up,
        }

        private class _HotKey
        {
            private Control _hWnd;
            private int _id;
            private UnityEngine.EventModifiers _modifier;
            private UnityEngine.KeyCode _key;

            public Control hWnd
            {
                get { return _hWnd; }
                private set { _hWnd = value; }
            }
            public int Id
            {
                get { return _id; }
                private set { _id = value; }
            }
            public UnityEngine.EventModifiers Modifier
            {
                get { return _modifier; }
                private set { _modifier = value; }
            }
            public UnityEngine.KeyCode Key
            {
                get { return _key; }
                private set { _key = value; }
            }

            public _HotKey(Control hwnd, int id, UnityEngine.EventModifiers modifier, UnityEngine.KeyCode key)
            {
                this.hWnd = hwnd;
                this.Id = id;
                this.Modifier = modifier;
                this.Key = key;
            }
        }
    }
}
