namespace Unity.Views
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    using Unity.API;

    using UnityEngine;

    public static class Editor
    {
        private static readonly string[] boolOnOff = { "On", "Off" };

        public static bool WinFormsCompatible { get; set; }

        private static float _width { get; set; }
        private static float _nameWidth { get; set; }
        private static float _contentWidth { get; set; }

        public static void BeginGroup(float width, string style = "Box")
        {
            _width = width;
            _nameWidth = 160;
            _contentWidth = width - _nameWidth;

            if (string.IsNullOrEmpty(style))
                GUILayout.BeginVertical();
            else
                GUILayout.BeginVertical(style);
        }
        public static void BeginHorizontal()
        {
            if (_width > 0)
                GUILayout.BeginHorizontal(GUILayout.Width(_width));
            else
                GUILayout.BeginHorizontal();
        }
        public static void BeginVertical()
        {
            GUILayout.BeginVertical();
        }
        public static void BeginVertical(string style)
        {
            if (string.IsNullOrEmpty(style))
                GUILayout.BeginVertical();
            else
                GUILayout.BeginVertical(style);
        }
        public static void BeginVertical(GUIStyle style)
        {
            GUILayout.BeginVertical(style);
        }
        public static void EndGroup()
        {
            GUILayout.EndVertical();
        }
        public static void EndHorizontal()
        {
            GUILayout.EndHorizontal();
        }
        public static void EndVertical()
        {
            GUILayout.EndVertical();
        }
        public static EditorValue<bool> BooleanField(string name, bool value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", GUILayout.Width(_nameWidth));
            var boolBuffer = GUILayout.Toolbar(value ? 0 : 1, boolOnOff, GUILayout.Width(_contentWidth)) != 1;
            GUILayout.EndHorizontal();

            return new EditorValue<bool>(boolBuffer, boolBuffer != value);
        }
        public static bool Button(string name, string text)
        {
            if (name == null)
                name = "null";

            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", GUILayout.Width(_nameWidth));
            var boolResult = GUILayout.Button(text, GUILayout.Width(_contentWidth));
            GUILayout.EndHorizontal();

            return boolResult;
        }
        public static bool Button(string text)
        {
            return GUILayout.Button(text);
        }
        public static bool Button(string text, int width)
        {
            return GUILayout.Button(text, GUILayout.Width(width));
        }
        public static void ColorField(string name, System.Drawing.Color value, Action<System.Drawing.Color> setColor)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", GUILayout.Width(_nameWidth));
            if (WinFormsCompatible)
            {
                if (Button(value.ToString()))
                {
                    var colorForm = new ColorPickerForm();
                    colorForm.Color = value;
                    colorForm.Show();
                    colorForm.ColorChanged += (s, a) =>
                    {
                        if (setColor != null)
                            setColor.Invoke(colorForm.Color);
                    };
                }
            }

#if UNITY_EDITOR
            else
            {
                var colorBuffer = UnityEditor.EditorGUILayout.ColorField(value.ToUnityColor(), GUILayout.Width(_contentWidth)).ToColor();
                if (colorBuffer != value && setColor != null)
                    setColor.Invoke(colorBuffer);
            }

#endif
            GUILayout.EndHorizontal();
        }
        public static EditorValue<Enum> EnumField(string name, Enum value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", GUILayout.Width(_nameWidth));
            var enumBuffer = value;
            if (WinFormsCompatible)
            {
            }

#if UNITY_EDITOR
            else
                enumBuffer = UnityEditor.EditorGUILayout.EnumPopup(value, GUILayout.Width(_contentWidth));
#endif
            GUILayout.EndHorizontal();

            return new EditorValue<Enum>(enumBuffer, !Equals(enumBuffer, value));
        }
        public static bool Foldout(string name, bool value)
        {
            if (WinFormsCompatible)
            {
                return Toggle(name, value);
            }

#if UNITY_EDITOR
            else
                return UnityEditor.EditorGUILayout.Foldout(value, name);
#else
            return false;
#endif
        }
        public static void Header(string text)
        {
            GUI.skin.label.fontStyle = UnityEngine.FontStyle.Bold;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0, 0, _width, 36), text);

            GUI.skin.label.fontStyle = UnityEngine.FontStyle.Normal;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
        }
        public static void Label(object value)
        {
            if (value != null)
                GUILayout.Label(value.ToString());
        }
        public static void Label(string name, object value)
        {
            string valueString = "null";
            if (value != null) valueString = value.ToString();
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", GUILayout.Width(_nameWidth));
            GUILayout.Label(valueString, GUILayout.Width(_contentWidth));
            GUILayout.EndHorizontal();
        }
        public static EditorValue<int[]> IntField(string name, params int[] value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", GUILayout.Width(_nameWidth));

            bool changed = false;
            int[] intBuffer = new int[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                if (WinFormsCompatible)
                {
                    var valueBuffer = GUILayout.TextField(value[i].ToString(), GUILayout.Width(_contentWidth / value.Length));
                    int.TryParse(valueBuffer, out intBuffer[i]);
                }

#if UNITY_EDITOR
                else
                    intBuffer[i] = UnityEditor.EditorGUILayout.IntField(value[i], GUILayout.Width(_contentWidth / value.Length - value.Length));
#endif
                if (intBuffer[i] != value[i])
                    changed = true;
            }

            GUILayout.EndHorizontal();

            return new EditorValue<int[]>(intBuffer, changed);
        }
        public static EditorValue<int> MaskField(string name, int value, string[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", GUILayout.Width(_nameWidth));
            var buffer = value;
            if (WinFormsCompatible)
            {
            }

#if UNITY_EDITOR
            else
                buffer = UnityEditor.EditorGUILayout.MaskField(value, options);
#endif
            GUILayout.EndHorizontal();

            return new EditorValue<int>(buffer, buffer != value);
        }
        public static void NewLine(int cnt)
        {
            for (int i = 0; i < cnt; i++)
                GUILayout.Label(string.Empty);
        }
        public static EditorValue<UnityEngine.Object> ObjectField(string name, UnityEngine.Object value, Type type)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", GUILayout.Width(_nameWidth));
            var objectBuffer = value;
            if (WinFormsCompatible)
            {
            }

#if UNITY_EDITOR
            else
                objectBuffer = UnityEditor.EditorGUILayout.ObjectField(value, type, true, GUILayout.Width(_contentWidth));
#endif
            GUILayout.EndHorizontal();

            return new EditorValue<UnityEngine.Object>(objectBuffer, objectBuffer != value);
        }
        public static EditorValue<int> Popup(string name, int index, string[] values)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", GUILayout.Width(_nameWidth));
            int intBuffer = 0;
            if (WinFormsCompatible)
            {
            }

