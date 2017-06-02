using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
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
            if (_repaintWait < 1f)
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

            if (Control.uwfDesigner == null)
                Control.uwfDesigner = Control.GetDesigner();

            var control = Control.uwfDesigner.Draw((int)position.width - 8, int.MaxValue);

            if (control is System.Windows.Forms.Control) Control = control as System.Windows.Forms.Control;
            UnityEngine.GUILayout.EndScrollView();
        }
    }
}

#endif