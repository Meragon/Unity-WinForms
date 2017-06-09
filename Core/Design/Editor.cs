using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Unity.API;
using UnityEngine;

namespace System.Drawing
{
    public class Editor
    {
        private static float _width { get; set; }
        private static float _nameWidth { get; set; }
        private static float _contentWidth { get; set; }
        private static readonly string[] boolOnOff = { "On", "Off" };

        public static bool WinFormsCompatible { get; set; }

        public static void BeginGroup(float width)
        {
            _width = width;
            _nameWidth = 160;
            _contentWidth = width - _nameWidth;

            UnityEngine.GUILayout.BeginVertical("Box");
        }
        public static void BeginHorizontal()
        {
            if (_width > 0)
                UnityEngine.GUILayout.BeginHorizontal(UnityEngine.GUILayout.Width(_width));
            else
                UnityEngine.GUILayout.BeginHorizontal();
        }
        public static void BeginVertical()
        {
            UnityEngine.GUILayout.BeginVertical();
        }
        public static void BeginVertical(string style)
        {
            UnityEngine.GUILayout.BeginVertical(style);
        }
        public static void BeginVertical(GUIStyle style)
        {
            UnityEngine.GUILayout.BeginVertical(style);
        }
        public static void EndGroup()
        {
            UnityEngine.GUILayout.EndVertical();
        }
        public static void EndHorizontal()
        {
            UnityEngine.GUILayout.EndHorizontal();
        }
        public static void EndVertical()
        {
            UnityEngine.GUILayout.EndVertical();
        }

