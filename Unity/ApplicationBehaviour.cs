namespace Unity.API
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Color = UnityEngine.Color;
    using UE = UnityEngine;

    public sealed class ApplicationBehaviour : UE.MonoBehaviour
    {
        public AppResources Resources;

        private static readonly List<invokeAction> actions = new List<invokeAction>();
        private static UE.Texture2D defaultSprite;

        private Application controller;
        private float lastWidth;
        private float lastHeight;
        private bool paused;

        public static UE.Texture2D DefaultSprite
        {
            get
            {
                if (defaultSprite != null) return defaultSprite;

                defaultSprite = new UE.Texture2D(32, 32);
                for (int i = 0; i < defaultSprite.height; i++)
                    for (int k = 0; k < defaultSprite.width; k++)
                        defaultSprite.SetPixel(k, i, Color.white);
                defaultSprite.Apply();
                return defaultSprite;
            }
        }
        public static AppGdiImages GdiImages { get; private set; }
        public static AppResources gResources { get; private set; }

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

        private void Awake()
        {
            gResources = this.Resources;

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

            this.lastWidth = UE.Screen.width;
            this.lastHeight = UE.Screen.height;

            this.controller = new Application();
            this.controller.Resources = GdiImages;
            this.controller.UpdatePaintClipRect();

            Control.uwfDefaultController = this.controller;

            MouseHook.MouseUp += (sender, args) =>
            {
                if (Views.AppControl.Self != null)
                    Views.AppControl.Self.Control = sender as Control;
            };
        }
        private void Update()
        {
            if (this.controller != null)
            {
                if (this.lastWidth != UE.Screen.width || this.lastHeight != UE.Screen.height)
                {
                    Size deltaSize = new Size(
                        (int)(this.lastWidth - UE.Screen.width),
                        (int)(this.lastHeight - UE.Screen.height));
                    for (int i = 0; i < this.controller.ModalForms.Count; i++)
                        this.controller.ModalForms[i].uwfAddjustSizeToScreen(deltaSize);
                    for (int i = 0; i < this.controller.Forms.Count; i++)
                        this.controller.Forms[i].uwfAddjustSizeToScreen(deltaSize);
                    this.controller.UpdatePaintClipRect();
                }
                this.lastWidth = UE.Screen.width;
                this.lastHeight = UE.Screen.height;

                this.controller.Update();
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
            this.paused = !focusStatus;

            UE.Cursor.visible = Cursor.IsVisible;
        }
        private void OnGUI()
        {
            if (this.controller == null) return;

            if (this.paused == false)
            {
                // Mouse.
                this.controller.ProccessMouse(UE.Input.mousePosition.x, UE.Screen.height - UE.Input.mousePosition.y);

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
                        this.controller.ProccessKeys(keyArgs, keyEventType);
                }
            }

            this.controller.Redraw();
        }

        internal class invokeAction
        {
            public Action Action { get; set; }
            public float Seconds { get; set; }
        }
    }
}