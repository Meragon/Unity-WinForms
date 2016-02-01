using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    public class Accordion : Control
    {
        public const int DefaultItemHeight = 23;
        public const int DefaultItemSpace = 4;

        public Color BorderColor { get; set; }
        public int ItemHeight { get; set; }
        public int ItemSpace { get; set; }

        public Accordion()
        {
            BorderColor = Color.Transparent;
            ItemHeight = DefaultItemHeight;
            ItemSpace = DefaultItemSpace;
        }

        public AccordionButton Add(string itemText, Control container)
        {
            int visibleItems = 0;
            for (int i = 0; i < Controls.Count; i++)
                if (Controls[i].Visible)
                    visibleItems++;

            AccordionButton itemButton = new AccordionButton(this);
            itemButton.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            itemButton.Location = new Point(0, visibleItems * (ItemHeight + ItemSpace));
            itemButton.Width = Width;
            itemButton.Text = itemText;

            container.Visible = false;

            Controls.Add(itemButton);
            Controls.Add(container);

            Refresh();
            return itemButton;
        }

        protected override void OnLatePaint(PaintEventArgs e)
        {
            base.OnLatePaint(e);

            e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, Width, Height);
        }
        public override void Refresh()
        {
            base.Refresh();

            int currentY = 0;
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is AccordionButton)
                {
                    Controls[i].Location = new Point(Controls[i].Location.X, currentY);
                    currentY += Controls[i].Height + ItemSpace;
                }
                else
                {
                    // Content.
                    var buttonCollapsed = ((AccordionButton)Controls[i - 1]).Collapsed;
                    if (buttonCollapsed)
                    {
                        Controls[i].Visible = false;
                    }
                    else
                    {
                        Controls[i].Visible = true;
                        Controls[i].Location = new Point(Controls[i].Location.X, currentY);
                        currentY += Controls[i].Height + ItemSpace;
                    }
                }
            }

            Height = currentY;
        }
    }

    public class AccordionButton : Button
    {
        private Accordion _owner;

        public bool Collapsed { get; set; }

        public AccordionButton(Accordion owner)
        {
            _owner = owner;

            Collapsed = true;
            Padding = new Forms.Padding(24, 0, 24, 0);
            TextAlign = ContentAlignment.MiddleLeft;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var image = Application.Resources.Reserved.ArrowDown;
            if (!Collapsed) image = Application.Resources.Reserved.ArrowUp;

            var imageRect = new Rectangle(8, Height / 2 - image.height / 2, image.width, image.height);
            e.Graphics.DrawTexture(image, imageRect);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            Collapsed = !Collapsed;
            _owner.Refresh();
        }
    }
}
