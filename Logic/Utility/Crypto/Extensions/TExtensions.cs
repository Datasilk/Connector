using System.Runtime.CompilerServices;

namespace Crypto.Extensions
{
    /// <summary>
    /// A collection of extension methods that directly or indirectly augment types generically.
    /// </summary>
    public static class TExtensions
    {
        /// <summary>
        /// Indicates whether the input is not null.
        /// </summary>
        /// <param name="value">The value that will be tested.</param>
        /// <typeparam name="T">The type of the instance that will be tested.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull<T>(this T value) => (null != value);
        /// <summary>
        /// Indicates whether the input is null.
        /// </summary>
        /// <param name="value">The value that will be tested.</param>
        /// <typeparam name="T">The type of the instance that will be tested.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(this T value) => (null == value);
    }
}
