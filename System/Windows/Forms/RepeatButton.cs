namespace System.Windows.Forms
{
    internal class RepeatButton : Button
    {
        private bool mdown = false;
        private bool repeatClick = false;
        private float repeatCooldown = 0;
        private float repeatCooldownCurrent = 0;
        private bool repeatStarted;
        private float cooldownBetweenClicks = .1f;
        private float waitCooldown = .4f;

        public RepeatButton()
        {
            MouseHook.MouseUp += Owner_UpClick;
            Application.UpdateEvent += ApplicationOnUpdateEvent;
        }

        public float CooldownBetweenClicks
        {
            get { return cooldownBetweenClicks; }
            set { cooldownBetweenClicks = value; }
        }
        /// <summary>
        /// After first click.
        /// </summary>
        public float WaitCooldown
        {
            get { return waitCooldown; }
            set { waitCooldown = value; }
        }

        internal override void RaiseOnMouseClick(MouseEventArgs e)
        {
            // We want to prevent clicking a button after MouseUp event. So it wont be double clicking thing.
            if (repeatStarted == false)
                base.RaiseOnMouseClick(e);
        }

        protected override void Dispose(bool release_all)
        {
            MouseHook.MouseUp -= Owner_UpClick;
            Application.UpdateEvent -= ApplicationOnUpdateEvent;

            base.Dispose(release_all);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && e.Clicks >= 1)
                StartRepeat();
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            
            if (e.Button == MouseButtons.Left)
                StartRepeat();
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (mdown)
            {
                repeatClick = true;
                repeatCooldown = waitCooldown;
                repeatCooldownCurrent = repeatCooldown;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            repeatClick = false;
        }

        private void Owner_UpClick(object sender, MouseEventArgs e)
        {
            mdown = false;
            repeatClick = false;
            repeatStarted = false;
        }
        private void ApplicationOnUpdateEvent()
        {
            if (!repeatClick) return;

            if (repeatCooldownCurrent <= 0)
            {
                repeatStarted = true;

                PerformClick();

                repeatCooldown = cooldownBetweenClicks;
                repeatCooldownCurrent = repeatCooldown;
            }
            else
                repeatCooldownCurrent -= swfHelper.GetDeltaTime();
        }
        private void StartRepeat()
        {
            mdown = true;
            repeatClick = true;
            repeatCooldown = waitCooldown;
            repeatCooldownCurrent = repeatCooldown;
        }
    }
}
