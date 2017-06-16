using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    internal sealed class FormCollection : IList
    {
        private readonly List<Form> items = new List<Form>();

        public Form this[int index]
        {
            get { return items[index]; }
        }

        public void Add(Form form)
        {
            items.Add(form);

            Sort();
        }
        public bool Contains(Form form)
        {
            return items.Contains(form);
        }
        public Form Find(Predicate<Form> match)
        {
            return items.Find(match);
        }
        public int FindIndex(Predicate<Form> match)
        {
            return items.FindIndex(match);
        }
        public void Remove(Form form)
        {
            items.Remove(form);
        }
        public void Sort()
        {
            bool topMostForms = true;
            int lastTopMost = -1;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var form = items[i];
                if (topMostForms)
                {
                    if (form.TopMost == false)
                        topMostForms = false;
                    else
                        lastTopMost = i;
                }
                else
                {
                    if (form.TopMost)
                    {
                        if (lastTopMost != -1)
                        {
                            items.Insert(lastTopMost, form);
                            lastTopMost--;
                        }
                        else
                        {
                            lastTopMost = items.Count - 1;
                            items.Add(form);
                        }
                        items.RemoveAt(i);
                    }
                }
            }
        }

        #region IList
        object IList.this[int index]
        {
            get { return items[index]; }
            set
            {
                var form = value as Form;
                if (form == null)
                    throw new ArgumentException("wrong value.");
                items[index] = form;
            }
        }

        public int Count { get { return items.Count; } }
        public bool IsFixedSize
        {
            get { return false; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        public bool IsSynchronized { get { return false; } }
        public object SyncRoot
        {
            get { return null; }
        }

        int IList.Add(object value)
        {
            var form = value as Form;
            if (form == null)
                throw new ArgumentException("wrong value.");

            items.Add(form);
            return items.Count - 1;
        }
        public void Clear()
        {
            items.Clear();
        }
        bool IList.Contains(object value)
        {
            var form = value as Form;
            if (form == null)
                throw new ArgumentException("wrong value.");

            return items.Contains(form);
        }
        void ICollection.CopyTo(Array array, int index)
        {
            var forms = array as Form[];
            if (forms == null)
                throw new ArgumentException("wrong value.");
            items.CopyTo(forms, index);
        }
        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }
        int IList.IndexOf(object value)
        {
            var form = value as Form;
            if (form == null)
                throw new ArgumentException("wrong value.");

            return items.IndexOf(form);
        }
        void IList.Insert(int index, object value)
        {
            var form = value as Form;
            if (form == null)
                throw new ArgumentException("wrong value.");

            items.Insert(index, form);
        }
        void IList.Remove(object value)
        {
            var form = value as Form;
            if (form == null)
                throw new ArgumentException("wrong value.");

            items.Remove(form);
        }
        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }
        #endregion
    }
}
