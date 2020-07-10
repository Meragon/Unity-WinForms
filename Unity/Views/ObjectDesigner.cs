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
        private readonly ObjectEditor editor;

        public ObjectDesigner(object c)
        {
            Value = c;

            editor = new ObjectEditor(c, c.GetType().Name);
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

        private class PropertyEditor
        {
            public readonly PropertyInfo info;
            public readonly bool canSet;
            
            public ObjectEditor editor;
            
            public PropertyEditor(PropertyInfo propertyInfo, bool instanceIsValueType)
            {
                canSet = !instanceIsValueType;
                info = propertyInfo;
                
                var pSetMethod = info.GetSetMethod(true);
                if (pSetMethod == null || pSetMethod.IsPrivate)
                    canSet = false;
            }
        }
        private class ObjectEditor
        {
            public bool toggleEditor;
            public readonly List<ObjectEditor> arrayEditors = new List<ObjectEditor>();

            private readonly List<FieldInfo> fields;
            private readonly List<MethodInfo> methods;
            private readonly object target;
            private readonly List<PropertyEditor> properties;
            private readonly string name;
            
            private int backgroundRGB = 255;
            private bool enumerableExpanded;

            public ObjectEditor(object o, string objName)
            {
                target = o;
                name = objName;

                var objType = target.GetType();
                var pList = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
                pList.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));

                properties = new List<PropertyEditor>();
                
                for (int i = 0; i < pList.Count; i++)
                {
                    var propertyInfo = pList[i];
                    if (propertyInfo.DeclaringType == typeof(Delegate)) continue;
                    if (propertyInfo.Name == "Item") continue; // this[] will throw an exception.

                    properties.Add(new PropertyEditor(propertyInfo, objType.IsValueType));
                }

                fields = objType.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
                fields.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));

                methods = objType.GetMethods(BindingFlags.Instance | BindingFlags.Public).ToList();
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
                    
                    // Array & List.
                    if (!(target is string) && (target.GetType().IsArray || target is IEnumerable))
                    {
                        Editor.BeginVertical();
                        
                        {
                            var arrayIndex = 0;
                            var e = target as IEnumerable;
                            
                            foreach (var item in e)
                            {
                                var control = item as Control;
                                if (control != null)
                                {
                                    if (Editor.Button("    " + arrayIndex, control.ToString()))
                                        return control;
                                }
                                else
                                {
                                    if (arrayIndex >= arrayEditors.Count)
                                    {
                                        var itemText = "null";
                                        if (item != null) itemText = item.ToString();
                                        
                                        var aEditor = new ObjectEditor(item, arrayIndex + " (" + itemText + ")");
                                        aEditor.backgroundRGB = MathHelper.Clamp(backgroundRGB - 25, 128, 255);
                                        
                                        arrayEditors.Add(aEditor);
                                    }

                                    arrayEditors[arrayIndex].Draw();
                                }
                                
                                arrayIndex++;
                            }
                        }
                    
                        Editor.EndVertical();
                    }
                }
                
                Editor.EndVertical();

                if (toggleEditor)
                    Editor.NewLine(1);

                return null;
            }
            public object Draw(PropertyEditor propertyEditor)
            {
                if (!propertyEditor.info.CanRead) return null;

                var value = propertyEditor.info.GetValue(target, null);
                if (value == null)
                {
                    Editor.Label(propertyEditor.info.Name, "(" + propertyEditor.info.PropertyType.Name +  ") null");
                    return null;
                }

                // Base editors.
                if (value is bool)    return PropertyEditorBool(propertyEditor, (bool) value);
                if (value is byte)    return PropertyEditorUInt8(propertyEditor, (byte) value);
                if (value is decimal) return PropertyEditorDecimal(propertyEditor, (decimal) value);
                if (value is double)  return PropertyEditorDouble(propertyEditor, (double) value);
                if (value is float)   return PropertyEditorSingle(propertyEditor, (float) value);
                if (value is int)     return PropertyEditorInt32(propertyEditor, (int) value);
                if (value is long)    return PropertyEditorInt64(propertyEditor, (long) value);
                if (value is sbyte)   return PropertyEditorInt8(propertyEditor, (sbyte) value);
                if (value is short)   return PropertyEditorInt16(propertyEditor, (short) value);
                if (value is uint)    return PropertyEditorUInt32(propertyEditor, (uint) value);
                if (value is ulong)   return PropertyEditorUInt64(propertyEditor, (ulong) value);
                if (value is ushort)  return PropertyEditorUInt16(propertyEditor, (ushort) value);
                
                if (value is string)  return PropertyEditorString(propertyEditor, (string) value);
                if (value is Color)   return PropertyEditorColor(propertyEditor, (Color) value);
                if (value is Control) return PropertyEditorControl(propertyEditor, (Control) value);
                if (value is Enum)    return PropertyEditorEnum(propertyEditor, (Enum) value);
                
                // Complex types.
                if (propertyEditor.editor == null)
                {
                    propertyEditor.editor = new ObjectEditor(value, propertyEditor.info.Name);
                    propertyEditor.editor.backgroundRGB = MathHelper.Clamp(backgroundRGB - 25, 128, 255);
                }

                return propertyEditor.editor.Draw();
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

            public object PropertyEditorBool(PropertyEditor propertyEditor, bool value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var ebVal = Editor.BooleanField(propertyEditor.info.Name, value);
                if (ebVal.Changed)
                    propertyEditor.info.SetValue(target, ebVal.Value, null);

                return null;
            }
            public object PropertyEditorColor(PropertyEditor propertyEditor, Color value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                Editor.ColorField(propertyEditor.info.Name, value, c => propertyEditor.info.SetValue(target, c, null));
                return null;
            }
            public object PropertyEditorControl(PropertyEditor propertyEditor, Control value)
            {
                if (Editor.Button(propertyEditor.info.Name, value.GetType().Name))
                    return value;

                return null;
            }
            public object PropertyEditorDecimal(PropertyEditor propertyEditor, decimal value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var eiVal = Editor.TextField(propertyEditor.info.Name, value.ToString(CultureInfo.InvariantCulture));
                if (eiVal.Changed)
                {
                    decimal newValue;
                    decimal.TryParse(eiVal.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out newValue);
                    
                    propertyEditor.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorDouble(PropertyEditor propertyEditor, double value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var eiVal = Editor.TextField(propertyEditor.info.Name, value.ToString(CultureInfo.InvariantCulture));
                if (eiVal.Changed)
                {
                    double newValue;
                    double.TryParse(eiVal.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out newValue);
                    
                    propertyEditor.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorEnum(PropertyEditor propertyEditor, Enum value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }
                    
                var enumHasFlagAttribute = value.GetType().GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;
                var enumOptions = Enum.GetNames(value.GetType());

                if (enumHasFlagAttribute)
                {
                    // TODO: not gonna work with 'None' flag.
                    // https://forum.unity3d.com/threads/editorguilayout-enummaskfield-doesnt-use-enums-values.233332/
                    var eeVal = Editor.MaskField(propertyEditor.info.Name, Convert.ToInt32(value), enumOptions);
                    if (eeVal.Changed)
                        propertyEditor.info.SetValue(target, eeVal.Value, null);
                }
                else
                {
                    var eeVal = Editor.EnumField(propertyEditor.info.Name, (Enum)value);
                    if (eeVal.Changed)
                        propertyEditor.info.SetValue(target, eeVal.Value, null);
                }

                return null;
            }
            public object PropertyEditorInt8(PropertyEditor propertyEditor, sbyte value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var eiVal = Editor.IntField(propertyEditor.info.Name, value);
                if (eiVal.Changed)
                {
                    var newValue = (sbyte) MathHelper.Clamp(eiVal.Value[0], sbyte.MinValue, sbyte.MaxValue);
                    
                    propertyEditor.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorInt16(PropertyEditor propertyEditor, short value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var eiVal = Editor.IntField(propertyEditor.info.Name, value);
                if (eiVal.Changed)
                {
                    var newValue = (short) MathHelper.Clamp(eiVal.Value[0], short.MinValue, short.MaxValue);
                    
                    propertyEditor.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorInt32(PropertyEditor propertyEditor, int value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var eiVal = Editor.IntField(propertyEditor.info.Name, value);
                if (eiVal.Changed)
                    propertyEditor.info.SetValue(target, eiVal.Value[0], null);
                
                return null;
            }
            public object PropertyEditorInt64(PropertyEditor propertyEditor, long value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var eiVal = Editor.TextField(propertyEditor.info.Name, value.ToString());
                if (eiVal.Changed)
                {
                    long newValue;
                    long.TryParse(eiVal.Value, out newValue);
                    
                    propertyEditor.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorSingle(PropertyEditor propertyEditor, float value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var eiVal = Editor.TextField(propertyEditor.info.Name, value.ToString(CultureInfo.InvariantCulture));
                if (eiVal.Changed)
                {
                    float newValue;
                    float.TryParse(eiVal.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out newValue);
                    
                    propertyEditor.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorString(PropertyEditor propertyEditor, string value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var esVal = Editor.TextField(propertyEditor.info.Name, value);
                if (esVal.Changed)
                    propertyEditor.info.SetValue(target, esVal.Value, null);
                
                return null;
            }
            public object PropertyEditorUInt8(PropertyEditor propertyEditor, byte value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var eiVal = Editor.IntField(propertyEditor.info.Name, value);
                if (eiVal.Changed)
                {
                    var newValue = (byte) MathHelper.Clamp(eiVal.Value[0], byte.MinValue, byte.MaxValue);
                    
                    propertyEditor.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorUInt16(PropertyEditor propertyEditor, ushort value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var eiVal = Editor.IntField(propertyEditor.info.Name, value);
                if (eiVal.Changed)
                {
                    var newValue = (ushort) MathHelper.Clamp(eiVal.Value[0], ushort.MinValue, ushort.MaxValue);
                    
                    propertyEditor.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorUInt32(PropertyEditor propertyEditor, uint value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var eiVal = Editor.TextField(propertyEditor.info.Name, value.ToString());
                if (eiVal.Changed)
                {
                    uint newValue;
                    uint.TryParse(eiVal.Value, out newValue);
                    
                    propertyEditor.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
            public object PropertyEditorUInt64(PropertyEditor propertyEditor, ulong value)
            {
                if (!propertyEditor.canSet)
                {
                    Editor.Label(propertyEditor.info.Name, value);
                    return null;
                }

                var eiVal = Editor.TextField(propertyEditor.info.Name, value.ToString());
                if (eiVal.Changed)
                {
                    ulong newValue;
                    ulong.TryParse(eiVal.Value, out newValue);
                    
                    propertyEditor.info.SetValue(target, newValue, null);
                }
                
                return null;
            }
        }
    }
}
