using System.Collections.Generic;
using System.Linq;

namespace Amazon.S3.Wrapper.Extensions
{
    public static class IEnumerableExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return true;

            return !enumerable.Any();
        }
    }
}
