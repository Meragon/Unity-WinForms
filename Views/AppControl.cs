using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Views
{
    public class AppControl : EditorWindow
    {
        private static AppControl _self;
        public static AppControl Self
        {
            get
            {
                if (_self == null)
                {
                    _self = (AppControl)ShowWindow();
                }
                return _self;
            }
        }

        [MenuItem("Views/SWF Inspector")]
        public static EditorWindow ShowWindow()
        {
            return EditorWindow.GetWindow(typeof(AppControl), false, "SWF Inspector");
        }

        private float _repaintWait;
        private Vector2 _scrollPosition;

        private System.Windows.Forms.Control _control;
        public System.Windows.Forms.Control Control
        {
            get { return _control; }
            set { _control = value; }
        }

        void Awake()
        {
            if (_control != null)
            {
                _control.Dispose();
                _control = null;
            }
        }
        void Update()
        {
            if (_repaintWait < 1)
                _repaintWait += Time.deltaTime;
            else
            {
                Repaint();
                _repaintWait = 0;
            }
        }
        void OnGUI()
        {
            if (Control == null) return;
            if (Control.Disposing || Control.IsDisposed || !Application.isPlaying)
            {
                _control = null;
                return;
            }

            _scrollPosition = UnityEngine.GUILayout.BeginScrollView(_scrollPosition);

            var control = Control.RaiseOnPaintEditor(position.width);

            UnityEngine.GUILayout.Label("");
            if (UnityEngine.GUILayout.Button("Dispose", "Box"))
                Control.Dispose();
            if (UnityEngine.GUILayout.Button("Render", "Box"))
            {
                var renderWindow = (AppControlRender)AppControlRender.ShowWindow();
                renderWindow.Control = Control;
            }

            if (control != null && control is System.Windows.Forms.Control) Control = control as System.Windows.Forms.Control;
            UnityEngine.GUILayout.EndScrollView();
        }
    }
}

#endif