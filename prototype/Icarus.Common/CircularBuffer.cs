using System;
using System.Collections;
using System.Collections.Generic;

namespace Icarus.Common
{
    // stripped version of  https://github.com/joaoportela/CircullarBuffer-CSharp
    public class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly T[] buffer;

        private int start;
        private int end;

        public CircularBuffer(int capacity, T[] items)
        {
            if (capacity < 1)
            {
                throw new ArgumentException("Circular buffer cannot have negative or zero capacity.", nameof(capacity));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (items.Length > capacity)
            {
                throw new ArgumentException("Too many items to fit circular buffer", nameof(items));
            }

            buffer = new T[capacity];

            Array.Copy(items, buffer, items.Length);
            Size = items.Length;

            start = 0;
            end = Size == capacity ? 0 : Size;
        }

        public int Capacity => buffer.Length;

        public bool IsFull => Size == Capacity;

        public bool IsEmpty => Size == 0;

        public int Size { get; private set; }

        public T this[int index]
        {
            get
            {
                if (IsEmpty)
                {
                    throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty");
                }

                if (index >= Size)
                {
                    throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {Size}");
                }

                var actualIndex = InternalIndex(index);
                return buffer[actualIndex];
            }
            set
            {
                if (IsEmpty)
                {
                    throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty");
                }

                if (index >= Size)
                {
                    throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {Size}");
                }

                var actualIndex = InternalIndex(index);
                buffer[actualIndex] = value;
            }
        }

        public void PushFront(T item)
        {
            if (IsFull)
            {
                Decrement(ref start);
                end = start;
                buffer[start] = item;
            }
            else
            {
                Decrement(ref start);
                buffer[start] = item;
                ++Size;
            }
        }
     
        public IEnumerator<T> GetEnumerator()
        {
            var segments = new ArraySegment<T>[2] {ArrayOne(), ArrayTwo()};
            foreach (var segment in segments)
            {
                for (var i = 0; i < segment.Count; i++)
                {
                    yield return segment.Array[segment.Offset + i];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Decrement(ref int index)
        {
            if (index == 0)
            {
                index = Capacity;
            }

            index--;
        }

        private int InternalIndex(int index)
        {
            return start + (index < (Capacity - start) ? index : index - Capacity);
        }

        private ArraySegment<T> ArrayOne()
        {
            if (IsEmpty)
            {
                return new ArraySegment<T>(new T[0]);
            }
            else if (start < end)
            {
                return new ArraySegment<T>(buffer, start, end - start);
            }
            else
            {
                return new ArraySegment<T>(buffer, start, buffer.Length - start);
            }
        }

        private ArraySegment<T> ArrayTwo()
        {
            if (IsEmpty)
            {
                return new ArraySegment<T>(new T[0]);
            }
            else if (start < end)
            {
                return new ArraySegment<T>(buffer, end, 0);
            }
            else
            {
                return new ArraySegment<T>(buffer, 0, end);
            }
        }
    }
}
