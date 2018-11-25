using System;
using System.Runtime.CompilerServices;

namespace Crypto.Extensions
{
    /// <summary>
    /// A collection of extension methods that directly or indirectly augment the <see cref="Span{T}"/> class.
    /// </summary>
    public static class SpanExtensions
    {
        /// <summary>
        /// Returns a new value containing the contents of both specified spans.
        /// </summary>
        /// <typeparam name="T">The type that is encapsulated by each span.</typeparam>
        /// <param name="value">The first span that will be copied.</param>
        /// <param name="other">The second span that will be copied.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> Append<T>(this ReadOnlySpan<T> value, ReadOnlySpan<T> other) {
            var spanLength = value.Length;
            var buffer = new T[(spanLength + other.Length)];
            var bufferSpan = buffer.AsSpan();

            value.CopyTo(bufferSpan.Slice(0, spanLength));
            other.CopyTo(bufferSpan.Slice(spanLength));

            return new ReadOnlySpan<T>(buffer);
        }
    }
}
