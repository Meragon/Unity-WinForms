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
        private readonly objectEditor editor;

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

            Editor.SetBackColor(Color.White);
            Editor.BeginGroup(width - 24, "");

            controlToSet = editor.Draw();

            Editor.EndGroup();

            return controlToSet;
        }

        private class controlProperty
        {
            public readonly List<objectEditor> arrayEditors = new List<objectEditor>();
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
            private int rgb = 255;

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
                    if (p.DeclaringType == typeof(Delegate)) continue;
                    if (p.Name == "Item") continue; // this[] will throw an exception.

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

                toggleEditor = Editor.Foldout(name, toggleEditor);
                var style = toggleEditor ? "Box" : null;
                Editor.BeginVertical(style);
                if (toggleEditor)
                {
                    // Fields.
                    if (fields.Count > 0)
                    {
                        Editor.SetBackColor(Color.AliceBlue);
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
                    Editor.SetBackColor(Color.White);
                    if (toggleEditor)
                        Editor.SetBackColor(Color.FromArgb(rgb, rgb, rgb));
                    for (int i = 0; i < props.Count; i++)
                    {
                        var tc = Draw(props[i]);
                        if (tc != null)
                            control = tc;
                    }

                    // Methods.
                    Editor.SetBackColor(Color.White);
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

                if (toggleEditor)
                    Editor.NewLine(1);

                return control;
            }
            public Control Draw(controlProperty p)
            {
                Control controlToSet = null;

                if (p.info.CanRead == false) return null;

                var val = p.info.GetValue(obj, null);
                Type type = null;
                if (val != null)
                    type = val.GetType();
                else
                {
                    Editor.Label(p.info.Name, "null");
                    return null;
                }

                // Array & List.
                if (val is string == false)
                    if (type.IsArray || val is IEnumerable)
                    {
                        Editor.BeginVertical();
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
                                    if (arrayIndex >= p.arrayEditors.Count)
                                    {
                                        var aEditor = new objectEditor(e, arrayIndex.ToString());
                                        aEditor.rgb = rgb - 25;
                                        if (aEditor.rgb < 128)
                                            aEditor.rgb = 128;
                                        p.arrayEditors.Add(aEditor);
                                    }

                                    p.arrayEditors[arrayIndex].Draw();
                                }
                                arrayIndex++;
                            }
                        }
                        Editor.EndVertical();
                        return controlToSet;
                    }

                // If there is no Set() method then skip.
                var canSet = true;
                var pSetMethod = p.info.GetSetMethod(true);
                if (pSetMethod == null || pSetMethod.IsPrivate)
                    canSet = false;

                // Other editors.
                if (val is bool)
                {
                    if (canSet == false)
                    {
                        Editor.Label(p.info.Name, val);
                        return null;
                    }

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
                    if (canSet == false)
                    {
                        Editor.Label(p.info.Name, val);
                        return null;
                    }

                    var colorVal = (Color)val;
                    Editor.ColorField(p.info.Name, colorVal, c => p.info.SetValue(obj, c, null));
                }
                else if (val is string)
                {
                    if (canSet == false)
                    {
                        Editor.Label(p.info.Name, val);
                        return null;
                    }

                    var stringtVal = (string)val;
                    var esVal = Editor.TextField(p.info.Name, stringtVal);
                    if (esVal.Changed)
                        p.info.SetValue(obj, esVal.Value, null);
                }
                else if (val is int)
                {
                    if (canSet == false)
                    {
                        Editor.Label(p.info.Name, val);
                        return null;
                    }

                    var eiVal = Editor.IntField(p.info.Name, (int)val);
                    if (eiVal.Changed)
                        p.info.SetValue(obj, eiVal.Value[0], null);
                }
                else if (val is byte || val is sbyte || val is short || val is ushort || val is uint || val is long || val is ulong || val is float || val is double)
                {
                    if (canSet == false)
                    {
                        Editor.Label(p.info.Name, val);
                        return null;
                    }

                    // TODO: editors for common types (like for int ^up there).
                    Editor.Label(p.info.Name, val);
                }
                else if (val is Enum)
                {
                    if (canSet == false)
                    {
                        Editor.Label(p.info.Name, val);
                        return null;
                    }
                    
                    var enumHasFlagAttribute = val.GetType().GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;
                    var enumOptions = Enum.GetNames(val.GetType());

                    if (enumHasFlagAttribute)
                    {
                        // TODO: not gonna work with 'None' flag.
                        // https://forum.unity3d.com/threads/editorguilayout-enummaskfield-doesnt-use-enums-values.233332/
                        var eeVal = Editor.MaskField(p.info.Name, Convert.ToInt32(val), enumOptions);
                        if (eeVal.Changed)
                            p.info.SetValue(obj, eeVal.Value, null);
                    }
                    else
                    {
                        var eeVal = Editor.EnumField(p.info.Name, (Enum)val);
                        if (eeVal.Changed)
                            p.info.SetValue(obj, eeVal.Value, null);
                    }
                }
                else if (val != null)
                {
                    if (p.editor == null)
                    {
                        p.editor = new objectEditor(val, p.info.Name);
                        p.editor.rgb = rgb - 25;
                        if (p.editor.rgb < 128)
                            p.editor.rgb = 128;
                    }

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
