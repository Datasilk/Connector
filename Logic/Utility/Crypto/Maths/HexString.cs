using Crypto.Extensions;
using System;
using System.Linq;

namespace Crypto.Maths
{
    /// <summary>
    /// A collection of methods for handling <a href="https://en.wikipedia.org/wiki/Hexadecimal">hexadecimal strings</a>.
    /// </summary>
    public static class HexString
    {
        private static readonly HexByte[] ByteToHexByteMap = Enumerable
            .Range(0, 256)
            .Select(i => new HexByte(((byte)i)))
            .ToArray();
        private static readonly byte[] NybbleToByteMap = Enumerable
            .Range(0, 103)
            .Select(i => (ParseNybble((char)i) ?? byte.MaxValue))
            .ToArray();

        /// <summary>
        /// Copies the hexadecimal characters in a string value to a new byte array.
        /// </summary>
        public static byte[] Convert(string value) {
            if (value.IsNull()) {
                return null;
            }

            var valueLength = value.Length;

            if (0 == valueLength) {
                return CompatibilityUtils.Array.Empty<byte>();
            }

            if ((2 > valueLength) || Operations.IsOdd(valueLength)) {
                throw new FormatException(message: "hexadecimal value length cannot be less than two or odd");
            }

            var count = (valueLength >> 1);
            var result = new byte[count];

            unsafe {
                fixed (char* source = value)
                fixed (byte* target = &result[0])
                fixed (byte* map = &NybbleToByteMap[0]) {
                    var s = ((HexByte*)source);
                    var t = target;

                    while (0 < count--) {
                        var cHigh = (*s).High;
                        var cLow = (*s).Low;

                        byte bHigh;
                        byte bLow;

                        if ((cHigh < 103) && (cLow < 103) && ((bHigh = map[cHigh]) < byte.MaxValue) && ((bLow = map[cLow]) < byte.MaxValue)) {
                            *t = ((byte)(((uint)(bHigh << 4)) | bLow));
                        }
                        else {
                            throw new FormatException(message: ("invalid hexadecimal value encountered: " + cHigh + cLow));
                        }

                        s++;
                        t++;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Copies the hexadecimal bytes in an array value to a string value.
        /// </summary>
        public static string Convert(byte[] value) {
            if (value.IsNull()) {
                return null;
            }

            var valueLength = value.Length;

            if (0 == valueLength) {
                return string.Empty;
            }

            var count = valueLength;
            var result = new string('☠', (valueLength << 1));

            unsafe {
                fixed (byte* source = &value[0])
                fixed (char* target = result)
                fixed (HexByte* map = &ByteToHexByteMap[0]) {
                    var s = source;
                    var t = ((HexByte*)target);

                    while (0 < count--) {
                        *t = map[*s];

                        s++;
                        t++;
                    }
                }
            }

            return result;
        }

        private static bool IsHexDigit(char value) {
            return (
                ((value >= '0') && (value <= '9'))
             || ((value >= 'A') && (value <= 'F'))
             || ((value >= 'a') && (value <= 'f'))
            );
        }
        /// <remarks>
        /// Method derived from https://stackoverflow.com/a/20695932; all credit goes to the original author.
        /// </remarks>
        private static byte? ParseNybble(char value) {
            if (IsHexDigit(value)) {
                return ((byte)(((value - 48) + (((57 - value) >> 31) & 9)) & 15));
            }
            else {
                return null;
            }
        }
    }
}
