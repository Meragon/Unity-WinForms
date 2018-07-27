namespace Highcharts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class SeriesCollection : IEnumerable, IDisposable
    {
        private readonly List<Series> items = new List<Series>();
        private readonly Highchart owner;

        public SeriesCollection(Highchart o)
        {
            owner = o;
        }

        public int Count { get { return items.Count; } }

        public Series this[int index] { get { return items[index]; } }
        public Series this[string name] { get { return items.Find(x => x.name == name); } }

        public Series Add(string name)
        {
            Series s = new Series();
            s.name = name;

            Add(s);
            return s;
        }
        public Series Add(string name, SeriesTypes type)
        {
            Series s = new Series(name);
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
        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
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
