using System;
using System.Runtime.ExceptionServices;

namespace Crypto.Extensions
{
    /// <summary>
    /// A collection of extension methods that directly or indirectly augment the <see cref="Exception"/> class.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Rethrows an exception; preserves the existing stack state.
        /// </summary>
        /// <typeparam name="TException">The type of exception that will be rethrown.</typeparam>
        /// <param name="exception">The exception that will be rethrown.</param>
        public static void Rethrow<TException>(this TException exception) where TException : Exception => ExceptionDispatchInfo.Capture(exception).Throw();
    }
}
