namespace System.Windows.Forms
{
    using System.Collections.Generic;

    [Serializable]
    public class ContainerControl : ScrollableControl, IContainerControl
    {
        private Control selectedControl;

        public Control ActiveControl { get; set; }

        public bool ActivateControl(Control active)
        {
            throw new NotImplementedException();
        }
        
        internal void RaiseProcessTabKey(bool forward, Control selectedControl)
        {
            this.selectedControl = selectedControl;
            ProcessTabKey(forward);
        }

        protected virtual bool ProcessTabKey(bool forward)
        {
            if (forward)
                NextTabControl(selectedControl);
            else
                PrevTabControl(selectedControl);

            return true;
        }

        // TODO: far from perfect. Need to remember current focused control and it's tab index.
        internal static void FillListWithPossibleControls(Control control, List<Control> list)
        {
            for (int i = 0; i < control.Controls.Count; i++)
            {
                var c = control.Controls[i];
                if (!c.Visible || !c.Enabled || !c.TabStop || c.IsDisposed) 
                    continue;
                
                list.Add(c);
                FillListWithPossibleControls(c, list);
            }
        }
        
        private static int TabComparison(Control c1, Control c2)
        {
            if (c1.TabIndex >= 0 || c2.TabIndex >= 0)
                return c1.TabIndex.CompareTo(c2.TabIndex);

            var c1Location = c1.Location;
            var c2Location = c2.Location;

            if (c1Location.Y != c2Location.Y)
                return c1Location.Y.CompareTo(c2Location.Y);
            if (c1Location.X == c2Location.X)
                return 0;

            return c1Location.X.CompareTo(c2Location.X);
        }
        private void NextTabControl(Control control)
        {
            var controlForm = Application.GetRootControl(control) as Form;
            if (controlForm == null || controlForm.Controls.Count <= 0) return;

            var formControls = new List<Control>();
            
            FillListWithPossibleControls(controlForm, formControls);

            var possibleControls = formControls.FindAll(x => x.CanSelect);
            if (possibleControls.Count == 0) return;

            for (int i = 0; i < possibleControls.Count; i++)
                possibleControls[i].uwfDrawTabDots = true;

            possibleControls.Sort(TabComparison);

            int controlIndex = possibleControls.FindIndex(x => x == control);

            var nextControlIndex = controlIndex + 1;
            if (nextControlIndex >= possibleControls.Count)
                nextControlIndex = 0;

            var nextControl = possibleControls[nextControlIndex];
            nextControl.Focus();
            
            EnsureVisible(nextControl);
        }
        private void PrevTabControl(Control control)
        {
            var controlForm = Application.GetRootControl(control) as Form;
            if (controlForm == null || controlForm.Controls.Count <= 0) return;

            var formControls = new List<Control>();
            
            FillListWithPossibleControls(controlForm, formControls);

            var possibleControls = formControls.FindAll(x => x.CanSelect);
            if (possibleControls.Count == 0) return;
            
            for (int i = 0; i < possibleControls.Count; i++)
                possibleControls[i].uwfDrawTabDots = true;

            possibleControls.Sort(TabComparison);

            int controlIndex = possibleControls.FindIndex(x => x == control);

            var nextControlIndex = controlIndex - 1;
            if (nextControlIndex < 0)
                nextControlIndex = possibleControls.Count - 1;

            var nextControl = possibleControls[nextControlIndex];
            nextControl.Focus();
            
            EnsureVisible(nextControl);
        }
    }
}
