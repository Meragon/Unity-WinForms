using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    [Serializable]
    public class Button : Control, IButtonControl
    {
        internal Color currentBackColor;
        private float cBackColorA;
        private float cBackColorR;
        private float cBackColorG;
        private float cBackColorB;
        protected Pen borderPen;
        private DialogResult dialogResult;

        private bool _toggleEditor = true;

        public virtual DialogResult DialogResult
        {
            get { return dialogResult; }
            set { dialogResult = value; }
        }
        public Color BorderColor { get; set; }
        public Color BorderDisableColor { get; set; }
        public Color BorderHoverColor { get; set; }
        public Color BorderSelectColor { get; set; }
        public Color DisableColor { get; set; }
        public Color HoverColor { get; set; }
        public Bitmap Image { get; set; }
        public Bitmap ImageHover { get; set; }
        public Color ImageColor { get; set; }
        public Color ImageHoverColor { get; set; }
        public ContentAlignment TextAlign { get; set; }
        
        public bool UseVisualStyleBackColor;

        public Button()
        {
            BackColor = Color.FromArgb(234, 234, 234);
            BackgroundImageLayout = ImageLayout.Center;
            BorderColor = Color.FromArgb(172, 172, 172);
            BorderDisableColor = Color.FromArgb(217, 217, 217);
            BorderHoverColor = Color.FromArgb(126, 180, 234);
            BorderSelectColor = Color.FromArgb(51, 153, 255);
            CanSelect = true;
            DisableColor = Color.FromArgb(239, 239, 239);
            ForeColor = Color.FromArgb(64, 64, 64);
            ImageColor = Color.White;
            ImageHoverColor = Color.White;
            HoverColor = Color.FromArgb(223, 238, 252);
            TextAlign = ContentAlignment.MiddleCenter;
            Size = new Drawing.Size(75, 23);

            borderPen = new Pen(BorderColor);
            currentBackColor = BackColor;
            cBackColorA = currentBackColor.A;
            cBackColorR = currentBackColor.R;
            cBackColorG = currentBackColor.G;
            cBackColorB = currentBackColor.B;
        }

        public void NotifyDefault(bool value)
        {
            // ?
        }
        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }

        protected override void OnClick(EventArgs e)
        {
            Form form = FindFormInternal();
            if (form != null)
            {
                form.DialogResult = dialogResult;
                if (form.AcceptButton == this && form.dialog)
                    form.Close();
            }

            base.OnClick(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == UnityEngine.KeyCode.Space || e.KeyCode == UnityEngine.KeyCode.Return)
                PerformClick();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Back.
            if (Enabled)
            {
                if (Hovered == false)
                    MathHelper.ColorLerp(BackColor, 5, ref cBackColorA, ref cBackColorR, ref cBackColorG, ref cBackColorB);
                else
                    MathHelper.ColorLerp(HoverColor, 5, ref cBackColorA, ref cBackColorR, ref cBackColorG, ref cBackColorB);
            }
            else
                MathHelper.ColorLerp(DisableColor, 5, ref cBackColorA, ref cBackColorR, ref cBackColorG, ref cBackColorB);

            currentBackColor = Color.FromArgb((int)cBackColorA, (int)cBackColorR, (int)cBackColorG, (int)cBackColorB);

            g.uwfFillRectangle(currentBackColor, 0, 0, Width, Height);

            // Border.
            if (Enabled == false)
                borderPen.Color = BorderDisableColor;
            else if (Hovered)
                borderPen.Color = BorderHoverColor;
            else if (Focused)
                borderPen.Color = BorderSelectColor;
            else
                borderPen.Color = BorderColor;

            g.DrawRectangle(borderPen, 0, 0, Width, Height);

            if (Image != null && Image.uTexture != null)
            {
                var imageToPaint = Image;
                var imageColorToPaint = ImageColor;
                if (Hovered)
                {
                    if (ImageHover != null)
                        imageToPaint = ImageHover;
                    imageColorToPaint = ImageHoverColor;
                }
                switch (BackgroundImageLayout)
                {
                    default:
                    case ImageLayout.None:
                        g.uwfDrawImage(imageToPaint, imageColorToPaint, 0, 0, imageToPaint.Width, imageToPaint.Height);
                        break;
                    case ImageLayout.Center:
                        g.uwfDrawImage(imageToPaint, imageColorToPaint,
                            Width / 2 - imageToPaint.Width / 2,
                            Height / 2 - imageToPaint.Height / 2,
                            imageToPaint.Width,
                            imageToPaint.Height);
                        break;
                    case ImageLayout.Stretch:
                        g.uwfDrawImage(imageToPaint, imageColorToPaint, 0, 0, Width, Height);
                        break;
                    case ImageLayout.Zoom:
                        // TODO: not working.
                        break;
                }
            }
            var textColor = ForeColor;
            if (Enabled == false) textColor = ForeColor + Color.FromArgb(0, 128, 128, 128);
            g.uwfDrawString(Text, Font, textColor,
                    Padding.Left,
                    Padding.Top,
                    Width - Padding.Left - Padding.Right,
                    Height - Padding.Top - Padding.Bottom, TextAlign);
        }
        protected override object uwfOnPaintEditor(float width)
        {
            var control = base.uwfOnPaintEditor(width);

            Editor.BeginVertical();
            Editor.NewLine(1);

            _toggleEditor = Editor.Foldout("Button", _toggleEditor);
            if (_toggleEditor)
            {
                Editor.BeginGroup(width - 24);

                Editor.ColorField("      BackColor", BackColor, (c) => { BackColor = c; });
                Editor.ColorField("      HoverBorderColor", BorderHoverColor, (c) => { BorderHoverColor = c; });
                Editor.ColorField("      HoverColor", HoverColor, (c) => { HoverColor = c; });
                Editor.ColorField("      ImageColor", ImageColor, (c) => { ImageColor = c; });
                Editor.ColorField("      NormalBorderColor", BorderColor, (c) => { BorderColor = c; });

                var editorTextAlign = Editor.EnumField("      TextAlign", TextAlign);
                if (editorTextAlign.Changed) TextAlign = (ContentAlignment)editorTextAlign.Value;

                Editor.Label("      TextMargin", Padding);

                Editor.EndGroup();
            }
            Editor.EndVertical();

            return control;
        }
    }
}
