using UnityEngine;
using System;
using System.Collections.Generic;

namespace System.Windows.Forms
{
    [Serializable]
    public class AppResources
    {
        public List<Font> Fonts;

        public ReservedResources Reserved;

        /// <summary>
        /// System resources.
        /// </summary>
        [Serializable]
        public struct ReservedResources
        {
            public Texture2D ArrowDown;
            public Texture2D ArrowLeft;
            public Texture2D ArrowRight;
            public Texture2D ArrowUp;
            public Texture2D Circle;
            public Texture2D Checked;
            public Texture2D Close;
            public Texture2D ComboBoxArrow;
            public Texture2D DropDownRightArrow_Black;
            public Texture2D DropDownRightArrow_White;
            public Texture2D FormResize;
            public Texture2D NumericUp;
            public Texture2D NumericDown;
            public Texture2D Roundrect;
        }
    }
}