#if UNITY_EDITOR
            else
                intBuffer = UnityEditor.EditorGUILayout.Popup(index, values, GUILayout.Width(_contentWidth - 8));
#endif
            GUILayout.EndHorizontal();

            return new EditorValue<int>(intBuffer, intBuffer != index);
        }
        public static EditorValue<float> Slider(string name, float value, float min, float max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", GUILayout.Width(_nameWidth));
            float floatBuffer = value;
            if (WinFormsCompatible)
            {
                floatBuffer = GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(_contentWidth - 96));
                GUILayout.TextField(floatBuffer.ToString(CultureInfo.InvariantCulture), GUILayout.Width(32));
            }

#if UNITY_EDITOR
            else
                floatBuffer = UnityEditor.EditorGUILayout.Slider(value, min, max, GUILayout.Width(_contentWidth));
#endif
            GUILayout.EndHorizontal();

            return new EditorValue<float>(floatBuffer, floatBuffer != value);
        }
        public static EditorValue<string> TextField(string name, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", GUILayout.Width(_nameWidth));
            if (value != null && value.Length > 1024)
            {
                GUILayout.TextField(value.Substring(0, 1024) + "...", GUILayout.Width(_contentWidth));
                GUILayout.EndHorizontal();
                return new EditorValue<string>(value, false);
            }
            var textBuffer = GUILayout.TextField(value, GUILayout.Width(_contentWidth));
            GUILayout.EndHorizontal();

            return new EditorValue<string>(textBuffer, textBuffer != value);
        }
        public static bool Toggle(string name, bool value)
        {
            return GUILayout.Toggle(value, name, GUILayout.Width(_width));
        }
        public static void SetBackColor(System.Drawing.Color color)
        {
            GUI.backgroundColor = color.ToUnityColor();
        }
    }
}
