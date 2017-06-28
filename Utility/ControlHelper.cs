namespace System.Windows.Forms
{
    using System.Drawing;

    /// <summary>
    /// Additional methods for designing and managing controls.
    /// </summary>
    public static class ControlHelper
    {
        public static void AddDialogButtons(this Control f, Control buttonOk, Control buttonCancel, params Control[] additionalButtons)
        {
            if (buttonOk == null) buttonOk = new Button();
            if (buttonCancel == null) buttonCancel = new Button();

            buttonCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            buttonCancel.Location = new Point(f.Width - buttonCancel.Width - 12, f.Height - buttonCancel.Height - 15);
            buttonCancel.Text = "Cancel";
            f.Controls.Add(buttonCancel);

            buttonOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            buttonOk.Location = new Point(buttonCancel.Location.X - buttonOk.Width - 9, buttonCancel.Location.Y);
            buttonOk.Text = "Ok";
            f.Controls.Add(buttonOk);

            if (additionalButtons != null)
            {
                Point lastLocation = buttonOk.Location;

                for (int i = 0; i < additionalButtons.Length; i++)
                {
                    if (additionalButtons[i] == null)
                        additionalButtons[i] = new Button();

                    additionalButtons[i].Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
                    additionalButtons[i].Location = new Point(lastLocation.X - additionalButtons[i].Width - 9, lastLocation.Y);
                    lastLocation = additionalButtons[i].Location;
                    f.Controls.Add(additionalButtons[i]);
                }
            }
        }
        public static void FillToBottom(this Control control, int offset = 0)
        {
            if (control.Parent == null) return;
            control.Height = control.Parent.Height - control.Location.Y - offset;
        }
        public static void FillToRight(this Control control, int offset = 0)
        {
            if (control.Parent == null) return;
            control.Width = control.Parent.Width - control.Location.X - offset;
        }
        public static void OpenUrl(this Button button, string url)
        {
#if UNITY_WEBGL || UNITY_WEBPLAYER
            UnityEngine.Application.ExternalEval("window.open(\"" +  url + "\")"); // with popup blocking...
#endif

#if UNITY_STANDALONE
            UnityEngine.Application.OpenURL(url);
#endif
        }
        public static void ToCenter(this Form f)
        {
            f.Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - f.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - f.Height) / 2);
        }
    }
}
