namespace Highcharts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class SeriesCollection : IList<Series>, IDisposable
    {
        internal readonly Highchart owner;
        
        private readonly List<Series> items = new List<Series>();
        
        public SeriesCollection(Highchart o)
        {
            owner = o;
        }

        public int Count { get { return items.Count; } }
        public bool IsReadOnly { get; private set; }

        public Series this[int index]
        {
            get { return items[index]; }
            set { throw new NotImplementedException(); }
        }
        public Series this[string name] { get { return items.Find(x => x.name == name); } }

        public Series Add(string name)
        {
            var s = new Series();
            s.owner = this;
            s.name = name;

            Add(s);
            return s;
        }
        public Series Add(string name, SeriesTypes type)
        {
            var s = new Series(name);
            s.owner = this;
            s.type = type;

            Add(s);
            return s;
        }
        public void Add(Series s)
        {
            s.color = owner.GetNextColor();
            s.NameChanged -= Series_OnNameChanged;
            s.NameChanged += Series_OnNameChanged;

            items.Add(s);

            owner.UpdateLegend();
        }
        public void Clear()
        {
            UnsubscribeSeries();
            items.Clear();

            owner.ResetColorIndex();
            owner.UpdateLegend();
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
            
            item.NameChanged += Series_OnNameChanged;
            item.NameChanged -= Series_OnNameChanged;
            
            owner.UpdateLegend();
        }
        public bool Remove(Series item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            var removeResult = items.Remove(item);
            item.NameChanged -= Series_OnNameChanged;
            owner.UpdateLegend();

            return removeResult;
        }
        public void RemoveAt(int index)
        {
            Remove(items[index]);
        }
        
        public void Dispose()
        {
            UnsubscribeSeries();

            if (owner != null) owner.Dispose();
        }

        private void Series_OnNameChanged(object sender, EventArgs eventArgs)
        {
            owner.UpdateLegend();
        }
        private void UnsubscribeSeries()
        {
            for (int i = 0; i < items.Count; i++)
                items[i].NameChanged -= Series_OnNameChanged;
        }
    }
}
