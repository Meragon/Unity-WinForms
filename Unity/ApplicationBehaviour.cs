using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Color = UnityEngine.Color;
using UE = UnityEngine;

namespace Unity.API
{
    public sealed class ApplicationBehaviour : UE.MonoBehaviour
    {
        private static UE.Texture2D _defaultSprite;

        public static UE.Texture2D DefaultSprite
        {
            get
            {
                if (_defaultSprite != null) return _defaultSprite;

                _defaultSprite = new UE.Texture2D(32, 32);
                for (int i = 0; i < _defaultSprite.height; i++)
                    for (int k = 0; k < _defaultSprite.width; k++)
                        _defaultSprite.SetPixel(k, i, Color.white);
                _defaultSprite.Apply();
                return _defaultSprite;
            }
        }
        public static AppGdiImages GdiImages { get; private set; }
        public static AppResources gResources { get; private set; }

        public AppResources _Resources;
        public static bool ShowControlProperties { get; set; }

        private static readonly List<invokeAction> actions = new List<invokeAction>();
        private Application _controller;
        private float _lastWidth;
        private float _lastHeight;
        private bool _paused;

        private void Awake()
        {
            gResources = _Resources;

            Graphics.ApiGraphics = ApiHolder.Graphics;

            // Enum + dictionary?
            GdiImages = new AppGdiImages();
            GdiImages.ArrowDown = gResources.Images.ArrowDown.ToBitmap();
            GdiImages.ArrowLeft = gResources.Images.ArrowLeft.ToBitmap();
            GdiImages.ArrowRight = gResources.Images.ArrowRight.ToBitmap();
            GdiImages.ArrowUp = gResources.Images.ArrowUp.ToBitmap();
            GdiImages.Circle = gResources.Images.Circle.ToBitmap();
            GdiImages.Checked = gResources.Images.Checked.ToBitmap();
            GdiImages.Close = gResources.Images.Close.ToBitmap();
            GdiImages.CurvedArrowDown = gResources.Images.CurvedArrowDown.ToBitmap();
            GdiImages.CurvedArrowLeft = gResources.Images.CurvedArrowLeft.ToBitmap();
            GdiImages.CurvedArrowRight = gResources.Images.CurvedArrowRight.ToBitmap();
            GdiImages.CurvedArrowUp = gResources.Images.CurvedArrowUp.ToBitmap();
            GdiImages.DropDownRightArrow = gResources.Images.DropDownRightArrow.ToBitmap();
            GdiImages.FormResize = gResources.Images.FormResize.ToBitmap();
            GdiImages.NumericDown = gResources.Images.NumericDown.ToBitmap();
            GdiImages.NumericUp = gResources.Images.NumericUp.ToBitmap();
            GdiImages.TreeNodeCollapsed = gResources.Images.TreeNodeCollapsed.ToBitmap();
            GdiImages.TreeNodeExpanded = gResources.Images.TreeNodeExpanded.ToBitmap();

            GdiImages.Cursors.Default = gResources.Images.Cursors.Default.ToBitmap();
            GdiImages.Cursors.HSplit = gResources.Images.Cursors.HSplit.ToBitmap();
            GdiImages.Cursors.Hand = gResources.Images.Cursors.Hand.ToBitmap();
            GdiImages.Cursors.Help = gResources.Images.Cursors.Help.ToBitmap();
            GdiImages.Cursors.SizeAll = gResources.Images.Cursors.SizeAll.ToBitmap();
            GdiImages.Cursors.SizeNESW = gResources.Images.Cursors.SizeNESW.ToBitmap();
            GdiImages.Cursors.SizeNS = gResources.Images.Cursors.SizeNS.ToBitmap();
            GdiImages.Cursors.SizeNWSE = gResources.Images.Cursors.SizeNWSE.ToBitmap();
            GdiImages.Cursors.SizeWE = gResources.Images.Cursors.SizeWE.ToBitmap();
            GdiImages.Cursors.VSplit = gResources.Images.Cursors.VSplit.ToBitmap();

            _lastWidth = UE.Screen.width;
            _lastHeight = UE.Screen.height;

            _controller = new Application();
            _controller.Resources = GdiImages;
            _controller.UpdatePaintClipRect();

            Control.uwfDefaultController = _controller;
        }
        private void Update()
        {
            if (_controller != null)
            {
                if (_lastWidth != UE.Screen.width || _lastHeight != UE.Screen.height)
                {
                    Size deltaSize = new Size(
                        (int)(_lastWidth - UE.Screen.width),
                        (int)(_lastHeight - UE.Screen.height));
                    for (int i = 0; i < _controller.ModalForms.Count; i++)
                        _controller.ModalForms[i].uwfAddjustSizeToScreen(deltaSize);
                    for (int i = 0; i < _controller.Forms.Count; i++)
                        _controller.Forms[i].uwfAddjustSizeToScreen(deltaSize);
                    _controller.UpdatePaintClipRect();
                }
                _lastWidth = UE.Screen.width;
                _lastHeight = UE.Screen.height;

                _controller.Update();
            }

            for (int i = 0; i < actions.Count; i++)
            {
                var a = actions[i];
                a.Seconds -= UE.Time.deltaTime;
                if (a.Seconds > 0) continue;

                a.Action();
                actions.RemoveAt(i);
                i--;
            }
        }
        private void OnApplicationFocus(bool focusStatus)
        {
            _paused = !focusStatus;

            UE.Cursor.visible = Cursor.IsVisible;
        }
        private void OnGUI()
        {
            if (_controller == null) return;

            if (_paused == false)
            {
                // Mouse.
                _controller.ProccessMouse(UE.Input.mousePosition.x, UE.Screen.height - UE.Input.mousePosition.y);

                // Keys.
                if (UE.Event.current.keyCode != UE.KeyCode.None)
                {
                    var keyData = UnityKeyTranslator.ToKeyData(UE.Event.current.modifiers, UE.Event.current.keyCode);
                    var keyArgs = new KeyEventArgs(keyData);
                    keyArgs.uwfKeyCode = UE.Event.current.keyCode;
                    keyArgs.uwfModifiers = UE.Event.current.modifiers;
                    if ((keyArgs.uwfModifiers & UE.EventModifiers.FunctionKey) != 0)
                        keyArgs.uwfModifiers &= ~UE.EventModifiers.FunctionKey;

                    var keyEventType = (Application.KeyEvents)(UE.Event.current.type - 3);
                    if (keyEventType == Application.KeyEvents.Down || keyEventType == Application.KeyEvents.Up)
                        _controller.ProccessKeys(keyArgs, keyEventType);
                }
            }

            _controller.Redraw();
        }

        internal static invokeAction Invoke(Action a, float seconds)
        {
            if (a == null) return null;

            var ia = new invokeAction()
            {
                Action = a,
                Seconds = seconds
            };

            actions.Add(ia);
            return ia;
        }

        internal class invokeAction
        {
            public Action Action { get; set; }
            public float Seconds { get; set; }
        }
    }
}