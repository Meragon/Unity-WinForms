namespace Unity.Views
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    public class ObjectDesigner : IObjectDesigner
    {
        private readonly objectEditor editor;

        public ObjectDesigner(object c)
        {
            Value = c;

            editor = new objectEditor(c, c.GetType().Name);
            editor.toggleEditor = true;
        }

        public object Value { get; set; }

        public virtual object Draw(int width, int height)
        {
            if (Value == null) return null;

            Editor.SetBackColor(Color.White);
            Editor.BeginGroup(width - 24, "");

            object objectToSet = editor.Draw();

            Editor.EndGroup();

            return objectToSet;
        }

        private class controlProperty
        {
            public readonly List<objectEditor> arrayEditors = new List<objectEditor>();
            public readonly PropertyInfo info;
            public readonly bool canSet;
            
            public objectEditor editor;
            public bool expanded;
            
            public controlProperty(PropertyInfo propertyInfo)
            {
                canSet = true;
                info = propertyInfo;
                
                var pSetMethod = info.GetSetMethod(true);
                if (pSetMethod == null || pSetMethod.IsPrivate)
                    canSet = false;
            }
        }
        private class objectEditor
        {
            public bool toggleEditor;

            private readonly List<FieldInfo> fields;
            private readonly List<MethodInfo> methods;
            private readonly object target;
            private readonly List<controlProperty> properties;
            private readonly string name;
            
            private int backgroundRGB = 255;

            public objectEditor(object o, string objName)
            {
                target = o;
                name = objName;

                var objType = target.GetType();
                var pList = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
                pList.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));

                properties = new List<controlProperty>();
                
                for (int i = 0; i < pList.Count; i++)
                {
                    var propertyInfo = pList[i];
                    if (propertyInfo.DeclaringType == typeof(Delegate)) continue;
                    if (propertyInfo.Name == "Item") continue; // this[] will throw an exception.

                    properties.Add(new controlProperty(propertyInfo));
                }

                fields = objType.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
                fields.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));

                methods = objType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).ToList();
                methods.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
                
                for (int i = 0; i < methods.Count; i++)
                {
                    var methodInfo = methods[i];
                    if (methodInfo.GetParameters().Length == 0 && methodInfo.ReturnType == typeof(void)) continue;

                    methods.RemoveAt(i);
                    i--;
                }
            }

            public object Draw()
            {
                if (properties.Count == 0 && methods.Count == 0)
                {
                    Editor.Label(name);
                    return null;
                }

                toggleEditor = Editor.Foldout(name, toggleEditor);
                
                Editor.BeginVertical(toggleEditor ? "Box" : null);
                
                if (toggleEditor)
                {
                    // Fields.
                    if (fields.Count > 0)
                    {
                        Editor.SetBackColor(Color.AliceBlue);
                        Editor.BeginVertical("Box");
                        Editor.Label("    Fields");
                        
                        for (int i = 0; i < fields.Count; i++)
                        {
                            var tc = Draw(fields[i]);
                            if (tc != null) return tc;
                        }
                        
                        Editor.EndVertical();
                        Editor.NewLine(1);
                    }

                    // Properties.
                    Editor.SetBackColor(Color.White);
                    if (toggleEditor)
                        Editor.SetBackColor(Color.FromArgb(backgroundRGB, backgroundRGB, backgroundRGB));
                    
                    for (int i = 0; i < properties.Count; i++)
                    {
                        var tc = Draw(properties[i]);
                        if (tc != null) return tc;
                    }

                    // Methods.
                    if (methods.Count > 0)
                    {
                        if (properties.Count > 0)
                            Editor.NewLine(1);
                        Editor.SetBackColor(Color.Lavender);
                        Editor.BeginVertical("Box");
                        Editor.Label("    Methods");
                        
                        for (int i = 0; i < methods.Count; i++)
                        {
                            var tc = Draw(methods[i]);
                            if (tc != null) return tc;
                        }
                        
                        Editor.EndVertical();
                    }
                }
                
                Editor.EndVertical();

                if (toggleEditor)
                    Editor.NewLine(1);

                return null;
            }
            public object Draw(controlProperty property)
            {
                if (property.info.CanRead == false) return null;

                var value = property.info.GetValue(target, null);
                if (value == null)
                {
                    Editor.Label(property.info.Name, "(" + property.info.PropertyType.Name +  ") null");
                    return null;
                }

                // Array & List.
                if (value is string == false && (value.GetType().IsArray || value is IEnumerable))
                {
                    return PropertyEditorEnumerable(property, (IEnumerable) value);
                }

                // Base editors.
                if (value is bool)    return PropertyEditorBool(property, (bool) value);
                if (value is byte)    return PropertyEditorUInt8(property, (byte) value);
                if (value is decimal) return PropertyEditorDecimal(property, (decimal) value);
                if (value is double)  return PropertyEditorDouble(property, (double) value);
                if (value is float)   return PropertyEditorSingle(property, (float) value);
                if (value is int)     return PropertyEditorInt32(property, (int) value);
                if (value is long)    return PropertyEditorInt64(property, (long) value);
                if (value is sbyte)   return PropertyEditorInt8(property, (sbyte) value);
                if (value is short)   return PropertyEditorInt16(property, (short) value);
                if (value is uint)    return PropertyEditorUInt32(property, (uint) value);
                if (value is ulong)   return PropertyEditorUInt64(property, (ulong) value);
                if (value is ushort)  return PropertyEditorUInt16(property, (ushort) value);
                
                if (value is string)  return PropertyEditorString(property, (string) value);
                if (value is Color)   return PropertyEditorColor(property, (Color) value);
                if (value is Control) return PropertyEditorControl(property, (Control) value);
                if (value is Enum)    return PropertyEditorEnum(property, (Enum) value);
                
                // Complex types.
                if (property.editor == null)
                {
                    property.editor = new objectEditor(value, property.info.Name);
                    property.editor.backgroundRGB = MathHelper.Clamp(backgroundRGB - 25, 128, 255);
                }

                return property.editor.Draw();
            }
            public object Draw(FieldInfo field)
            {
                // TODO: editors for fields.

                var val = field.GetValue(target);
                Editor.Label(field.Name, val);

                return null;
            }
            public object Draw(MethodInfo method)
            {
                if (Editor.Button(method.Name, "Invoke(null)"))
                    method.Invoke(target, null);

                return null;
            }

            public object PropertyEditorBool(controlProperty property, bool value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var ebVal = Editor.BooleanField(property.info.Name, value);
                if (ebVal.Changed)
                    property.info.SetValue(target, ebVal.Value, null);

                return null;
            }
            public object PropertyEditorColor(controlProperty property, Color value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                Editor.ColorField(property.info.Name, value, c => property.info.SetValue(target, c, null));
                return null;
            }
            public object PropertyEditorControl(controlProperty property, Control value)
            {
                if (Editor.Button(property.info.Name, value.GetType().Name))
                    return value;

                return null;
            }
            public object PropertyEditorDecimal(controlProperty property, decimal value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var eiVal = Editor.TextField(property.info.Name, value.ToString(CultureInfo.InvariantCulture));
                if (eiVal.Changed)
                {
                    decimal newValue;
                    decimal.TryParse(eiVal.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out newValue);
                    
                    property.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorDouble(controlProperty property, double value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var eiVal = Editor.TextField(property.info.Name, value.ToString(CultureInfo.InvariantCulture));
                if (eiVal.Changed)
                {
                    double newValue;
                    double.TryParse(eiVal.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out newValue);
                    
                    property.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorEnum(controlProperty property, Enum value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }
                    
                var enumHasFlagAttribute = value.GetType().GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;
                var enumOptions = Enum.GetNames(value.GetType());

                if (enumHasFlagAttribute)
                {
                    // TODO: not gonna work with 'None' flag.
                    // https://forum.unity3d.com/threads/editorguilayout-enummaskfield-doesnt-use-enums-values.233332/
                    var eeVal = Editor.MaskField(property.info.Name, Convert.ToInt32(value), enumOptions);
                    if (eeVal.Changed)
                        property.info.SetValue(target, eeVal.Value, null);
                }
                else
                {
                    var eeVal = Editor.EnumField(property.info.Name, (Enum)value);
                    if (eeVal.Changed)
                        property.info.SetValue(target, eeVal.Value, null);
                }

                return null;
            }
            public object PropertyEditorEnumerable(controlProperty property, IEnumerable value)
            {
                Editor.BeginVertical();
                property.expanded = Editor.Foldout(property.info.Name, property.expanded);
                if (property.expanded)
                {
                    var arrayIndex = 0;
                            
                    foreach (var item in value)
                    {
                        var control = item as Control;
                        if (control != null)
                        {
                            if (Editor.Button("    " + arrayIndex, control.ToString()))
                                return control;
                        }
                        else
                        {
                            if (arrayIndex >= property.arrayEditors.Count)
                            {
                                var itemText = "null";
                                if (item != null) itemText = item.ToString();
                                        
                                var aEditor = new objectEditor(item, arrayIndex + " (" + itemText + ")");
                                aEditor.backgroundRGB = MathHelper.Clamp(backgroundRGB - 25, 128, 255);
                                        
                                property.arrayEditors.Add(aEditor);
                            }

                            property.arrayEditors[arrayIndex].Draw();
                        }
                                
                        arrayIndex++;
                    }
                }
                    
                Editor.EndVertical();
                return null;
            }
            public object PropertyEditorInt8(controlProperty property, sbyte value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var eiVal = Editor.IntField(property.info.Name, value);
                if (eiVal.Changed)
                {
                    var newValue = (sbyte) MathHelper.Clamp(eiVal.Value[0], sbyte.MinValue, sbyte.MaxValue);
                    
                    property.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorInt16(controlProperty property, short value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var eiVal = Editor.IntField(property.info.Name, value);
                if (eiVal.Changed)
                {
                    var newValue = (short) MathHelper.Clamp(eiVal.Value[0], short.MinValue, short.MaxValue);
                    
                    property.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorInt32(controlProperty property, int value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var eiVal = Editor.IntField(property.info.Name, value);
                if (eiVal.Changed)
                    property.info.SetValue(target, eiVal.Value[0], null);
                
                return null;
            }
            public object PropertyEditorInt64(controlProperty property, long value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var eiVal = Editor.TextField(property.info.Name, value.ToString());
                if (eiVal.Changed)
                {
                    long newValue;
                    long.TryParse(eiVal.Value, out newValue);
                    
                    property.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorSingle(controlProperty property, float value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var eiVal = Editor.TextField(property.info.Name, value.ToString(CultureInfo.InvariantCulture));
                if (eiVal.Changed)
                {
                    float newValue;
                    float.TryParse(eiVal.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out newValue);
                    
                    property.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorString(controlProperty property, string value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var esVal = Editor.TextField(property.info.Name, value);
                if (esVal.Changed)
                    property.info.SetValue(target, esVal.Value, null);
                
                return null;
            }
            public object PropertyEditorUInt8(controlProperty property, byte value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var eiVal = Editor.IntField(property.info.Name, value);
                if (eiVal.Changed)
                {
                    var newValue = (byte) MathHelper.Clamp(eiVal.Value[0], byte.MinValue, byte.MaxValue);
                    
                    property.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorUInt16(controlProperty property, ushort value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var eiVal = Editor.IntField(property.info.Name, value);
                if (eiVal.Changed)
                {
                    var newValue = (ushort) MathHelper.Clamp(eiVal.Value[0], ushort.MinValue, ushort.MaxValue);
                    
                    property.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorUInt32(controlProperty property, uint value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var eiVal = Editor.TextField(property.info.Name, value.ToString());
                if (eiVal.Changed)
                {
                    uint newValue;
                    uint.TryParse(eiVal.Value, out newValue);
                    
                    property.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorUInt64(controlProperty property, ulong value)
            {
                if (property.canSet == false)
                {
                    Editor.Label(property.info.Name, value);
                    return null;
                }

                var eiVal = Editor.TextField(property.info.Name, value.ToString());
                if (eiVal.Changed)
                {
                    ulong newValue;
                    ulong.TryParse(eiVal.Value, out newValue);
                    
                    property.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
        }
    }
}
