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
        private objectEditor editor;
        private bool toggleEditor = true;

        public Control Control { get; set; }

        public ControlDesigner(Control c)
        {
            Control = c;

            editor = new objectEditor(c, Control.GetType().Name);
            editor.toggleEditor = true;
        }

        public virtual object Draw(int width, int height)
        {
            if (Control == null) return null;

            Control controlToSet = null;

            Editor.BeginGroup(width - 24, "");

            controlToSet = editor.Draw();

            Editor.EndGroup();

            return controlToSet;
        }

        private class controlProperty
        {
            public objectEditor editor;
            public bool expanded;
            public PropertyInfo info;
        }
        private class objectEditor
        {
            private readonly List<FieldInfo> fields;
            private readonly List<MethodInfo> methods;
            private readonly object obj;
            private readonly List<controlProperty> props;
            private readonly string name;

            public bool toggleEditor;

            public objectEditor(object o, string objName)
            {
                obj = o;
                name = objName;

                var objType = obj.GetType();
                var pList = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
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

                fields = objType.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
                fields.Sort((x, y) => x.Name.CompareTo(y.Name));

                methods = objType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).ToList();
                methods.Sort((x, y) => String.Compare(x.Name, y.Name, StringComparison.Ordinal));
                for (int i = 0; i < methods.Count; i++)
                {
                    var m = methods[i];
                    if (m.GetParameters().Length == 0 && m.ReturnType == typeof(void)) continue;

                    methods.RemoveAt(i);
                    i--;
                }
            }

            public Control Draw()
            {
                Control control = null;
                if (props.Count == 0 && methods.Count == 0)
                {
                    Editor.Label(name);
                    return null;
                }

                Editor.BeginVertical("Box");
                toggleEditor = Editor.Foldout(name, toggleEditor);
                if (toggleEditor)
                {
                    // Fields.
                    if (fields.Count > 0)
                    {
                        Editor.BeginVertical("Box");
                        for (int i = 0; i < fields.Count; i++)
                        {
                            var tc = Draw(fields[i]);
                            if (tc != null)
                                control = tc;
                        }
                        Editor.EndVertical();

                        Editor.NewLine(1);
                    }

                    // Properties.
                    for (int i = 0; i < props.Count; i++)
                    {
                        var tc = Draw(props[i]);
                        if (tc != null)
                            control = tc;
                    }

                    // Methods.
                    if (methods.Count > 0)
                    {
                        Editor.NewLine(1);

                        for (int i = 0; i < methods.Count; i++)
                        {
                            var tc = Draw(methods[i]);
                            if (tc != null)
                                control = tc;
                        }
                    }
                }
                Editor.EndVertical();
                return control;
            }
            public Control Draw(controlProperty p)
            {
                Control controlToSet = null;
                var val = p.info.GetValue(obj, null);
                Type type = null;
                if (val != null)
                    type = val.GetType();

                // Array & List.
                if (val is string == false)
                    if ((type != null && type.IsArray) || val is IEnumerable)
                    {
                        Editor.BeginVertical("Box");
                        p.expanded = Editor.Foldout(p.info.Name, p.expanded);
                        if (p.expanded)
                        {
                            var vEnum = val as IEnumerable;
                            var arrayIndex = 0;
                            foreach (var e in vEnum)
                            {
                                var ec = e as Control;
                                if (ec != null)
                                {
                                    if (Editor.Button(ec.ToString()))
                                        controlToSet = ec;
                                }
                                else
                                {
                                    if (p.editor == null)
                                        p.editor = new objectEditor(e, arrayIndex.ToString());

                                    p.editor.Draw();
                                }
                                arrayIndex++;
                            }
                        }
                        Editor.EndVertical();
                        return controlToSet;
                    }

                // If there is no Set() method then skip.
                var pSetMethod = p.info.GetSetMethod(true);
                if (pSetMethod == null || pSetMethod.IsPrivate)
                {
                    Editor.Label(p.info.Name, val);
                    return null;
                }

                // Other editors.
                if (val is bool)
                {
                    var bVal = (bool)val;
                    var ebVal = Editor.BooleanField(p.info.Name, bVal);
                    if (ebVal.Changed)
                        p.info.SetValue(obj, ebVal.Value, null);
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
                    Editor.ColorField(p.info.Name, colorVal, c => p.info.SetValue(obj, c, null));
                }
                else if (val is string)
                {
                    var stringtVal = (string)val;
                    var esVal = Editor.TextField(p.info.Name, stringtVal);
                    if (esVal.Changed)
                        p.info.SetValue(obj, esVal.Value, null);
                }
                else if (val is int)
                {
                    var eiVal = Editor.IntField(p.info.Name, (int) val);
                    if (eiVal.Changed)
                        p.info.SetValue(obj, eiVal.Value[0], null);
                }
                else if (val is byte || val is sbyte || val is short || val is ushort || val is uint || val is long || val is ulong || val is float || val is double)
                {
                    // TODO: editors for common types (like for int ^up there).
                    Editor.Label(p.info.Name, val);
                }
                else if (val is Enum)
                {
                    var eeVal = Editor.EnumField(p.info.Name, (Enum)val);
                    if (eeVal.Changed)
                        p.info.SetValue(obj, eeVal.Value, null);
                }
                else if (val != null)
                {
                    if (p.editor == null)
                        p.editor = new objectEditor(val, p.info.Name);

                    p.editor.Draw();
                }

                return controlToSet;
            }
            public Control Draw(FieldInfo f)
            {
                // TODO: editors for fields.

                var val = f.GetValue(obj);
                Editor.Label(f.Name, val);

                return null;
            }
            public Control Draw(MethodInfo m)
            {
                if (Editor.Button(m.Name))
                {
                    m.Invoke(obj, null);
                }

                return null;
            }
        }
    }
}
