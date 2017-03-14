using System;
using System.Collections.Generic;

namespace Isidore.MagicMirror.ImageProcessing.Helpers
{
    public static class LinqExtensions
    {
        public static T MaxBy<T>(this IEnumerable<T> collection, Func<T, int> comparisonAction)
        {
            T max = default(T);
            var maxVal = int.MinValue;
            foreach (var el in collection)
            {
                var value = comparisonAction(el);
                if (value > maxVal)
                {
                    max = el;
                    maxVal = value;
                }
            }

            return max;
        }
    }
}