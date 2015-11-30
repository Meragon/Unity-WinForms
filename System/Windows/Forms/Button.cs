using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class Button : Control
    {
        internal ColorF currentBackColor;
        private Color _normalColor;

#if UNITY_EDITOR
        private bool _toggleEditor = true;
#endif

        public virtual Color HoverBorderColor { get; set; }
        public virtual Color HoverColor { get; set; }
        public UnityEngine.Texture2D Image { get; set; }
        public UnityEngine.Texture2D ImageHover { get; set; }
        public Color ImageColor { get; set; }
        public virtual Color NormalBorderColor { get; set; }
        public virtual Color NormalColor
        {
            get { return _normalColor; }
            set
            {
                _normalColor = value;
                currentBackColor = value;
            }
        }
        public ContentAlignment TextAlign { get; set; }

        public Button()
        {
            BackgroundImageLayout = ImageLayout.Center;
            Font = new Drawing.Font("Arial", 12);
            ForeColor = Color.FromArgb(64, 64, 64);
            ImageColor = Color.White;
            NormalColor = Color.FromArgb(234, 234, 234);
            NormalBorderColor = Color.FromArgb(172, 172, 172);
            HoverColor = Color.FromArgb(223, 238, 252);
            HoverBorderColor = Color.FromArgb(126, 180, 234);
            TextAlign = ContentAlignment.MiddleCenter;
            Size = new Drawing.Size(75, 23);

            currentBackColor = NormalColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (!Hovered || !Enabled)
            {
                currentBackColor = MathCustom.ColorLerp(currentBackColor, NormalColor, 5);

                g.FillRectangle(new SolidBrush(currentBackColor), 0, 0, Width, Height);
                g.DrawRectangle(new Pen(NormalBorderColor), 0, 0, Width, Height);
            }
            else
            {
                currentBackColor = MathCustom.ColorLerp(currentBackColor, HoverColor, 5);

                g.FillRectangle(new SolidBrush(currentBackColor), 0, 0, Width, Height);
                g.DrawRectangle(new Pen(HoverBorderColor), 0, 0, Width, Height);
            }

            SolidBrush textBrush = new SolidBrush(ForeColor);
            if (!Enabled) textBrush.Color = ForeColor + Color.FromArgb(0, 128, 128, 128);
            if (Image != null)
            {
                switch (BackgroundImageLayout)
                {
                    default:
                    case ImageLayout.None:
                        g.DrawTexture(Hovered && ImageHover != null ? ImageHover : Image, 0, 0, Image.width, Image.height, ImageColor);
                        break;
                    case ImageLayout.Center:
                        g.DrawTexture(Hovered && ImageHover != null ? ImageHover : Image, Width / 2 - Image.width / 2, Height / 2 - Image.height / 2, Image.width, Image.height, ImageColor);
                        break;
                    case ImageLayout.Stretch:
                        g.DrawTexture(Hovered && ImageHover != null ? ImageHover : Image, 0, 0, Width, Height, ImageColor);
                        break;
                    case ImageLayout.Zoom:
                        // TODO: not working.
                        break;
                }
            }
            g.DrawString(Text, Font, textBrush,
                    Padding.Left,
                    Padding.Top,
                    Width - Padding.Left - Padding.Right,
                    Height - Padding.Top - Padding.Bottom, TextAlign);
        }
        protected override object OnPaintEditor(float width)
        {
            var control = base.OnPaintEditor(width);

#if UNITY_EDITOR
            Editor.BeginVertical();
            Editor.NewLine(1);

            _toggleEditor = Editor.Foldout("Button", _toggleEditor);
            if (_toggleEditor)
            {
                Editor.BeginGroup(width - 24);

                var editorHoverBorderColor = Editor.ColorField("      HoverBorderColor", HoverBorderColor);
                if (editorHoverBorderColor.Changed) HoverBorderColor = editorHoverBorderColor;

                var editorHoverColor = Editor.ColorField("      HoverColor", HoverColor);
                if (editorHoverColor.Changed) HoverColor = editorHoverColor;

                var editorImage = Editor.ObjectField("      Image", Image, typeof(UnityEngine.Texture2D));
                if (editorImage.Changed) Image = (UnityEngine.Texture2D)editorImage.Value;

                var editorImageHover = Editor.ObjectField("      ImageHover", ImageHover, typeof(UnityEngine.Texture2D));
                if (editorImageHover.Changed) ImageHover = (UnityEngine.Texture2D)editorImageHover.Value;

                var editorImageColor = Editor.ColorField("      ImageColor", ImageColor);
                if (editorImageColor.Changed) ImageColor = editorImageColor;

                var editorNormalBorderColor = Editor.ColorField("      NormalBorderColor", NormalBorderColor);
                if (editorNormalBorderColor.Changed) NormalBorderColor = editorNormalBorderColor;

                var editorNormalColor = Editor.ColorField("      NormalColor", NormalColor);
                if (editorNormalColor.Changed) NormalColor = editorNormalColor;

                var editorTextAlign = Editor.EnumField("      TextAlign", TextAlign);
                if (editorTextAlign.Changed) TextAlign = (ContentAlignment)editorTextAlign.Value;

                Editor.Label("      TextMargin", Padding);

                Editor.EndGroup();
            }
            Editor.EndVertical();
#endif

            return control;
        }
    }
}
