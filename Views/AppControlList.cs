using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Views
{
    public class AppControlList : EditorWindow
    {
        [MenuItem("Views/AppControlList")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(AppControlList), false, "AppControlList");
        }

        private string _filter = "";
        private float _repaintWait;
        private Vector2 _scrollPosition;

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

            float width = position.width - 24;
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            GUILayout.BeginVertical();
            _filter = GUILayout.TextField(_filter);
            for (int i = 0; i < System.Windows.Forms.Control.DefaultController.Controls.Count; i++)
            {
                string c_type = System.Windows.Forms.Control.DefaultController.Controls[i].GetType().ToString().Replace("System.Windows.Forms", "SWF");
                string c_name = System.Windows.Forms.Control.DefaultController.Controls[i].Name;
                if (c_name == null) c_name = "";
                if (!String.IsNullOrEmpty(_filter))
                {
                    if (!c_type.ToLower().Contains(_filter.ToLower()) && !c_name.ToLower().Contains(_filter.ToLower()))
                        continue;
                }

                GUILayout.BeginHorizontal(GUILayout.Width(width));
                if (GUILayout.Button("...", GUILayout.Width(24)))
                {
                    var w = AppControl.ShowWindow();
                    (w as AppControl).Control = System.Windows.Forms.Control.DefaultController.Controls[i];
                }
                GUILayout.Label(c_type, GUILayout.Width(160));
                GUILayout.Label(c_name, GUILayout.Width(220));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
    }
}

#endif