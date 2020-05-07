using System;
using System.Collections.Generic;
using System.Text;

namespace Icarus.Common
{
    public class Directional<T> : IDirectional<T>
    {
        public Directional(T left, T right)
        {
            this.Left = left;
            this.Right = right;
        }

        public T Left { get; }
        public T Right { get; }
    }
}
