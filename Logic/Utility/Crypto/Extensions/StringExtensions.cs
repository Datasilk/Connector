using System.Runtime.CompilerServices;

namespace Crypto.Extensions
{
    /// <summary>
    /// A collection of extension methods that directly or indirectly augment the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Indicates whether the input string is null or empty.
        /// </summary>
        /// <param name="value">The string that will be tested.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
        /// <summary>
        /// Indicates whether the input string is null, empty, or consists entirely of white-space characters.
        /// </summary>
        /// <param name="value">The string that will be tested.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);
    }
}
