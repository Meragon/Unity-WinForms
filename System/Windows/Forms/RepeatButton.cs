using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class RepeatButton : Button
    {
        private bool mdown = false;
        private bool repeatClick = false;
        private float repeatCooldown = 0;
        private float repeatCooldownCurrent = 0;
        private float repeatCooldownMin = .1f;
        private float repeatStartCooldown = .4f;
        
        public float CooldownBetweenClicks
        {
            get { return repeatCooldownMin; }
            set { repeatCooldownMin = value; }
        }
        /// <summary>
        /// After first click.
        /// </summary>
        public float WaitCooldown
        {
            get { return repeatStartCooldown; }
            set { repeatStartCooldown = value; }
        }

        public RepeatButton()
        {
            uwfAppOwner.UpClick += Owner_UpClick;
            uwfAppOwner.UpdateEvent += Owner_UpdateEvent;
        }

        private void Owner_UpClick(object sender, MouseEventArgs e)
        {
            mdown = false;
            repeatClick = false;
        }
        private void Owner_UpdateEvent()
        {
            if (repeatClick)
            {
                if (repeatCooldownCurrent <= 0)
                {
                    PerformClick();

                    repeatCooldown = repeatCooldownMin;
                    repeatCooldownCurrent = repeatCooldown;
                }
                else
                    repeatCooldownCurrent -= Application.DeltaTime;
            }
        }
        private void StartRepeat()
        {
            mdown = true;
            repeatClick = true;
            repeatCooldown = repeatStartCooldown;
            repeatCooldownCurrent = repeatCooldown;
        }

        public override void Dispose()
        {
            uwfAppOwner.UpClick -= Owner_UpClick;

            base.Dispose();
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
                repeatCooldown = repeatStartCooldown;
                repeatCooldownCurrent = repeatCooldown;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            repeatClick = false;
        }
    }
}
