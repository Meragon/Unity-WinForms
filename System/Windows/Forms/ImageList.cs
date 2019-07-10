namespace System.Windows.Forms
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Drawing;

    public sealed class ImageList
    {
        private readonly ImageCollection images = new ImageCollection();

        public ImageCollection Images { get { return images; } }

        public sealed class ImageCollection : IList
        {
            private readonly List<ImageInfo> items = new List<ImageInfo>();

            public int Count { get { return items.Count; } }
            public bool Empty { get { return items.Count == 0; } }
            public bool IsReadOnly { get { return false; } }
            public StringCollection Keys
            {
                get
                {
                    var collection = new StringCollection();
                    
                    for (int i = 0; i < items.Count; i++)
                    {
                        var itemKey = items[i].Key;
                        if (itemKey == null)
                            itemKey = string.Empty;
                        
                        collection.Add(itemKey);
                    }
                    
                    return collection;
                }
            }

            public Image this[int index]
            {
                get { return items[index].Image; }
                set { items[index].Image = value; }
            }
            public Image this[string key]
            {
                get
                {
                    var item = items.Find(x => x.Key == key);
                    if (item == null) return null;
                    return item.Image;
                }
            }

            public void Add(Image value)
            {
                items.Add(new ImageInfo(string.Empty, value));
            }
            public void Add(string key, Image image)
            {
                items.Add(new ImageInfo(key, image));
            }
            public void AddRange(Image[] images)
            {
                for (int i = 0; i < images.Length; i++)
                    items.Add(new ImageInfo(string.Empty, images[i]));
            }
            public void Clear()
            {
                items.Clear();
            }
            public bool Contains(Image image)
            {
                return items.Find(x => x.Image == image) != null;
            }
            public bool ContainsKey(string key)
            {
                return items.Find(x => x.Key == key) != null;
            }
            public IEnumerator GetEnumerator()
            {
                return items.GetEnumerator();
            }
            public int IndexOf(Image image)
            {
                for (int i = 0; i < items.Count; i++)
                    if (image == items[i].Image)
                        return i;

                return -1;
            }
            public int IndexOfKey(string key)
            {
                for (int i = 0; i < items.Count; i++)
                    if (key == items[i].Key)
                        return i;

                return -1;
            }
            public void Remove(Image image)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (image == items[i].Image)
                    {
                        items.RemoveAt(i);
                        return;
                    }
                }
            }
            public void RemoveAt(int index)
            {
                items.RemoveAt(index);
            }
            public void RemoveByKey(string key)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (key == items[i].Key)
                    {
                        items.RemoveAt(i);
                        return;
                    }
                }
            }
            public void SetKeyName(int index, string name)
            {
                items[index].Key = name;
            }

            int IList.Add(object value)
            {
                var image = value as Image;
                if (image != null)
                {
                    Add(image);
                    return Count - 1;
                }

                return -1;
            }
            bool IList.Contains(object value)
            {
                var image = value as Image;
                return image != null && Contains(image);
            }
            int IList.IndexOf(object value)
            {
                var image = value as Image;
                if (image != null)
                    return IndexOf(image);
                
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
            void IList.Remove(object value)
            {
                var image = value as Image;
                if (image != null) 
                    Remove(image);
            }
            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    var image = value as Image;
                    if (image != null) 
                        this[index] = image;
                }
            }
            void ICollection.CopyTo(Array array, int index)
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

            internal class ImageInfo
            {
                public ImageInfo(string key, Image image)
                {
                    Key = key;
                    Image = image;
                }

                public string Key { get; set; }
                public Image Image { get; set; }
            }
        }
    }
}
