using System.Runtime.InteropServices;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents a single byte in <a href="https://en.wikipedia.org/wiki/Hexadecimal">hexadecimal notation</a>.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 2, Size = (sizeof(char) * 2))]
    public readonly struct HexByte
    {
        [FieldOffset((sizeof(char) * 0))]
        private readonly char m_high;
        [FieldOffset((sizeof(char) * 1))]
        private readonly char m_low;

        /// <summary>
        /// Returns the most significant char of this <see cref="HexByte"/> value.
        /// </summary>
        public char High { get => m_high; }
        /// <summary>
        /// Returns the least significant char of this <see cref="HexByte"/> value.
        /// </summary>
        public char Low { get => m_low; }

        /// <summary>
        /// Initializes a new <see cref="HexByte"/> value.
        /// </summary>
        /// <param name="high">The most significant char.</param>
        /// <param name="low">The least significant char.</param>
        public HexByte(char high, char low) {
            m_high = high;
            m_low = low;
        }
        /// <summary>
        /// Initializes a new <see cref="HexByte"/> value.
        /// </summary>
        /// <param name="value">The byte value that will be converted to hexadecimal.</param>
        public HexByte(byte value) : this(ParseChar(Operations.ExtractHigh(value)), ParseChar(Operations.ExtractLow(value))) { }

        /// <remarks>
        /// Algorithm derived from https://stackoverflow.com/a/14333437; all credit goes to the original author.
        /// </remarks>
        private static char ParseChar(byte value) {
            return ((char)((55 + value) + (((value - 10) >> 31) & 65529)));
        }
    }
}
