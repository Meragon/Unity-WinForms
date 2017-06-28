using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public sealed class ImageList
    {
        private readonly ImageList.ImageCollection _images = new ImageCollection();

        public ImageList.ImageCollection Images { get { return _images; } }

        public sealed class ImageCollection : IList, ICollection, IEnumerable
        {
            private readonly List<ImageInfo> _items = new List<ImageInfo>();

            public int Count { get { return _items.Count; } }
            public bool Empty { get { return _items.Count == 0; } }
            public bool IsReadOnly { get { return false; } }
            public StringCollection Keys
            {
                get
                {
                    StringCollection scollection = new StringCollection();
                    for (int i = 0; i < _items.Count; i++)
                    {
                        if (_items[i].Key == null)
                            scollection.Add(string.Empty);
                        else
                            scollection.Add(_items[i].Key);
                    }
                    return scollection;
                }
            }

            public Image this[int index]
            {
                get { return _items[index].Image; }
                set { _items[index].Image = value; }
            }
            public Image this[string key]
            {
                get
                {
                    var item = _items.Find(x => x.Key == key);
                    if (item == null) return null;
                    return item.Image;
                }
            }

            public void Add(Image value)
            {
                _items.Add(new ImageInfo(string.Empty, value));
            }
            public void Add(string key, Image image)
            {
                _items.Add(new ImageInfo(key, image));
            }
            public void AddRange(Image[] images)
            {
                for (int i = 0; i < images.Length; i++)
                    _items.Add(new ImageInfo(string.Empty, images[i]));
            }
            public void Clear()
            {
                _items.Clear();
            }
            public bool Contains(Image image)
            {
                return _items.Find(x => x.Image == image) != null;
            }
            public bool ContainsKey(string key)
            {
                return _items.Find(x => x.Key == key) != null;
            }
            public IEnumerator GetEnumerator()
            {
                return _items.GetEnumerator();
            }
            public int IndexOf(Image image)
            {
                for (int i = 0; i < _items.Count; i++)
                    if (image == _items[i].Image)
                        return i;

                return -1;
            }
            public int IndexOfKey(string key)
            {
                for (int i = 0; i < _items.Count; i++)
                    if (key == _items[i].Key)
                        return i;

                return -1;
            }
            public void Remove(Image image)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if (image == _items[i].Image)
                    {
                        _items.RemoveAt(i);
                        return;
                    }
                }
            }
            public void RemoveAt(int index)
            {
                _items.RemoveAt(index);
            }
            public void RemoveByKey(string key)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if (key == _items[i].Key)
                    {
                        _items.RemoveAt(i);
                        return;
                    }
                }
            }
            public void SetKeyName(int index, string name)
            {
                _items[index].Key = name;
            }

            internal class ImageInfo
            {
                public string Key { get; set; }
                public Image Image { get; set; }

                public ImageInfo(string key, Image image)
                {
                    Key = key;
                    Image = image;
                }
            }

            int IList.Add(object value)
            {
                if (value is Image)
                {
                    Add((Image)value);
                    return Count - 1;
                }

                return -1;
            }
            bool IList.Contains(object image)
            {
                return image is Image && Contains((Image)image);
            }
            int IList.IndexOf(object image)
            {
                if (image is Image)
                {
                    return IndexOf((Image)image);
                }
                return -1;
            }
            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException();
            }
            bool IList.IsFixedSize
            {
                get { return false; }
            }
            void IList.Remove(object image)
            {
                if (image is Image)
                {
                    Remove((Image)image);
                }
            }
            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    if (value is Image)
                    {
                        this[index] = (Image)value;
                        return;
                    }
                }
            }
            void ICollection.CopyTo(Array dest, int index)
            {
                throw new NotSupportedException(); // implement this?
            }
            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }
            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }
        }
    }
}
