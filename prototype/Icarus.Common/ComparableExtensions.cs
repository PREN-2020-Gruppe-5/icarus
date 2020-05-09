using System;

namespace Icarus.Common
{
    public static class ComparableExtensions
    {
        public static bool IsWithin(this IComparable value, IComparable minimum, IComparable maximum)
        {
            return maximum.CompareTo(value) >= 0 && value.CompareTo(minimum) >= 0;
        }
    }
}
