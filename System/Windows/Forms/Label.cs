using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class Label : Control
    {
#if UNITY_EDITOR
        private bool _toggleEditor = true;
#endif

        public UnityEngine.Texture2D Image { get; set; }
        public ContentAlignment TextAlign { get; set; }

        public Label()
        {
            Size = new Drawing.Size(128, 20);
            TextAlign = ContentAlignment.TopLeft;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);
            if (Image != null)
                g.DrawTexture(Image, Width / 2 - Image.width / 2, Height / 2 - Image.height / 2, Image.width, Image.height);
            g.DrawString(Text, Font, new SolidBrush(ForeColor), 4, 0, Width - 8, Height, TextAlign);
        }
        protected override object OnPaintEditor(float width)
        {
            var control = base.OnPaintEditor(width);

#if UNITY_EDITOR
            Editor.BeginVertical();
            Editor.NewLine(1);

            _toggleEditor = Editor.Foldout("Label", _toggleEditor);
            if (_toggleEditor)
            {
                Editor.BeginGroup(width - 24);

                var editorImage = Editor.ObjectField("      Image", Image, typeof(UnityEngine.Texture2D));
                if (editorImage.Changed) Image = (UnityEngine.Texture2D)editorImage.Value;

                var editorTextAlign = Editor.EnumField("      TextAlign", TextAlign);
                if (editorTextAlign.Changed) TextAlign = (ContentAlignment)editorTextAlign.Value;

                Editor.EndGroup();
            }
            Editor.EndVertical();
#endif

            return control;
        }
    }
}
