using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel_Mini
{
    public class CollectionChangeEventArgs : EventArgs
    {
        private CollectionChangeAction _action;
        private object _element;

        public CollectionChangeEventArgs(CollectionChangeAction action, object element)
        {
            _action = action;
            _element = element;
        }
        public virtual CollectionChangeAction Action { get { return _action; } }
        public virtual object Element { get { return _element; } }
    }
}
