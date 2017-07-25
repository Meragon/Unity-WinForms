namespace System.Windows.Forms
{
    using System.Collections;

    public class BaseCollection : IEnumerable, ICollection
    {
        private readonly ArrayList list;

        public BaseCollection()
        {
            list = new ArrayList();
        }

        public virtual int Count { get { return list.Count; } }
        public bool IsReadOnly { get { return list.IsReadOnly; } }
        public bool IsSynchronized { get { return list.IsSynchronized; } }
        public object SyncRoot { get { return list.SyncRoot; } }

        protected virtual ArrayList List { get { return list; } }

        public void CopyTo(Array ar, int index)
        {
            list.CopyTo(ar, index);
        }
        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
