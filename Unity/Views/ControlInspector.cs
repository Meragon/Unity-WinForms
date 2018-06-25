#if UNITY_EDITOR

namespace Unity.Views
{
    using System;
    using System.Windows.Forms;

    using UnityEngine;
    using UnityEditor;

    public class ControlInspector : EditorWindow
    {
        private static ControlInspector _self;
        public static ControlInspector Self
        {
            get
            {
                if (_self == null)
                {
                    _self = (ControlInspector)EditorMenu.ShowInspector();
                }
                return _self;
            }
        }

        private IObjectDesigner designer;
        private float repaintWait;
        private Vector2 scrollPosition;

        internal object DesignerObject;

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
            if (control != null && (control.Disposing || control.IsDisposed) || !UnityEngine.Application.isPlaying)
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