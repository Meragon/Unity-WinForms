namespace Highcharts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    public class AxisCollection : IList<Axis>
    {
        private readonly List<Axis> items = new List<Axis>();
        private readonly Highchart owner;
        
        public AxisCollection(Highchart chart)
        {
            owner = chart;
        }
        
        public Axis this[int index]
        {
            get { return items[index]; }
            set
            {
                Unsubscribe(items[index]);
                
                items[index] = value;
                
                Unsubscribe(value);
                Subscribe(value);
            }
        }
        
        public int Count
        {
            get { return items.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        
        public IEnumerator<Axis> GetEnumerator()
        {
            return items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void Add(Axis item)
        {
            items.Add(item);
            
            Unsubscribe(item);
            Subscribe(item);
        }
        public void Clear()
        {
            for (int i = 0; i < items.Count; i++)
                Unsubscribe(items[i]);
            
            items.Clear();
        }
        public bool Contains(Axis item)
        {
            return items.Contains(item);
        }
        public void CopyTo(Axis[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }
        public bool Remove(Axis item)
        {
            Unsubscribe(item);
            
            return items.Remove(item);
        }
        public int IndexOf(Axis item)
        {
            return items.IndexOf(item);
        }
        public void Insert(int index, Axis item)
        {
            Unsubscribe(item);
            Subscribe(item);
            
            items.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            Unsubscribe(items[index]);
            
            items.RemoveAt(index);
        }

        private void Subscribe(Axis axis)
        {
            if (axis == null)
                return;
            
            axis.OffsetChanged += AxisOnOffsetChanged;
        }
        private void Unsubscribe(Axis axis)
        {
            if (axis == null)
                return;
            
            axis.OffsetChanged -= AxisOnOffsetChanged;
        }
        
        private void AxisOnOffsetChanged(object sender, EventArgs e)
        {
            // Adjust plot left position.
            // TODO: adjust plot bottom position.
            var marginLeftValue = owner.chart.spacingLeft + 38;
            if (owner.chart.marginLeft != null)
                marginLeftValue = owner.chart.marginLeft.Value;

            var nextLeft = marginLeftValue;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i] as YAxis;
                if (item == null)
                    continue;
                
                nextLeft = marginLeftValue + item.offset;
            }

            if (nextLeft < 8) // Add minimum padding to show left category.
                nextLeft = 8;
            
            if (nextLeft == owner.cachedPlotLeft)
                return;
            
            owner.cachedPlotLeft = nextLeft;
            owner.Refresh();
        }
    }
}