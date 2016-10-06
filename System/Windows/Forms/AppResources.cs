using UnityEngine;
using System;
using System.Collections.Generic;

namespace System.Windows.Forms
{
    [Serializable]
    public class AppResources
    {
        public List<Font> Fonts;

        public ReservedResources Images;

        /// <summary>
        /// System resources.
        /// </summary>
        [Serializable]
        public struct ReservedResources
        {
            [Tooltip("Form resize icon")]
            public Texture2D ArrowDown;

            [Tooltip("Form resize icon, MonthCalendar, TabControl")]
            public Texture2D ArrowLeft;

            [Tooltip("Form resize icon, MonthCalendar, TabControl")]
            public Texture2D ArrowRight;

            [Tooltip("Form resize icon")]
            public Texture2D ArrowUp;

            public Texture2D Circle;

            [Tooltip("Checkbox, ToolStripItem")]
            public Texture2D Checked;

            [Tooltip("Form close button")]
            public Texture2D Close;

            [Tooltip("ComboBox, VScrollBar")]
            public Texture2D CurvedArrowDown;

            [Tooltip("HScrollBar")]
            public Texture2D CurvedArrowLeft;

            [Tooltip("HScrollBar")]
            public Texture2D CurvedArrowRight;

            [Tooltip("VScrollBar")]
            public Texture2D CurvedArrowUp;

            [Tooltip("ToolStripDropDown")]
            public Texture2D DropDownRightArrow;

            [Tooltip("Form")]
            public Texture2D FormResize;

            [Tooltip("NumericUpDown")]
            public Texture2D NumericDown;

            [Tooltip("NumericUpDown")]
            public Texture2D NumericUp;
        }
    }
}
