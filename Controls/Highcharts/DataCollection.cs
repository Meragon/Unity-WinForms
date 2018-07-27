namespace Highcharts
{
    using System.Collections;
    using System.Collections.Generic;

    public class DataCollection : IEnumerable
    {
        private readonly List<double> items = new List<double>();
        private readonly Series       owner;

        private double max;
        private double min;
        
        public DataCollection(Series s)
        {
            owner = s;

            DefaultMax = 1d;

            Clear();
        }

        public int Count
        {
            get { return items.Count; }
        }
        public double DefaultMax { get; set; }
        
        public double this[int index]
        {
            get { return items[index]; }
        }

        public void Add(double val)
        {
            items.Add(val);

            if (val < min) min = val;
            if (val > max) max = val;
        }
        public void Clear()
        {
            items.Clear();

            min = 0;
            max = DefaultMax;
        }
        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }
        public double GetMax()
        {
            UpdateMaximum();
            return max;
        }
        public double GetMin()
        {
            UpdateMinimum();
            return min;
        }
        public void RemoveAt(int index)
        {
            double valAt = items[index];

            items.RemoveAt(index);

            if (valAt <= min)
                UpdateMinimum();

            if (valAt >= max)
                UpdateMaximum();
        }

        private void UpdateMaximum()
        {
            var changed = false;
            max = 0;

            // Find a new max value.
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (changed == false || item > max)
                {
                    max = item;
                    changed = true;
                }
            }

            // Reset max.
            if (changed == false)
                max = DefaultMax;
        }
        private void UpdateMinimum()
        {
            var changed = false;
            min = 0;

            // Find a new min value.
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (changed == false || item < min)
                {
                    min = item;
                    changed = true;
                }
            }

            // Reset min.
            if (changed == false)
                min = 0;
        }
    }
}