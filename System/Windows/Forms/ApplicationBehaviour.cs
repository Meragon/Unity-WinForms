using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    public sealed class ApplicationBehaviour : MonoBehaviour
    {
        private static Texture2D _defaultSprite;

        public static Texture2D DefaultSprite
        {
            get
            {
                if (_defaultSprite == null)
                {
                    _defaultSprite = new Texture2D(32, 32);
                    for (int i = 0; i < _defaultSprite.height; i++)
                        for (int k = 0; k < _defaultSprite.width; k++)
                            _defaultSprite.SetPixel(k, i, Color.white);
                    _defaultSprite.Apply();
                }
                return _defaultSprite;
            }
        }
        internal static Texture2D DefaultSpriteSmoothLine { get; private set; }
        public static AppResources Resources { get; private set; }

        public Material LineMaterial;
        public AppResources _Resources;
        public static bool ShowControlProperties { get; set; }

        private Application _controller;
        private float _lastWidth;
        private float _lastHeight;
        private bool _paused;

        private void Awake()
        {
            if (LineMaterial != null)
                System.Drawing.Graphics.DefaultMaterial = LineMaterial;
            else
            {
                System.Drawing.Graphics.DefaultMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
                    "SubShader { Pass { " +
                    "    Blend SrcAlpha OneMinusSrcAlpha " +
                    "    ZWrite Off Cull Off Fog { Mode Off } " +
                    "    BindChannels {" +
                    "      Bind \"vertex\", vertex Bind \"color\", color }" +
                    "} } }");
                System.Drawing.Graphics.DefaultMaterial.hideFlags = HideFlags.HideAndDontSave;
                System.Drawing.Graphics.DefaultMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
            }
            Resources = _Resources;

            _lastWidth = UnityEngine.Screen.width;
            _lastHeight = UnityEngine.Screen.height;

            _controller = new Application();
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
                    _controller.ModalForms[i].AddjustSizeToScreen(deltaSize);
                for (int i = 0; i < _controller.Forms.Count; i++)
                    _controller.Forms[i].AddjustSizeToScreen(deltaSize);
            }
            _lastWidth = UnityEngine.Screen.width;
            _lastHeight = UnityEngine.Screen.height;

            _controller.Update();
        }
        private void OnApplicationFocus(bool focusStatus)
        {
            _paused = !focusStatus;
        }
        private void OnGUI()
        {
            if (_paused == false)
            {
                var mousePosition = new System.Drawing.PointF(Input.mousePosition.x, UnityEngine.Screen.height - Input.mousePosition.y);
                _controller.ProccessMouse(mousePosition);
                _controller.ProccessKeys();
            }

            _controller.Draw();
        }
    }
}