using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Unity.Views
{
    using System.Windows.Forms;

    using Application = UnityEngine.Application;

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

        [MenuItem("Window/NetWinForms/SWF Inspector")]
        public static EditorWindow ShowWindow()
        {
            return EditorWindow.GetWindow(typeof(AppControl), false, "SWF Inspector");
        }

        private IObjectDesigner designer;
        private float repaintWait;
        private Vector2 scrollPosition;

        public object DesignerObject;

        void Awake()
        {
            var iDisposable = DesignerObject as IDisposable;
            if (iDisposable != null)
                iDisposable.Dispose();
            DesignerObject = null;
        }
        void Update()
        {
            if (repaintWait < 1f)
                repaintWait += Time.deltaTime;
            else
            {
                Repaint();
                repaintWait = 0;
            }
        }
        void OnGUI()
        {
            if (DesignerObject == null) return;

            var control = DesignerObject as Control;
            if (control != null && (control.Disposing || control.IsDisposed) || !Application.isPlaying)
            {
                DesignerObject = null;
                return;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            if (designer == null || designer.Value != DesignerObject)
                designer = new ObjectDesigner(DesignerObject);
            
            var newObject = designer.Draw((int)position.width - 8, int.MaxValue);
            if (newObject != null)
                DesignerObject = newObject;
            
            GUILayout.EndScrollView();
        }
    }
}

#endif