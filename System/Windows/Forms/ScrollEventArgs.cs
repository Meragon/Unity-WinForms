using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class ScrollEventArgs : EventArgs
    {
        private int _oldValue;
        private ScrollOrientation _scrollOrientation;
        private ScrollEventType _scrollType;

        public ScrollEventArgs(ScrollEventType type, int newValue)
        {
            _scrollType = type;
            NewValue = newValue;
        }
        public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue)
        {
            _scrollType = type;
            _oldValue = oldValue;
            NewValue = newValue;
        }
        public ScrollEventArgs(ScrollEventType type, int newValue, ScrollOrientation scroll)
        {
            _scrollType = type;
            NewValue = newValue;
            _scrollOrientation = scroll;
        }
        public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue, ScrollOrientation scroll)
        {
            _scrollType = type;
            _oldValue = oldValue;
            NewValue = newValue;
            _scrollOrientation = scroll;
        }

        public int NewValue { get; set; }
        public int OldValue { get { return _oldValue; } }
        public ScrollOrientation ScrollOrientation { get { return _scrollOrientation; } }
        public ScrollEventType Type { get { return _scrollType; } }
    }
}
