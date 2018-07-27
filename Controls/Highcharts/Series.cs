namespace Highcharts
{
    using System;
    using System.Drawing;

    public enum SeriesTypes
    {
        None = 0,

        areaSolid = 1,
        areaSolidOutline = 2,
        line = 3,
        lineSolid = 4,
        point = 5,
    }

    public class Series : IDisposable
    {
        internal Pen pen = new Pen(Color.Transparent);

        private string v_name;

        internal Series()
        {
            color = Color.Gray;
            data = new DataCollection(this);
            pointInterval = 1;
            type = SeriesTypes.line;
            visible = true;
        }
        internal Series(string n) : this()
        {
            name = n;
        }

        public event EventHandler NameChanged;

        public Color color
        {
            get { return pen.Color; }
            set { pen.Color = value; }
        }
        public DataCollection data { get; private set; }
        public bool linearGradient { get; set; }
        public string name
        {
            get { return v_name; }
            set
            {
                if (v_name == value)
                    return;

                v_name = value;
                OnNameChanged(EventArgs.Empty);
            }
        }
        /// <summary>
        /// If no x values are given for the points in a series, pointInterval defines the interval of the x values. 
        /// For example, if a series contains one value every decade starting from year 0, set pointInterval to 10.
        /// Defaults to 1.
        /// </summary>
        public float pointInterval { get; set; }
        public object tag { get; set; }
        public SeriesTypes type { get; set; }
        public bool visible { get; set; }

        public void Dispose()
        {
            if (pen != null) pen.Dispose();
        }

        protected virtual void OnNameChanged(EventArgs e)
        {
            var handler = NameChanged;
            if (handler != null)
                handler(this, e);
        }
    }
}
