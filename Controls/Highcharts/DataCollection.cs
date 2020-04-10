namespace Highcharts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class DataCollection : IEnumerable
    {
        private static readonly double[] emptyArray = new double[0];

        private readonly Series owner;

        private double[] items = new double[0]; // TODO: nullable type.
        private double max;
        private double min;
        private int size;
        
        public DataCollection(Series s)
        {
            owner = s;

            DefaultMax = 1d;

            Clear();
        }

        public int Count
        {
            get { return size; }
        }
        public double DefaultMax { get; set; }
        public double[] Items
        {
            get { return items; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                
                items = value;
                size = value.Length;
            
                UpdateMinimum();
                UpdateMaximum();
            }
        }
        
        public double this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }

        public void Add(double val)
        {
            if (size == items.Length)
                EnsureCapacity(size + 1);

            items[size++] = val;
            
            if (val < min) min = val;
            if (val > max) max = val;
        }
        public void Clear()
        {
            if (size > 0)
            {
                Array.Clear(items, 0, size);
                size = 0;
            }

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

            if ((uint) index >= (uint) size)
                throw new ArgumentOutOfRangeException("index");
            
            --size;
            
            if (index < size)
                Array.Copy(items, index + 1, items, index, size - index);
            
            items[size] = 0;

            if (valAt <= min)
                UpdateMinimum();

            if (valAt >= max)
                UpdateMaximum();
        }
        
        private void EnsureCapacity(int minCapacity)
        {
            if (items.Length >= minCapacity)
                return;
            
            int newCapacity = items.Length == 0 ? 4 : items.Length * 2;
            if (newCapacity < minCapacity)
                newCapacity = minCapacity;
            
            SetCapacity(newCapacity);
        }
        private void SetCapacity(int value)
        {
            if (value == items.Length)
                return;
            
            if (value < size)
                throw new ArgumentOutOfRangeException("value");
                
            if (value > 0)
            {
                var objArray = new double[value];
                if (size > 0)
                    Array.Copy(items, 0, objArray, 0, size);
                
                items = objArray;
            }
            else
                items = emptyArray;   
        }
        private void UpdateMaximum()
        {
            var changed = false;
            max = 0;

            // Find a new max value.
            for (int i = 0; i < items.Length; i++)
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
            for (int i = 0; i < items.Length; i++)
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