using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.UI;

namespace System.Windows.Forms.Design
{
    public class ControlDesigner : IControlDesigner
    {
        private List<controlProperty> props;
        private bool toggleEditor = true;

        public Control Control { get; set; }

        public ControlDesigner(Control c)
        {
            Control = c;

            if (Control != null)
                Init();
        }

        private Control DrawProperty(controlProperty p)
        {
            Control controlToSet = null;
            var val = p.info.GetValue(Control, null);
            Type type = null;
            if (val != null)
                type = val.GetType();

            var pSetMethod = p.info.GetSetMethod(true);
            if (pSetMethod == null || pSetMethod.IsPrivate)
            {
                Editor.Label(p.info.Name, val);
                return null;
            }

            if (val is bool)
            {
                var bVal = (bool)val;
                var ebVal = Editor.BooleanField(p.info.Name, bVal);
                if (ebVal.Changed)
                    p.info.SetValue(Control, ebVal.Value, null);
            }
            else if (val is Control)
            {
                var cVal = val as Control;
                if (Editor.Button(p.info.Name, cVal.GetType().Name))
                    controlToSet = cVal;
            }
            else if (val is Color)
            {
                var colorVal = (Color)val;
                Editor.ColorField(p.info.Name, colorVal, c => p.info.SetValue(Control, c, null));
            }
            else if (val is string)
            {
                var stringtVal = (string)val;
                var esVal = Editor.TextField(p.info.Name, stringtVal);
                if (esVal.Changed)
                    p.info.SetValue(Control, esVal.Value, null);
            }
            else if ((type != null && type.IsArray) || val is IEnumerable)
            {
                p.expanded = Editor.Foldout(p.info.Name, p.expanded);
                if (p.expanded)
                {
                    var vEnum = val as IEnumerable;
                    foreach (var e in vEnum)
                    {
                        var ec = e as Control;
                        if (ec != null)
                        {
                            if (Editor.Button(ec.ToString()))
                                controlToSet = ec;
                        }
                        else
                            Editor.Label(e);
                    }
                }
            }
            else
                Editor.Label(p.info.Name, val);

            return controlToSet;
        }
        private void Init()
        {
            var pList = Control.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            pList.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));

            props = new List<controlProperty>();
            for (int i = 0; i < pList.Count; i++)
            {
                var p = pList[i];
                var cp = new controlProperty()
                {
                    info = p,
                };

                props.Add(cp);
            }
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
                    var c = DrawProperty(p);
                    if (c != null)
                        controlToSet = c;
                }
            }

            Editor.EndGroup();

            return controlToSet;
        }

        private class controlProperty
        {
            public bool expanded;
            public PropertyInfo info;
        }
    }
}
