namespace Crypto.Extensions
{
    /// <summary>
    /// A collection of utilities that help unify coding patterns when targeting different versions of the .NET framework.
    /// </summary>
    public static class CompatibilityUtils
    {
#pragma warning disable 1591
        public static class Array
#pragma warning restore 1591
        {
            /// <summary>
            /// Returns an empty array.
            /// </summary>
            /// <typeparam name="T">The type of array that will be returned.</typeparam>
            public static T[] Empty<T>() {
#if (NET45 || NET451 || NET452)
                return EmptyArray<T>.Value;
#else
                return System.Array.Empty<T>();
#endif
            }
        }

#if (NET45 || NET451 || NET452)
        private static class EmptyArray<T>
        {
            public static readonly T[] Value = new T[0];
        }
#endif
    }
}
