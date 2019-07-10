namespace System.Windows.Forms
{
    using System.ComponentModel;

    public class Timer : Component
    {
        private float currentTime;
        private bool enabled;
        private int interval;

        public Timer()
        {
            interval = 100;
        }
        public Timer(IContainer container) : this()
        {
            if (container == null)
                throw new ArgumentNullException("container");

            container.Add(this);
        }

        public event EventHandler Tick;

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value)
                    return;

                enabled = value;
                if (value)
                    StartTimer();
                else
                    StopTimer();
            }
        }
        public int Interval
        {
            get { return interval; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");

                if (interval == value)
                    return;

                interval = value;
                if (Enabled)
                    RestartTimer();
            }
        }
        public object Tag { get; set; }

        public void Start()
        {
            Enabled = true;
        }
        public void Stop()
        {
            Enabled = false;
        }
        public override string ToString()
        {
            return base.ToString() + ", Interval: " + interval;
        }

        protected override void Dispose(bool release_all)
        {
            base.Dispose(release_all);

            StopTimer();
        }
        protected virtual void OnTick(EventArgs e)
        {
            var handler = Tick;
            if (handler != null)
                handler(this, e);
        }

        private void Application_UpdateEvent()
        {
            if (!Enabled)
                return;
            
            currentTime += swfHelper.GetDeltaTime();
            
            if (currentTime <= interval / 1000f) 
                return;

            currentTime = 0;
            
            OnTick(EventArgs.Empty);
        }
        private void RestartTimer()
        {
            StopTimer();
            StartTimer();
        }
        private void StartTimer()
        {
            Application.UpdateEvent -= Application_UpdateEvent;
            Application.UpdateEvent += Application_UpdateEvent;
        }
        private void StopTimer()
        {
            currentTime = 0;
            Application.UpdateEvent -= Application_UpdateEvent;
        }
    }
}
