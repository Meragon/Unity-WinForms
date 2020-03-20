#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using SWF = System.Windows.Forms;

namespace Unity.Views
{
    public class AppControlList : EditorWindow
    {
        private string filter = "";
        private float _repaintWait;
        private Vector2 scrollPosition;

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
            if (!Application.isPlaying) return;
            if (SWF.Control.uwfDefaultController == null)
            {
                GUILayout.Label("SWF.Control.DefaultController is null");
                return;
            }
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical();
            filter = GUILayout.TextField(filter).ToLower();
            
            GUILayout.Label("Context");
            
            for (int i = 0; i < SWF.Control.uwfDefaultController.Contexts.Count; i++)
                PaintControl(SWF.Control.uwfDefaultController.Contexts[i]);

            GUILayout.Space(24);
            GUILayout.Label("Modal Forms");

            for (int i = 0; i < SWF.Control.uwfDefaultController.ModalForms.Count; i++)
                PaintControl(SWF.Control.uwfDefaultController.ModalForms[i]);

            GUILayout.Space(24);
            GUILayout.Label("Forms");
            
            for (int i = 0; i < SWF.Control.uwfDefaultController.Forms.Count; i++)
                PaintControl(SWF.Control.uwfDefaultController.Forms[i]);

            GUILayout.Space(24);
            GUILayout.Label("Hovered Control");

            var hoveredControl = SWF.Control.uwfDefaultController.hoveredControl;
            if (hoveredControl != null) 
                PaintControl(hoveredControl);
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void PaintControl(SWF.Control control)
        {
            string controlType = control.GetType().ToString().Replace("System.Windows.Forms", "SWF");
            string controlName = control.Name;
                
            if (controlName == null) 
                controlName = "";
                
            if (!string.IsNullOrEmpty(filter))
            {
                if (!controlType.ToLower().Contains(filter) && !controlName.ToLower().Contains(filter))
                    return;
            }

            GUILayout.BeginHorizontal();
                
            if (GUILayout.Button("...", GUILayout.Width(24)))
            {
                EditorMenu.ShowInspector();
                ControlInspector.DesignerObject = control;
            }
                
            GUILayout.Label(control.GetHashCode().ToString("X2"), GUILayout.Width(64));
            GUILayout.Label(controlType);
            GUILayout.Label(controlName);
            GUILayout.EndHorizontal();
        }
    }
}

#endif