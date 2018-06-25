namespace Unity.Views
{
    using System.Collections.Generic;

    public struct EditorValue<T>
    {
        private readonly bool changed;
        private readonly T fvalue;

        public EditorValue(T value, bool changed)
        {
            fvalue = value;
            this.changed = changed;
        }

        public bool Changed { get { return changed; } }
        public T Value { get { return fvalue; } }

        public static bool operator ==(EditorValue<T> left, EditorValue<T> right)
        {
            return left.Value.Equals(right.Value);
        }
        public static bool operator ==(EditorValue<T> left, T right)
        {
            return left.Value.Equals(right);
        }
        public static bool operator !=(EditorValue<T> left, T right)
        {
            return !left.Value.Equals(right);
        }
        public static bool operator !=(EditorValue<T> left, EditorValue<T> right)
        {
            return !left.Value.Equals(right.Value);
        }
        public static implicit operator T(EditorValue<T> value)
        {
            return value.Value;
        }
        public static implicit operator EditorValue<T>(T value)
        {
            return new EditorValue<T>(value, true);
        }

        public bool Equals(EditorValue<T> other)
        {
            return changed == other.changed && EqualityComparer<T>.Default.Equals(fvalue, other.fvalue);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is EditorValue<T> && Equals((EditorValue<T>)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (changed.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(fvalue);
            }
        }
    }
}