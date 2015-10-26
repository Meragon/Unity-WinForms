using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace Views
{
    class AppControlRender : EditorWindow
    {
        [MenuItem("Views/AppControlRender")]
        public static EditorWindow ShowWindow()
        {
            return EditorWindow.GetWindow(typeof(AppControlRender), false, "AppControlRender");
        }

        private float _repaintWait;

        private System.Windows.Forms.Control _control;
        public System.Windows.Forms.Control Control
        {
            get { return _control; }
            set { _control = value; }
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
            if (Control.Disposing || Control.IsDisposed)
            {
                _control = null;
                return;
            }

            System.Drawing.Graphics g = new System.Drawing.Graphics();
            g.Control = Control;
            Control.RaiseOnPaint(new System.Windows.Forms.PaintEventArgs() { Graphics = g });

            GUI.skin.label.font = null;
            GUI.skin.label.fontSize = 13;
        }
    }
}
#endif