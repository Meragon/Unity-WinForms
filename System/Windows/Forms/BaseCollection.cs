using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class BaseCollection : IEnumerable, ICollection
    {
        private ArrayList _list;

        public BaseCollection()
        {
            _list = new ArrayList();
        }

        public virtual int Count { get { return _list.Count; } }
        public bool IsReadOnly { get { return _list.IsReadOnly; } }
        public bool IsSynchronized { get { return _list.IsSynchronized; } }
        protected virtual ArrayList List { get { return _list; } }
        public object SyncRoot { get { return _list.SyncRoot; } }

        public void CopyTo(Array ar, int index)
        {
            _list.CopyTo(ar, index);
        }
        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
