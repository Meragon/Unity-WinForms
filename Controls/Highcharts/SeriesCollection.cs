namespace Highcharts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class SeriesCollection : IList<Series>, IDisposable
    {
        internal readonly Highchart chart;
        
        private readonly List<Series> items = new List<Series>();
        
        public SeriesCollection(Highchart o)
        {
            chart = o;
        }

        public int Count { get { return items.Count; } }
        public bool IsReadOnly { get; private set; }

        public Series this[int index]
        {
            get { return items[index]; }
            set { throw new NotImplementedException(); }
        }

        public Series this[string name]
        {
            get { return items.Find(x => x.name == name); }
        }

        public Series Add(string name)
        {
            var s = new SeriesLineSolid(); // Default.
            s.name = name;
            s.collection = this;

            Add(s);
            return s;
        }
        
        public void Add(Series s)
        {
            s.index = items.Count;
            s.color = chart.GetNextColor();
            s.collection = this;
            
            s.NameChanged -= Series_OnNameChanged;
            s.NameChanged += Series_OnNameChanged;

            items.Add(s);

            chart.AsyncUpdateLegendButtons();
        }
        public void Clear()
        {
            UnsubscribeSeries();
            items.Clear();

            chart.ResetColorIndex();
            chart.AsyncUpdateLegendButtons();
        }
        public bool Contains(Series item)
        {
            return items.Contains(item);
        }
        public void CopyTo(Series[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }
        public Series Find(Predicate<Series> predicate)
        {
            return items.Find(predicate);
        }
        IEnumerator<Series> IEnumerable<Series>.GetEnumerator()
        {
            return items.GetEnumerator();
        }
        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }
        public int IndexOf(Series item)
        {
            return items.IndexOf(item);
        }
        public void Insert(int index, Series item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            
            items.Insert(index, item);

            UpdateIndexes();
            
            item.NameChanged += Series_OnNameChanged;
            item.NameChanged -= Series_OnNameChanged;
            
            chart.AsyncUpdateLegendButtons();
        }
        public bool Remove(Series item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            var removeResult = items.Remove(item);
            
            item.NameChanged -= Series_OnNameChanged;
            
            UpdateIndexes();
            
            chart.AsyncUpdateLegendButtons();

            return removeResult;
        }
        public void RemoveAt(int index)
        {
            Remove(items[index]);
            UpdateIndexes();
        }
        
        public void Dispose()
        {
            UnsubscribeSeries();

            if (chart != null) chart.Dispose();
        }

        internal int GetColorAlpha()
        {
            return 255 / Count;
        }

        private void Series_OnNameChanged(object sender, EventArgs eventArgs)
        {
            chart.AsyncUpdateLegendButtons();
        }
        private void UnsubscribeSeries()
        {
            for (int i = 0; i < items.Count; i++)
                items[i].NameChanged -= Series_OnNameChanged;
        }
        private void UpdateIndexes()
        {
            for (int i = 0; i < items.Count; i++) 
                items[i].index = i;
        }
    }
}
