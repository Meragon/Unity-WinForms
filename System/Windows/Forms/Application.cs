using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    public sealed class Application : MonoBehaviour
    {
        private static Application _instance;
        public static Application Instance
        {
            get { return _instance; }
        }

        public static bool Debug { get; set; }
        internal static List<Control> Controls = new List<Control>();
        internal static List<Control> BringToFrontControls = new List<Control>();
        internal static List<Control> ToCloseControls = new List<Control>();

        internal static Texture2D DefaultSprite { get; private set; }

        // Editor settings.
        public GUISkin Skin;
        public AppResources _Resources;
        public bool ShowControlProperties;

        public static bool IsDraging { get { return _dragndrop; } }
        public static AppResources Resources { get; set; }
        public static Action<Control> ShowCallback { get; set; }

        private static KeyCode _currentKeyDown = KeyCode.None;
        private static List<_HotKey> _hotKeys = new List<_HotKey>();
        private bool _keyModeLeftControl;
        private bool _keyModeLeftShift;
        private MouseEvents _mouseEvent = 0;
        private MouseButtons _mouseButton = 0;
        private Control _mouseLastClickControl;
        private float _mouseWheelDelta;
        private Vector3 _mouseMovePosition;
        private bool _mouseFoundHoveredControl;
        private bool _waitForDoubleClick = false;
        private float _waitForDoubleClickTimer = .2f;
        private float _waitForDoubleClickTimerCurrent = 0;

        private static bool _dragndrop;
        private static object _dragData;
        private static DragDropEffects _dragControlEffects;
        private static DragDropRenderHandler _dragRender;

        private float _lastWidth;
        private float _lastHeight;


        private void Awake()
        {
            if (_instance != null)
            {
                UnityEngine.Debug.LogError("Too many instances.");
                return;
            }
            _instance = this;

            DefaultSprite = new Texture2D(32, 32);
            for (int i = 0; i < DefaultSprite.height; i++)
                for (int k = 0; k < DefaultSprite.width; k++)
                    DefaultSprite.SetPixel(k, i, Color.white);
            DefaultSprite.Apply();

            Resources = _Resources;

            _mouseMovePosition = Input.mousePosition;
            _lastWidth = UnityEngine.Screen.width;
            _lastHeight = UnityEngine.Screen.height;
        }
        private void Update()
        {
            #region Mouse
            _mouseEvent = MouseEvents.None;
            _mouseButton = MouseButtons.None;
            _mouseFoundHoveredControl = false;

            System.Drawing.PointF mousePosition = new System.Drawing.PointF(Input.mousePosition.x, UnityEngine.Screen.height - Input.mousePosition.y);
            //mousePosition = new System.Drawing.PointF(mousePosition.X * ScaleWidth, mousePosition.Y * ScaleHeight);

            if (_waitForDoubleClick)
            {
                if (_waitForDoubleClickTimerCurrent > _waitForDoubleClickTimer)
                {
                    _waitForDoubleClickTimerCurrent = 0;
                    _waitForDoubleClick = false;
                }
                else
                    _waitForDoubleClickTimerCurrent += Time.deltaTime;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _mouseButton = MouseButtons.Left;
                _mouseEvent = MouseEvents.Down;

                if (_waitForDoubleClick)
                {
                    _mouseEvent = MouseEvents.DoubleClick;
                    _mouseButton = MouseButtons.Left;
                    _waitForDoubleClick = false;
                    _waitForDoubleClickTimerCurrent = 0;
                }
                else
                    _waitForDoubleClick = true;
            }
            if (Input.GetMouseButtonDown(1))
            {
                _mouseButton = MouseButtons.Right;
                _mouseEvent = MouseEvents.Down;
            }
            if (Input.GetMouseButtonDown(2))
            {
                _mouseButton = MouseButtons.Middle;
                _mouseEvent = MouseEvents.Down;
            }
            if (Input.GetMouseButtonUp(0))
            {
                _mouseButton = MouseButtons.Left;
                _mouseEvent = MouseEvents.Up;
                //if (_mouseLastClickControl != null)
                {
                    //_ProcessControl(mousePosition, _mouseLastClickControl, true);
                    //_mouseLastClickControl = null;
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                _mouseButton = MouseButtons.Right;
                _mouseEvent = MouseEvents.Up;
                //_mouseLastClickControl = null;
            }
            if (Input.GetMouseButtonUp(2))
            {
                _mouseButton = MouseButtons.Middle;
                _mouseEvent = MouseEvents.Up;
                //_mouseLastClickControl = null;
            }
            if ((_mouseWheelDelta = Input.GetAxis("Mouse ScrollWheel")) != 0)
            {
                _mouseEvent = MouseEvents.Wheel;
            }

            if (_mouseLastClickControl != null && _mouseEvent == MouseEvents.None && _mouseMovePosition != Input.mousePosition)
            {
                _ProcessControl(mousePosition, _mouseLastClickControl, true);
                //if (mouseEvent == MouseEvents.Up)
                //    mouseLastClickControl = null;
            }

            if (_mouseEvent != MouseEvents.None || _mouseMovePosition != Input.mousePosition)
            {
                // Dispose context first.
                if (_mouseEvent == MouseEvents.Down)
                {
                    for (int i = 0; i < Controls.Count; i++)
                    {
                        if (Controls[i].Context)
                        {
                            var c_location = Controls[i].PointToScreen(System.Drawing.Point.Zero);
                            //var client_mpos = Controls[i].PointToClient(mousePosition);

                            var clientRect = new System.Drawing.Rectangle(c_location.X, c_location.Y, Controls[i].Width, Controls[i].Height);
                            var contains = clientRect.Contains(mousePosition);
                            //UnityEngine.Debug.Log(Controls[i].ToString() + " " + contains.ToString());
                            if (!contains)
                            {
                                /*bool dispose = true;
                                if (Controls[i] is ToolStrip)
                                {
                                    foreach (var tItem in (Controls[i] as ToolStrip).Items)
                                    {
                                        if (tItem.OwnerItem != null && tItem.OwnerItem.Owner != null)
                                        {
                                            dispose = false;
                                            break;
                                        }
                                    }
                                }
                                if (dispose)*/
                                Controls[i].Dispose();
                                continue;
                            }
                        }
                    }
                }

                bool alwaysTop = true;
                bool found = false;
                Control lastPreccessedControl = null;
                for (int i = Controls.Count - 1; i >= -1; i--)
                {
                    // Top first.
                    if (i == -1)
                    {
                        if (alwaysTop)
                        {
                            i = Controls.Count;
                            alwaysTop = false;
                            continue;
                        }
                        break;
                    }

                    if (alwaysTop && !Controls[i].TopMost) continue;
                    if (_ControlVisible(Controls[i]) == false || Controls[i].Enabled == false) continue;

                    if (_ProcessControl(mousePosition, Controls[i], false))
                    {
                        lastPreccessedControl = Controls[i];
                        found = true;
                        break;
                    }
                }
                if (!found && _mouseEvent == MouseEvents.Up)
                {
                    _dragndrop = false;
                    _dragData = null;
                }

                if (_mouseEvent == MouseEvents.Down)
                    DownClick(lastPreccessedControl, new MouseEventArgs(_mouseButton, 1, (int)mousePosition.X, (int)mousePosition.Y, 0));
                if (_mouseEvent == MouseEvents.Up)
                    UpClick(lastPreccessedControl, new MouseEventArgs(_mouseButton, 1, (int)mousePosition.X, (int)mousePosition.Y, 0));
            }

            _mouseMovePosition = Input.mousePosition;

            #endregion

            if (_lastWidth != UnityEngine.Screen.width || _lastHeight != UnityEngine.Screen.height)
            {
                System.Drawing.Size deltaSize = new System.Drawing.Size(
                    (int)(_lastWidth - UnityEngine.Screen.width),
                    (int)(_lastHeight - UnityEngine.Screen.height));
                for (int i = 0; i < Controls.Count; i++)
                    if (Controls[i].Parent == null)
                        Controls[i].AddjustSizeToScreen(deltaSize);
                //Controls[i].Size -= deltaSize;
                //Controls[i].CallOnResize(new System.Drawing.Point((int)(_lastWidth - UnityEngine.Screen.width), (int)(_lastHeight - UnityEngine.Screen.height)));
            }
            _lastWidth = UnityEngine.Screen.width;
            _lastHeight = UnityEngine.Screen.height;

            UpdateEvent();
        }
        private void LateUpdate()
        {
            if (BringToFrontControls.Count > 0)
            {
                for (; BringToFrontControls.Count > 0; )
                {
                    int c_index = Controls.FindIndex(x => x == BringToFrontControls[0]);
                    if (c_index > -1)
                    {
                        Controls.RemoveAt(c_index);
                        Controls.Add(BringToFrontControls[0]);

                        if (BringToFrontControls[0].Controls != null)
                            for (int i = 0; i < BringToFrontControls[0].Controls.Count; i++)
                                BringToFrontControls.Add(BringToFrontControls[0].Controls[i]);
                    }
                    BringToFrontControls.RemoveAt(0);
                }
            }
            if (ToCloseControls.Count > 0)
            {
                for (; ToCloseControls.Count > 0; )
                {
                    int c_index = Controls.FindIndex(x => x == ToCloseControls[0]);
                    if (c_index > -1)
                        Controls.RemoveAt(c_index);
                    else
                    {
#if UNITY_EDITOR
                        UnityEngine.Debug.LogError("Not found. Da hell. " + ToCloseControls[0].GetType().ToString());
#endif
                    }

                    ToCloseControls.RemoveAt(0);
                }
            }
        }
        private void OnGUI()
        {
            if (Event.current.keyCode != KeyCode.None)
            {
                KeyEventArgs args = new KeyEventArgs();
                args.KeyCode = Event.current.keyCode;
                args.Modifiers = Event.current.modifiers;
                if ((args.Modifiers & EventModifiers.FunctionKey) != 0)
                    args.Modifiers &= ~EventModifiers.FunctionKey;

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

                foreach (var c in new List<Control>(Controls))
                    if (c.Visible && (c.Focused))
                    {
                        switch (Event.current.type)
                        {
                            case EventType.KeyDown:
                                if (_currentKeyDown == KeyCode.None || _currentKeyDown != args.KeyCode)
                                    c.RaiseOnKeyDown(args);

                                c.RaiseOnKeyPress(args);
                                break;
                            case EventType.KeyUp:
                                _currentKeyDown = KeyCode.None;
                                c.RaiseOnKeyUp(args);
                                break;
                            case EventType.Layout:

                                break;
                        }
                    }

                if (Event.current.type == EventType.keyDown)
                    _currentKeyDown = args.KeyCode;
            }

            GUI.skin = Skin;
            Draw(Controls);
        } // OnPostRender is faster. Should think about it.

        private static void Draw(IList<Control> Controls)
        {

            GUI.color = Color.white;

            PaintEventArgs args = new PaintEventArgs();
            args.Graphics = new System.Drawing.Graphics();

            for (int i = 0; i < Controls.Count; i++)
            {
                if (!_ControlVisible(Controls[i])) continue;
                if (Controls[i].Parent != null || Controls[i].TopMost || !Controls[i].Visible)
                    continue;
                bool simpleCulling = false;

                var currentAbspos = Controls[i].PointToScreen(System.Drawing.Point.Zero);
                // Offscreen culling.
                /*if (currentAbspos.X + Controls[i].Width < 0 || currentAbspos.X > UnityEngine.Screen.width ||
                    currentAbspos.Y + Controls[i].Height < 0 || currentAbspos.Y > UnityEngine.Screen.height)
                    continue;*/

                System.Drawing.Rectangle currentRect = new System.Drawing.Rectangle();
                currentRect.X = currentAbspos.X;
                currentRect.Y = currentAbspos.Y;
                currentRect.Width = Controls[i].Width;
                currentRect.Height = Controls[i].Height;
                for (int k = Controls.Count - 1; k > i; k--)
                {
                    if (Controls[k].BackColor.A < 255) continue;
                    var prevAbsPos = Controls[k].PointToScreen(System.Drawing.Point.Zero);
                    System.Drawing.Rectangle prevRect = new System.Drawing.Rectangle();
                    prevRect.X = prevAbsPos.X;
                    prevRect.Y = prevAbsPos.Y;
                    prevRect.Width = Controls[k].Width;
                    prevRect.Height = Controls[k].Height;
                    if (prevRect.Contains(currentRect))
                    {
                        simpleCulling = true;
                        break;
                    }
                }
                if (!simpleCulling)
                    Controls[i].RaiseOnPaint(args);
            }
            // Top.
            for (int i = 0; i < Controls.Count; i++)
            {
                if (!Controls[i].Visible) continue;
                if (Controls[i].Parent == null && Controls[i].TopMost)
                {
                    /*var currentAbspos = Controls[i].PointToScreen(System.Drawing.Point.Zero);
                    if (currentAbspos.X + Controls[i].Width < 0 || currentAbspos.X > UnityEngine.Screen.width ||
                    currentAbspos.Y + Controls[i].Height < 0 || currentAbspos.Y > UnityEngine.Screen.height)
                        continue;*/
                    Controls[i].RaiseOnPaint(args);
                }
            }

            if (_dragRender != null && _dragndrop)
            {
                System.Drawing.Graphics g = new System.Drawing.Graphics();
                Control _dragRenderControl = new Control();
                g.Control = _dragRenderControl;
                _dragRender.Invoke(g);
                _dragRenderControl.Dispose();
            }

            // ToolTip.
            ToolTip.OnPaint(args);
        }
        public static void Run(Form form)
        {

        }
        internal static void DoDragDrop(object data, DragDropEffects effect)
        {
            _dragData = data;
            _dragControlEffects = effect;
        }
        internal static void DoDragDrop(object data, DragDropEffects effect, DragDropRenderHandler render)
        {
            _dragData = data;
            _dragControlEffects = effect;
            _dragRender = render;
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

        private static bool _ControlVisible(Control control)
        {
            if (control.Visible == false) return false;
            if (control.Parent == null)
                return control.Visible;
            if (control.Parent.Visible)
                return _ControlVisible(control.Parent);
            return false;
        }
        private bool _IsPointInPolygon(List<System.Drawing.Point> polygon, System.Drawing.Point testPoint)
        {
            bool result = false;
            int j = polygon.Count - 1;
            for (int i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
        private bool _ProcessControl(System.Drawing.PointF mousePosition, Control control, bool ignore_rect)
        {
            // ignore_rect will call mouse_up & mouse_move in any case.
            var c_location = control.PointToScreen(System.Drawing.Point.Zero);
            var client_mpos = control.PointToClient(mousePosition);

            var clientRect = new System.Drawing.RectangleF(c_location.X, c_location.Y, control.Width, control.Height);
            var contains = clientRect.Contains(mousePosition);
            /*if (control is ToolStrip && !contains && mouseEvent != MouseEvents.None)
            {
                if (control.Parent == null)
                    control.Dispose();
                return false;
            }*/
            if (ignore_rect || contains)
            {
                if (contains)
                {
                    if (!_mouseFoundHoveredControl)
                    {
                        control.RaiseOnMouseHover(null);
                        control.RaiseOnMouseEnter(null);

                        if (control.Context)
                            _mouseFoundHoveredControl = true;
                    }
                }

                if (_mouseMovePosition != Input.mousePosition)
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
                        if (_mouseLastClickControl != null && control != _mouseLastClickControl)
                            _mouseLastClickControl.RaiseOnMouseUp(mu_args);
                        return true;
                    case MouseEvents.DoubleClick:
                        MouseEventArgs mdc_args = new MouseEventArgs(_mouseButton, 2, (int)client_mpos.X, (int)client_mpos.Y, 0);
                        control.RaiseOnMouseDoubleClick(mdc_args);
                        return true;
                    case MouseEvents.Wheel:
                        MouseEventArgs mw_args = new MouseEventArgs(MouseButtons.Middle, 0, (int)client_mpos.X, (int)client_mpos.Y, (int)(_mouseWheelDelta * 10));
                        control.RaiseOnMouseWheel(mw_args);
                        return true;
                }
            }
            if (!contains)
                control.RaiseOnMouseLeave(null);
            return false;
        }

        public delegate void UpdateEventDelegate();

        internal static event MouseEventHandler DownClick = delegate { };
        internal static event MouseEventHandler UpClick = delegate { };
        public static event UpdateEventDelegate UpdateEvent = delegate { };

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