        public static EditorValue<bool> BooleanField(string name, bool value)
        {
            UnityEngine.GUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label(name + ":", UnityEngine.GUILayout.Width(_nameWidth));
            var boolBuffer = UnityEngine.GUILayout.Toolbar(value ? 0 : 1, boolOnOff, UnityEngine.GUILayout.Width(_contentWidth)) != 1;
            UnityEngine.GUILayout.EndHorizontal();

            return new EditorValue<bool>(boolBuffer, boolBuffer != value);
        }
        public static bool Button(string name, string text)
        {
            if (name == null)
                name = "null";

            UnityEngine.GUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label(name + ":", UnityEngine.GUILayout.Width(_nameWidth));
            var boolResult = UnityEngine.GUILayout.Button(text, UnityEngine.GUILayout.Width(_contentWidth));
            UnityEngine.GUILayout.EndHorizontal();

            return boolResult;
        }
        public static bool Button(string text)
        {
            return UnityEngine.GUILayout.Button(text);
        }
        public static bool Button(string text, int width)
        {
            return UnityEngine.GUILayout.Button(text, UnityEngine.GUILayout.Width(width));
        }
        public static void ColorField(string name, Color value, Action<Color> setColor)
        {
            UnityEngine.GUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label(name + ":", UnityEngine.GUILayout.Width(_nameWidth));
            Color colorBuffer = value;
            if (WinFormsCompatible)
            {
                if (Button(value.ToString()))
                {
                    ColorPicker colorPicker = new ColorPicker();
                    ColorPickerForm colorForm = new ColorPickerForm(colorPicker);
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
                colorBuffer = UnityEditor.EditorGUILayout.ColorField(value.ToUnityColor(), UnityEngine.GUILayout.Width(_contentWidth)).ToColor();
                if (colorBuffer != value && setColor != null)
                    setColor.Invoke(colorBuffer);
            }
#endif
            UnityEngine.GUILayout.EndHorizontal();
        }
        public static EditorValue<Enum> EnumField(string name, Enum value)
        {
            UnityEngine.GUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label(name + ":", UnityEngine.GUILayout.Width(_nameWidth));
            var enumBuffer = value;
            if (WinFormsCompatible)
            {

            }
#if UNITY_EDITOR
            else
                enumBuffer = UnityEditor.EditorGUILayout.EnumPopup(value, UnityEngine.GUILayout.Width(_contentWidth));
#endif
            UnityEngine.GUILayout.EndHorizontal();

            return new EditorValue<Enum>(enumBuffer, enumBuffer != value);
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
#endif
            return false;
        }
        public static void Header(string text)
        {
            UnityEngine.GUI.skin.label.fontStyle = UnityEngine.FontStyle.Bold;
            UnityEngine.GUI.skin.label.alignment = UnityEngine.TextAnchor.MiddleCenter;
            UnityEngine.GUI.Label(new UnityEngine.Rect(0, 0, _width, 36), text);

            UnityEngine.GUI.skin.label.fontStyle = UnityEngine.FontStyle.Normal;
            UnityEngine.GUI.skin.label.alignment = UnityEngine.TextAnchor.UpperLeft;
        }
        public static void Label(object value)
        {
            if (value != null)
                UnityEngine.GUILayout.Label(value.ToString());
        }
        public static void Label(string name, object value)
        {
            string valueString = "null";
            if (value != null) valueString = value.ToString();
            UnityEngine.GUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label(name + ":", UnityEngine.GUILayout.Width(_nameWidth));
            UnityEngine.GUILayout.Label(valueString, UnityEngine.GUILayout.Width(_contentWidth));
            UnityEngine.GUILayout.EndHorizontal();
        }
        public static EditorValue<int[]> IntField(string name, params int[] value)
        {
            UnityEngine.GUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label(name + ":", UnityEngine.GUILayout.Width(_nameWidth));

            bool changed = false;
            int[] intBuffer = new int[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                if (WinFormsCompatible)
                {
                    var valueBuffer = UnityEngine.GUILayout.TextField(value[i].ToString(), UnityEngine.GUILayout.Width(_contentWidth / value.Length));
                    int.TryParse(valueBuffer, out intBuffer[i]);
                }
#if UNITY_EDITOR
                else
                    intBuffer[i] = UnityEditor.EditorGUILayout.IntField(value[i], UnityEngine.GUILayout.Width(_contentWidth / value.Length - value.Length));
#endif
                if (intBuffer[i] != value[i])
                    changed = true;
            }
            UnityEngine.GUILayout.EndHorizontal();

            return new EditorValue<int[]>(intBuffer, changed);
        }
        public static void NewLine(int cnt)
        {
            for (int i = 0; i < cnt; i++)
                UnityEngine.GUILayout.Label("");
        }
        public static EditorValue<UnityEngine.Object> ObjectField(string name, UnityEngine.Object value, Type type)
        {
            UnityEngine.GUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label(name + ":", UnityEngine.GUILayout.Width(_nameWidth));
            UnityEngine.Object objectBuffer = value;
            if (WinFormsCompatible)
            {

            }
#if UNITY_EDITOR
            else
                objectBuffer = UnityEditor.EditorGUILayout.ObjectField(value, type, UnityEngine.GUILayout.Width(_contentWidth));
#endif
            UnityEngine.GUILayout.EndHorizontal();

            return new EditorValue<UnityEngine.Object>(objectBuffer, objectBuffer != value);
        }
        public static EditorValue<int> Popup(string name, int index, string[] values)
        {
            UnityEngine.GUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label(name + ":", UnityEngine.GUILayout.Width(_nameWidth));
            int intBuffer = 0;
            if (WinFormsCompatible)
            {

            }
#if UNITY_EDITOR
            else
                intBuffer = UnityEditor.EditorGUILayout.Popup(index, values, UnityEngine.GUILayout.Width(_contentWidth - 8));
#endif
            UnityEngine.GUILayout.EndHorizontal();

            return new EditorValue<int>(intBuffer, intBuffer != index);
        }
        public static EditorValue<float> Slider(string name, float value, float min, float max)
        {
            UnityEngine.GUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label(name + ":", UnityEngine.GUILayout.Width(_nameWidth));
            float floatBuffer = value;
            if (WinFormsCompatible)
            {
                floatBuffer = UnityEngine.GUILayout.HorizontalSlider(value, min, max, UnityEngine.GUILayout.Width(_contentWidth - 96));
                UnityEngine.GUILayout.TextField(floatBuffer.ToString(), UnityEngine.GUILayout.Width(32));
            }
#if UNITY_EDITOR
            else
                floatBuffer = UnityEditor.EditorGUILayout.Slider(value, min, max, UnityEngine.GUILayout.Width(_contentWidth));
#endif
            UnityEngine.GUILayout.EndHorizontal();

            return new EditorValue<float>(floatBuffer, floatBuffer != value);
        }
        public static EditorValue<string> TextField(string name, string value)
        {
            UnityEngine.GUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label(name + ":", UnityEngine.GUILayout.Width(_nameWidth));
            var textBuffer = UnityEngine.GUILayout.TextField(value, UnityEngine.GUILayout.Width(_contentWidth));
            UnityEngine.GUILayout.EndHorizontal();

            return new EditorValue<string>(textBuffer, textBuffer != value);
        }
        public static bool Toggle(string name, bool value)
        {
            return UnityEngine.GUILayout.Toggle(value, name, UnityEngine.GUILayout.Width(_width));
        }
    }

    public struct EditorValue<T>
    {
        private bool _changed;
        private T _value;

        public bool Changed { get { return _changed; } }
        public T Value { get { return _value; } }

        public EditorValue(T value, bool changed)
        {
            _value = value;
            _changed = changed;
        }

        public static bool operator ==(EditorValue<T> left, EditorValue<T> right)
        {
            return left.Value.Equals(right.Value);
        }
        public static bool operator ==(EditorValue<T> left, T right)
        {
            return left.Value.Equals(right);
        }
        public static bool operator !=(EditorValue<T> left, T right)
        {
            return !left.Value.Equals(right);
        }
        public static bool operator !=(EditorValue<T> left, EditorValue<T> right)
        {
            return !left.Value.Equals(right.Value);
        }
        public static implicit operator T(EditorValue<T> value)
        {
            return value.Value;
        }
        public static implicit operator EditorValue<T>(T value)
        {
            return new EditorValue<T>(value, true);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
