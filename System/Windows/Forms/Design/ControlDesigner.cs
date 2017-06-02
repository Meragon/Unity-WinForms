using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Windows.Forms.Design
{
    public class ControlDesigner : IControlDesigner
    {
        private List<PropertyInfo> props;
        private bool toggleEditor = true;

        public Control Control { get; set; }

        public ControlDesigner(Control c)
        {
            Control = c;

            if (Control != null)
                Init();
        }

        private void Init()
        {
            props = Control.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            props.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
        }

        public virtual object Draw(int width, int height)
        {
            if (Control == null) return null;
            if (props == null) return null;

            Control controlToSet = null;

            Editor.BeginGroup(width - 24);

            toggleEditor = Editor.Foldout("Control (" + Control.GetType().Name + ")", toggleEditor);
            if (toggleEditor)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    var p = props[i];
                    var val = p.GetValue(Control, null);

                    var pSetMethod = p.GetSetMethod(true);
                    if (pSetMethod == null || pSetMethod.IsPrivate)
                    {
                        Editor.Label(p.Name, val);
                        continue;
                    }

                    if (val is bool)
                    {
                        var bVal = (bool)val;
                        var ebVal = Editor.BooleanField(p.Name, bVal);
                        if (ebVal.Changed)
                            p.SetValue(Control, ebVal.Value, null);
                    }
                    else if (val is Control)
                    {
                        var cVal = val as Control;
                        if (Editor.Button(p.Name, cVal.GetType().Name))
                            controlToSet = cVal;
                    }
                    else if (val is Color)
                    {
                        var colorVal = (Color)val;
                        Editor.ColorField(p.Name, colorVal, c => p.SetValue(Control, c, null));
                    }
                    else if (val is string)
                    {
                        var stringtVal = (string)val;
                        var esVal = Editor.TextField(p.Name, stringtVal);
                        if (esVal.Changed)
                            p.SetValue(Control, esVal.Value, null);
                    }
                    else
                        Editor.Label(p.Name, val);
                }
            }

            Editor.EndGroup();

            return controlToSet;
        }
    }
}
