using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Crypto.Extensions
{
    /// <summary>
    /// A collection of extension methods that directly or indirectly augment the <see cref="List{T}"/> class.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Indicates whether the input list is null or empty.
        /// </summary>
        /// <param name="list">The list that will be tested.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(IList<T> list) => (list.IsNull() || (0 == list.Count));
    }
}
