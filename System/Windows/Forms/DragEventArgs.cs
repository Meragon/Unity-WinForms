using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class DragEventArgs : EventArgs
    {
        public DragEventArgs(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect)
        {
            _data = data;
            _keyState = keyState;
            _x = x;
            _y = y;
            _allowedEffect = allowedEffect;
            _effect = effect;
        }

        private DragDropEffects _allowedEffect;
        private IDataObject _data;
        private DragDropEffects _effect;
        private int _keyState;
        private int _x;
        private int _y;

        public DragDropEffects AllowedEffect { get { return _allowedEffect; } }
        public IDataObject Data { get { return _data; } }
        public DragDropEffects Effect { get { return _effect; } set { _effect = value; } }
        public int KeyState { get { return _keyState; } }
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
    }

    public enum DragDropEffects
    {
        Scroll = -2147483648,
        All = -2147483645,
        None = 0,
        Copy = 1,
        Move = 2,
        Link = 4,
    }
}
