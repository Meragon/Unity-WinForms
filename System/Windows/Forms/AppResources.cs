using UnityEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using Font = UnityEngine.Font;
using Image = UnityEngine.Texture2D;

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
            public Image ArrowDown;

            [Tooltip("Form resize icon, MonthCalendar, TabControl")]
            public Image ArrowLeft;

            [Tooltip("Form resize icon, MonthCalendar, TabControl")]
            public Image ArrowRight;

            [Tooltip("Form resize icon")]
            public Image ArrowUp;

            public Image Circle;

            [Tooltip("Checkbox, ToolStripItem")]
            public Image Checked;

            [Tooltip("Form close button")]
            public Image Close;

            public CursorImages Cursors;

            [Tooltip("ComboBox, VScrollBar")]
            public Image CurvedArrowDown;

            [Tooltip("HScrollBar")]
            public Image CurvedArrowLeft;

            [Tooltip("HScrollBar")]
            public Image CurvedArrowRight;

            [Tooltip("VScrollBar")]
            public Image CurvedArrowUp;

            [Tooltip("ToolStripDropDown")]
            public Image DropDownRightArrow;

            [Tooltip("Form")]
            public Image FormResize;

            [Tooltip("NumericUpDown")]
            public Image NumericDown;

            [Tooltip("NumericUpDown")]
            public Image NumericUp;

            [Tooltip("Tree")]
            public Image TreeNodeCollapsed;

            [Tooltip("Tree")]
            public Image TreeNodeExpanded;
        }

        [Serializable]
        public struct CursorImages
        {
            [Tooltip("Leave this field empty if you don't want to use your own cursor.")]
            public Image Default;

            public Image Hand;
            public Image Help;
            public Image HSplit;
            public Image SizeAll;
            public Image SizeNESW;
            public Image SizeNS;
            public Image SizeNWSE;
            public Image SizeWE;
            public Image VSplit;
        }
    }

    public class AppGdiImages
    {
        public Bitmap ArrowDown;
        public Bitmap ArrowLeft;
        public Bitmap ArrowRight;
        public Bitmap ArrowUp;
        public Bitmap Circle;
        public Bitmap Checked;
        public Bitmap Close;
        public CursorImages Cursors = new CursorImages();
        public Bitmap CurvedArrowDown;
        public Bitmap CurvedArrowLeft;
        public Bitmap CurvedArrowRight;
        public Bitmap CurvedArrowUp;
        public Bitmap DropDownRightArrow;
        public Bitmap FormResize;
        public Bitmap NumericDown;
        public Bitmap NumericUp;
        public Bitmap TreeNodeCollapsed;
        public Bitmap TreeNodeExpanded;

        public struct CursorImages
        {
            public Bitmap Default;

            public Bitmap Hand;
            public Bitmap Help;
            public Bitmap HSplit;
            public Bitmap SizeAll;
            public Bitmap SizeNESW;
            public Bitmap SizeNS;
            public Bitmap SizeNWSE;
            public Bitmap SizeWE;
            public Bitmap VSplit;
        }
    }
}
