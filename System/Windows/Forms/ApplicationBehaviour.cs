using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Unity.API;
using Color = UnityEngine.Color;

namespace System.Windows.Forms
{
    public sealed class ApplicationBehaviour : MonoBehaviour
    {
        private static Texture2D _defaultSprite;

        public static Texture2D DefaultSprite
        {
            get
            {
                if (_defaultSprite != null) return _defaultSprite;

                _defaultSprite = new Texture2D(32, 32);
                for (int i = 0; i < _defaultSprite.height; i++)
                    for (int k = 0; k < _defaultSprite.width; k++)
                        _defaultSprite.SetPixel(k, i, Color.white);
                _defaultSprite.Apply();
                return _defaultSprite;
            }
        }
        public static AppGdiImages GdiImages { get; private set; }
        public static AppResources Resources { get; private set; }

        public AppResources _Resources;
        public static bool ShowControlProperties { get; set; }

        private readonly List<invokeAction> actions = new List<invokeAction>();
        private Application _controller;
        private float _lastWidth;
        private float _lastHeight;
        private bool _paused;

        private void Awake()
        {
            Resources = _Resources;

            System.Drawing.Graphics.ApiGraphics = ApiHolder.Graphics;

            // Enum + dictionary?
            GdiImages = new AppGdiImages();
            GdiImages.ArrowDown = Resources.Images.ArrowDown.ToBitmap();
            GdiImages.ArrowLeft = Resources.Images.ArrowLeft.ToBitmap();
            GdiImages.ArrowRight = Resources.Images.ArrowRight.ToBitmap();
            GdiImages.ArrowUp = Resources.Images.ArrowUp.ToBitmap();
            GdiImages.Circle = Resources.Images.Circle.ToBitmap();
            GdiImages.Checked = Resources.Images.Checked.ToBitmap();
            GdiImages.Close = Resources.Images.Close.ToBitmap();
            GdiImages.CurvedArrowDown = Resources.Images.CurvedArrowDown.ToBitmap();
            GdiImages.CurvedArrowLeft = Resources.Images.CurvedArrowLeft.ToBitmap();
            GdiImages.CurvedArrowRight = Resources.Images.CurvedArrowRight.ToBitmap();
            GdiImages.CurvedArrowUp = Resources.Images.CurvedArrowUp.ToBitmap();
            GdiImages.DropDownRightArrow = Resources.Images.DropDownRightArrow.ToBitmap();
            GdiImages.FormResize = Resources.Images.FormResize.ToBitmap();
            GdiImages.NumericDown = Resources.Images.NumericDown.ToBitmap();
            GdiImages.NumericUp = Resources.Images.NumericUp.ToBitmap();
            GdiImages.TreeNodeCollapsed = Resources.Images.TreeNodeCollapsed.ToBitmap();
            GdiImages.TreeNodeExpanded = Resources.Images.TreeNodeExpanded.ToBitmap();

            GdiImages.Cursors.Default = Resources.Images.Cursors.Default.ToBitmap();
            GdiImages.Cursors.HSplit = Resources.Images.Cursors.HSplit.ToBitmap();
            GdiImages.Cursors.Hand = Resources.Images.Cursors.Hand.ToBitmap();
            GdiImages.Cursors.Help = Resources.Images.Cursors.Help.ToBitmap();
            GdiImages.Cursors.SizeAll = Resources.Images.Cursors.SizeAll.ToBitmap();
            GdiImages.Cursors.SizeNESW = Resources.Images.Cursors.SizeNESW.ToBitmap();
            GdiImages.Cursors.SizeNS = Resources.Images.Cursors.SizeNS.ToBitmap();
            GdiImages.Cursors.SizeNWSE = Resources.Images.Cursors.SizeNWSE.ToBitmap();
            GdiImages.Cursors.SizeWE = Resources.Images.Cursors.SizeWE.ToBitmap();
            GdiImages.Cursors.VSplit = Resources.Images.Cursors.VSplit.ToBitmap();

            _lastWidth = UnityEngine.Screen.width;
            _lastHeight = UnityEngine.Screen.height;

            _controller = new Application(this);
            _controller.UpdatePaintClipRect();

            Control.DefaultController = _controller;
        }
        private void Update()
        {
            if (_lastWidth != UnityEngine.Screen.width || _lastHeight != UnityEngine.Screen.height)
            {
                System.Drawing.Size deltaSize = new System.Drawing.Size(
                    (int)(_lastWidth - UnityEngine.Screen.width),
                    (int)(_lastHeight - UnityEngine.Screen.height));
                for (int i = 0; i < _controller.ModalForms.Count; i++)
                    _controller.ModalForms[i].uwfAddjustSizeToScreen(deltaSize);
                for (int i = 0; i < _controller.Forms.Count; i++)
                    _controller.Forms[i].uwfAddjustSizeToScreen(deltaSize);
                _controller.UpdatePaintClipRect();
            }
            _lastWidth = UnityEngine.Screen.width;
            _lastHeight = UnityEngine.Screen.height;

            if (_controller != null)
                _controller.Update();

            for (int i = 0; i < actions.Count; i++)
            {
                var a = actions[i];
                a.Seconds -= Time.deltaTime;
                if (a.Seconds > 0) continue;

                a.Action();
                actions.RemoveAt(i);
                i--;
            }
        }
        private void OnApplicationFocus(bool focusStatus)
        {
            _paused = !focusStatus;

            UnityEngine.Cursor.visible = Cursor.IsVisible;
        }
        private void OnGUI()
        {
            if (_controller == null) return;

            if (_paused == false)
            {
                _controller.ProccessMouse(Input.mousePosition.x, UnityEngine.Screen.height - Input.mousePosition.y);
                _controller.ProccessKeys();
            }

            _controller.Redraw();
        }

        internal invokeAction Invoke(Action a, float seconds)
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