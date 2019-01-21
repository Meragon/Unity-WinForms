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
        internal SeriesCollection owner;
        internal readonly Pen pen = new Pen(Color.Transparent);
        internal bool _needRedraw = true;
        
        private string      _name;
        private bool        _linearGradient;
        private float       _pointInterval;
        private SeriesTypes _type;
        private bool        _visible;

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
        public bool linearGradient
        {
            get { return _linearGradient; }
            set
            {
                if (_linearGradient == value)
                    return;
                
                _linearGradient = value;
                _needRedraw = true;
            }
        }
        public string name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnNameChanged(EventArgs.Empty);
            }
        }
        /// <summary>
        /// If no x values are given for the points in a series, pointInterval defines the interval of the x values. 
        /// For example, if a series contains one value every decade starting from year 0, set pointInterval to 10.
        /// Defaults to 1.
        /// </summary>
        public float pointInterval
        {
            get { return _pointInterval; }
            set
            {
                if (_pointInterval == value)
                    return;

                _pointInterval = value;
                _needRedraw = true;
            }
        }
        public object tag { get; set; }
        public SeriesTypes type
        {
            get { return _type; }
            set
            {
                if (_type == value)
                    return;

                _type = value;
                _needRedraw = true;
            }
        }
        public bool visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value)
                    return;
                
                _visible = value;
                _needRedraw = true;
            }
        }
        public int yAxis { get; set; }

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
