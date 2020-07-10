namespace Unity.API
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    
    using UE = UnityEngine;
    using SWF = System.Windows.Forms;

    public sealed class UnityWinForms : UE.MonoBehaviour
    {
        public AppResources Resources;

        [UE.Tooltip("Delay between first KeyDown event and following ones")]
        public float ShiftDownDelayTime = .8f;
        
        [UE.Tooltip("Delay between KeyDown events")]
        public float ShiftDownTime = .05f;
        
        private static UE.Texture2D defaultTexture;
        private static UE.Texture2D transparentTexture;

        private Application controller;

        private float lastWidth;
        private float lastHeight;
        private bool  shiftPressed;
        private float shiftDownTimer;
        private float shiftDownDelayTimer; // Shift pressing is really fast.
        private bool  paused;
        
        internal static UE.Texture2D DefaultTexture
        {
            get { return defaultTexture; }
        }

        internal static UE.Texture2D TransparentTexture
        {
            get { return transparentTexture; }
        }

        internal static AppGdiImages GdiImages { get; private set; }
        internal static AppResources gResources { get; private set; }

        internal static void Inspect(object obj)
        {
            if (obj == null)
                return;
#if UNITY_EDITOR
            Views.ControlInspector.DesignerObject = obj;
#endif
        }

        private void Awake()
        {
            defaultTexture = new UE.Texture2D(1, 1);
            defaultTexture.SetPixel(0, 0, UE.Color.white);
            defaultTexture.Apply();
            
            transparentTexture = new UE.Texture2D(1, 1);
            transparentTexture.SetPixel(0, 0, UE.Color.clear);
            transparentTexture.Apply();
            
            ApiHolder.Graphics = new UnityGdi();
            ApiHolder.Input = new UnityInput();
            ApiHolder.System = new UnitySystem();
            ApiHolder.Timing = new UnityTiming();

            gResources = Resources;

            Screen.width = (int) (UE.Screen.width / Application.ScaleX);
            Screen.height = (int) (UE.Screen.height / Application.ScaleY);

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
            GdiImages.DateTimePicker = gResources.Images.DateTimePicker.ToBitmap();
            GdiImages.DropDownRightArrow = gResources.Images.DropDownRightArrow.ToBitmap();
            GdiImages.FileDialogBack = gResources.Images.FileDialogBack.ToBitmap();
            GdiImages.FileDialogFile = gResources.Images.FileDialogFile.ToBitmap();
            GdiImages.FileDialogFolder = gResources.Images.FileDialogFolder.ToBitmap();
            GdiImages.FileDialogRefresh = gResources.Images.FileDialogRefresh.ToBitmap();
            GdiImages.FileDialogUp = gResources.Images.FileDialogUp.ToBitmap();
            GdiImages.FormResize = gResources.Images.FormResize.ToBitmap();
            GdiImages.NumericDown = gResources.Images.NumericDown.ToBitmap();
            GdiImages.NumericUp = gResources.Images.NumericUp.ToBitmap();
            GdiImages.RadioButton_Checked = gResources.Images.RadioButton_Checked.ToBitmap();
            GdiImages.RadioButton_Hovered = gResources.Images.RadioButton_Hovered.ToBitmap();
            GdiImages.RadioButton_Unchecked = gResources.Images.RadioButton_Unchecked.ToBitmap();
            GdiImages.TreeNodeCollapsed = gResources.Images.TreeNodeCollapsed.ToBitmap();
            GdiImages.TreeNodeExpanded = gResources.Images.TreeNodeExpanded.ToBitmap();

            GdiImages.Cursors.Default = gResources.Images.Cursors.Default.ToBitmap();
            GdiImages.Cursors.Hand = gResources.Images.Cursors.Hand.ToBitmap();
            GdiImages.Cursors.Help = gResources.Images.Cursors.Help.ToBitmap();
            GdiImages.Cursors.HSplit = gResources.Images.Cursors.HSplit.ToBitmap();
            GdiImages.Cursors.IBeam = gResources.Images.Cursors.IBeam.ToBitmap();
            GdiImages.Cursors.SizeAll = gResources.Images.Cursors.SizeAll.ToBitmap();
            GdiImages.Cursors.SizeNESW = gResources.Images.Cursors.SizeNESW.ToBitmap();
            GdiImages.Cursors.SizeNS = gResources.Images.Cursors.SizeNS.ToBitmap();
            GdiImages.Cursors.SizeNWSE = gResources.Images.Cursors.SizeNWSE.ToBitmap();
            GdiImages.Cursors.SizeWE = gResources.Images.Cursors.SizeWE.ToBitmap();
            GdiImages.Cursors.VSplit = gResources.Images.Cursors.VSplit.ToBitmap();

            ApplicationResources.Images = GdiImages;
            ApplicationResources.Fonts = new List<string>();
            
            if (Resources != null && Resources.Fonts != null)
                for (int i = 0; i < Resources.Fonts.Count; i++)
                {
                    var font = Resources.Fonts[i];
                    if (font != null)
                        ApplicationResources.Fonts.Add(font.fontNames[0]);
                }
            
            Cursors.images = GdiImages.Cursors;
            
            lastWidth = UE.Screen.width;
            lastHeight = UE.Screen.height;

            controller = new Application();
            controller.Resources = GdiImages;
            controller.UpdatePaintClipRect();

            Control.uwfDefaultController = controller;

#if UNITY_EDITOR
            MouseHook.MouseUp += (sender, args) => Inspect(sender);
#endif
        }
        private void Update()
        {
            var ueScreenWidth = UE.Screen.width;
            var ueScreenHeight = UE.Screen.height;

            Screen.width = (int) (ueScreenWidth / Application.ScaleX);
            Screen.height = (int) (ueScreenHeight / Application.ScaleY);

            if (controller == null) return;
            
            if (lastWidth != ueScreenWidth || lastHeight != ueScreenHeight)
            {
                var deltaWidth = (int) (lastWidth - UE.Screen.width);
                var deltaHeight = (int) (lastHeight - UE.Screen.height);
                var deltaSize = new Size(deltaWidth, deltaHeight);
                    
                for (int i = 0; i < controller.ModalForms.Count; i++)
                    controller.ModalForms[i].uwfAdjustSizeToScreen(deltaSize);
                    
                for (int i = 0; i < controller.Forms.Count; i++)
                    controller.Forms[i].uwfAdjustSizeToScreen(deltaSize);
                    
                controller.UpdatePaintClipRect();
            }

            lastWidth = ueScreenWidth;
            lastHeight = ueScreenHeight;

            controller.Update();
        }
        private void UpdateKeys()
        {
            var currentEvent = UE.Event.current;
            var currentKeyCode = currentEvent.keyCode;
            var currentKeyModifiers = currentEvent.modifiers;
            var currentType = currentEvent.type;

            // Shift key is not have a KeyDown and KeyUp events, so we need to update them manually.
            // There is also problem that if any key besides modifier keys were pressed along with Shift one
            // Unity will alternately send KeyDown events with pressed key and Shift key.
            
            // Update shift press.
            if (shiftPressed)
            {
                if (shiftDownDelayTimer <= 0)
                {
                    if (shiftDownTimer <= 0)
                    {
                        currentKeyModifiers = UE.EventModifiers.Shift;

                        if (currentKeyCode == UE.KeyCode.None)
                        {
                            currentKeyCode = UE.KeyCode.LeftShift;
                            currentType = UE.EventType.KeyDown;
                        }

                        shiftDownTimer = ShiftDownTime;
                    }
                    else
                        shiftDownTimer -= swfHelper.GetDeltaTime();
                }
                else
                    shiftDownDelayTimer -= swfHelper.GetDeltaTime();
            }

            var prevShift = shiftPressed;
            shiftPressed = currentEvent.shift;

            // Start shift key.
            if (!prevShift && shiftPressed)
            {
                currentKeyModifiers = UE.EventModifiers.Shift;
                currentKeyCode = UE.KeyCode.LeftShift;
                currentType = UE.EventType.KeyDown;
                shiftDownDelayTimer = ShiftDownDelayTime;
            }

            // Release shift key.
            if (prevShift && !shiftPressed)
            {
                currentKeyModifiers = UE.EventModifiers.Shift;
                currentKeyCode = UE.KeyCode.LeftShift;
                currentType = UE.EventType.KeyUp;
                shiftDownTimer = 0;
            }

            if (currentKeyCode == UE.KeyCode.None) return;
            if (currentType != UE.EventType.KeyDown && currentType != UE.EventType.KeyUp) return;
            
            var keyData = UnityKeyTranslator.ToKeyData(currentKeyModifiers, currentKeyCode);
            var keyArgs = new KeyEventArgs(keyData);

            var keyEventType = (Application.KeyEvents) (currentType - 3);
            if (keyEventType == Application.KeyEvents.Down || keyEventType == Application.KeyEvents.Up)
                controller.ProcessKeys(keyArgs, keyEventType);
        }
        private void UpdateMouse()
        {
            var currentEvent = UE.Event.current;
            var currentButton = currentEvent.button;
            var currentClicks = currentEvent.clickCount;
            var currentDelta = currentEvent.delta.y;
            var currentType = currentEvent.type;

            // Prepare mouse.
            var mouseButton = MouseButtons.None;
            var mouseEvent = Application.MouseEvents.None;
            var mouseWheelDelta = 0f;

            switch (currentType)
            {
                case UE.EventType.MouseDown:
                    switch (currentButton)
                    {
                        case 0:
                            mouseButton = MouseButtons.Left;
                            mouseEvent = Application.MouseEvents.Down;
                            if (currentClicks > 1)
                                mouseEvent = Application.MouseEvents.DoubleClick;
                            break;
                        case 1:
                            mouseButton = MouseButtons.Right;
                            mouseEvent = Application.MouseEvents.Down;
                            break;
                        case 2:
                            mouseButton = MouseButtons.Middle;
                            mouseEvent = Application.MouseEvents.Down;
                            break;
                    }

                    break;
                
                case UE.EventType.MouseUp:
                    switch (currentButton)
                    {
                        case 0:
                            mouseButton = MouseButtons.Left;
                            mouseEvent = Application.MouseEvents.Up;
                            break;
                        case 1:
                            mouseButton = MouseButtons.Right;
                            mouseEvent = Application.MouseEvents.Up;
                            break;
                        case 2:
                            mouseButton = MouseButtons.Middle;
                            mouseEvent = Application.MouseEvents.Up;
                            break;
                    }

                    break;
                
                case UE.EventType.ScrollWheel:
                    mouseEvent = Application.MouseEvents.Wheel;
                    mouseWheelDelta = currentDelta;
                    break;
            }

            // Mouse.
            var mouseX = UE.Input.mousePosition.x;
            var mouseY = UE.Screen.height - UE.Input.mousePosition.y;

            controller.ProcessMouse(mouseEvent, mouseX, mouseY, mouseButton, currentClicks, (int) mouseWheelDelta);
        }
        private void OnApplicationFocus(bool focusStatus)
        {
            paused = !focusStatus;

            if (paused && controller != null)
                controller.OnLostFocus();
            
            UE.Cursor.visible = Cursor.IsVisible;
        }
        private void OnGUI()
        {
            if (controller == null) return;

            if (paused == false)
            {
                UpdateMouse();
                UpdateKeys();
            }

            // Scale if needed.
            var scaleX = SWF.Application.ScaleX;
            var scaleY = SWF.Application.ScaleY;
            if (scaleX != 1f || scaleY != 1f)
                UnityEngine.GUI.matrix = UnityEngine.Matrix4x4.TRS(
                    UE.Vector3.zero,
                    UE.Quaternion.AngleAxis(0, UE.Vector3.up),
                    new UE.Vector3(scaleX, scaleY, 1));

            controller.Redraw();
        }
    }
}