namespace System.Windows.Forms
{
    using System.Collections;
    using System.Drawing;

    public sealed class MdiClient : Control
    {
        private readonly ArrayList items = new ArrayList();

        public MdiClient()
        {
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            BackColor = SystemColors.AppWorkspace;
            SetStyle(ControlStyles.Selectable, false);
        }

        public Form[] MdiChildren
        {
            get
            {
                var temp = new Form[items.Count];
                items.CopyTo(temp, 0);
                return temp;
            }
        }

        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new ControlCollection(this);
        }

        public new class ControlCollection : Control.ControlCollection
        {
            private readonly MdiClient owner;

            public ControlCollection(MdiClient owner) : base(owner)
            {
                this.owner = owner;
            }

            public override void Add(Control value)
            {
                if (value == null)
                    return;

                var form = value as Form;
                if (form == null || !form.IsMdiChild)
                    throw new ArgumentException("form");

                owner.items.Add(form);

                base.Add(value);
            }
            public override void Remove(Control item)
            {
                owner.items.Remove(item);

                base.Remove(item);
            }
        }
    }
}
