using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Crypto.Maths
{
    /// <summary>
    /// A collection of static methods that encapsulate "common" mathematical operations.
    /// </summary>
    public static class Operations
    {
        private const int BYTE_SIZE_IN_BITS = (sizeof(byte) << 3);
        private const double DOORNIK_INVERSE_32_BIT = 2.32830643653869628906e-010d;
        private const double DOORNIK_INVERSE_52_BIT = 2.22044604925031308085e-016d;

        private static readonly IReadOnlyList<byte> m_00FF = Array.AsReadOnly(new byte[] { byte.MinValue, byte.MaxValue });

        /// <summary>
        /// Calculates the absolute value of an integer.
        /// </summary>
        /// <param name="value">The integer whose absolute value will be calculated.</param>
        /// <remarks>long.MinValue will return itself because its positive value cannot be represented as a 64 bit integer.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Abs(long value) => ((value + (value >>= ((8 * sizeof(long)) - 1))) ^ value);
        /// <summary>
        /// Adds two 32-bit signed integers into a 64-bit result.
        /// </summary>
        /// <param name="x">The first integer that will be added.</param>
        /// <param name="y">The second integer that will be added.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Add(int x, int y) => (((long)x) + y);
        /// <summary>
        /// Adds two 32-bit unsigned integers into a 64-bit result.
        /// </summary>
        /// <param name="x">The first integer that will be added.</param>
        /// <param name="y">The second integer that will be added.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Add(uint x, uint y) => (((ulong)x) + y);
        /// <summary>
        /// Compares the contents of two spans for equality in constant time.
        /// </summary>
        /// <param name="x">The first span that will be compared.</param>
        /// <param name="y">The second span that will be compared.</param>
        public static bool CompareInConstantTime(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y) {
            var min = ((x.Length > y.Length) ? y.Length : x.Length);
            var max = ((x.Length < y.Length) ? y.Length : x.Length);
            var offset = 0;
            var result = 0;
            var z = m_00FF;

            if (Vector.IsHardwareAccelerated) {
                var vectorX = MemoryMarshal.Cast<byte, Vector<byte>>(x.Slice(0, min));
                var vectorY = MemoryMarshal.Cast<byte, Vector<byte>>(y.Slice(0, min));
                var vectorCount = vectorX.Length;
                var vectorResult = Vector<byte>.Zero;

                for (var i = offset; (i < vectorCount); i++) {
                    vectorResult |= (vectorX[i] ^ vectorY[i]);
                }

                offset = (Vector<byte>.Count * vectorCount);
                result |= (Vector<byte>.Zero == vectorResult ? byte.MinValue : byte.MaxValue);
            }

            for (var i = offset; (i < min); i++) {
                result |= (x[i] ^ y[i]);
            }

            for (var i = min; (i < max); i++) {
                result |= (z[0] ^ z[1]);
            }

            return (0 == result);
        }
        /// <summary>
        /// Returns an unsigned 32-bit integer by concatenating the bit patterns of two signed 16-bit integers.
        /// </summary>
        /// <param name="highPart">The most significant half of the integer.</param>
        /// <param name="lowPart">The least significant half of the integer.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConcatBits(short highPart, short lowPart) => ConcatBits(((ushort)highPart), ((ushort)lowPart));
        /// <summary>
        /// Returns an unsigned 32-bit integer by concatenating the bit patterns of two unsigned 16-bit integers.
        /// </summary>
        /// <param name="highPart">The most significant half of the integer.</param>
        /// <param name="lowPart">The least significant half of the integer.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConcatBits(ushort highPart, ushort lowPart) => ((((uint)highPart) << 16) | lowPart);
        /// <summary>
        /// Returns an unsigned 64-bit integer by concatenating the bit patterns of two signed 32-bit integers.
        /// </summary>
        /// <param name="highPart">The most significant half of the integer.</param>
        /// <param name="lowPart">The least significant half of the integer.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ConcatBits(int highPart, int lowPart) => ConcatBits(((uint)highPart), ((uint)lowPart));
        /// <summary>
        /// Returns an unsigned 64-bit integer by concatenating the bit patterns of two unsigned 32-bit integers.
        /// </summary>
        /// <param name="highPart">The most significant half of the integer.</param>
        /// <param name="lowPart">The least significant half of the integer.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ConcatBits(uint highPart, uint lowPart) => ((((ulong)highPart) << 32) | lowPart);
        /// <summary>
        /// Counts the number of consecutive zero bits on the left-hand side of the most signifcant bit.
        /// </summary>
        /// <param name="value">The integer that will have its bits aggregated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CountLeadingZeros(long value) => ((long)CountLeadingZeros((ulong)value));
        /// <summary>
        /// Counts the number of consecutive zero bits on the left-hand side of the most signifcant bit.
        /// </summary>
        /// <param name="value">The integer that will have its bits aggregated.</param>
        /// <remarks>
        /// Algorithm derived from http://aggregate.org/MAGIC/#Leading%20Zero%20Count; all credit goes to the original author.
        /// </remarks>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong CountLeadingZeros(ulong value) => ((8 * sizeof(ulong)) - HammingWeight(FoldBits(value)));
        /// <summary>
        /// Counts the number of consecutive zero bits on the right-hand side of the least signifcant bit.
        /// </summary>
        /// <param name="value">The integer that will have its bits aggregated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CountTrailingZeros(long value) => ((long)CountTrailingZeros((ulong)value));
        /// <summary>
        /// Counts the number of consecutive zero bits on the right-hand side of the least signifcant bit.
        /// </summary>
        /// <param name="value">The integer that will have its bits aggregated.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong CountTrailingZeros(ulong value) {
            if (IsOdd(value)) { return 0UL; }
            if (0UL == value) { return 64UL; }

            var count = 1UL;

            if (0UL == (value & 0xFFFFFFFFUL)) {
                value >>= 32;
                count += 32UL;
            }

            if (0UL == (value & 0x0000FFFFUL)) {
                value >>= 16;
                count += 16UL;
            }

            if (0UL == (value & 0x000000FFUL)) {
                value >>= 8;
                count += 8UL;
            }

            if (0UL == (value & 0x0000000FUL)) {
                value >>= 4;
                count += 4UL;
            }

            if (0UL == (value & 0x00000003UL)) {
                value >>= 2;
                count += 2UL;
            }

            return (count - (value & 0x00000001UL));
        }
        /// <summary>
        /// Creates double precision floating point value between the exclusive range (0, 1).
        /// </summary>
        /// <param name="x">The value of x.</param>
        /// <param name="y">The value of y.</param>
        /// <remarks>
        /// Algorithm derived from http://doornik.com/research/randomdouble.pdf; all credit goes to the original author.
        /// </remarks>
        public static double DoornikDouble(int x, int y) => ((x * DOORNIK_INVERSE_32_BIT) + (0.5d + (DOORNIK_INVERSE_52_BIT / 2)) + ((y & 0x000FFFFF) * DOORNIK_INVERSE_52_BIT));
        /// <summary>
        /// Creates single precision floating point value between the exclusive range (0, 1).
        /// </summary>
        /// <param name="x">The value of x.</param>
        /// <remarks>
        /// Algorithm derived from http://doornik.com/research/randomdouble.pdf; all credit goes to the original author.
        /// </remarks>
        public static float DoornikSingle(int x) => ((x * ((float)DOORNIK_INVERSE_32_BIT)) + (0.5f + (((float)DOORNIK_INVERSE_32_BIT) / 2)));
        /// <summary>
        /// Extracts the most significant bits of the integer.
        /// </summary>
        /// <param name="value">The integer that bits will be extracted from.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ExtractHigh(byte value) => ((byte)(value >> 4));
        /// <summary>
        /// Extracts the most significant bits of the integer.
        /// </summary>
        /// <param name="value">The integer that bits will be extracted from.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ExtractHigh(uint value) => (value >> 16);
        /// <summary>
        /// Extracts the most significant bits of the integer.
        /// </summary>
        /// <param name="value">The integer that bits will be extracted from.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ExtractHigh(ulong value) => (value >> 32);
        /// <summary>
        /// Extracts the most significant bits of the integer.
        /// </summary>
        /// <param name="value">The integer that bits will be extracted from.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ExtractHigh(ushort value) => ((ushort)(value >> 8));
        /// <summary>
        /// Extracts the least significant bits of the integer.
        /// </summary>
        /// <param name="value">The integer that bits will be extracted from.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ExtractLow(byte value) => ((byte)(value & 0b1111));
        /// <summary>
        /// Extracts the least significant bits of the integer.
        /// </summary>
        /// <param name="value">The integer that bits will be extracted from.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ExtractLow(uint value) => ((ushort)value);
        /// <summary>
        /// Extracts the least significant bits of the integer.
        /// </summary>
        /// <param name="value">The integer that bits will be extracted from.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ExtractLow(ulong value) => ((uint)value);
        /// <summary>
        /// Extracts the least significant bits of the integer.
        /// </summary>
        /// <param name="value">The integer that bits will be extracted from.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ExtractLow(ushort value) => ((byte)value);
        /// <summary>
        /// Calculates the largest positive integer that divides x and y.
        /// </summary>
        /// <param name="x">The value of x.</param>
        /// <param name="y">The value of y.</param>
        [CLSCompliant(false)]
        public static ulong GreatestCommonDivisor(ulong x, ulong y) {
            if (x == 0) { return y; }
            if (y == 0) { return x; }

            var g = ((int)CountTrailingZeros(x | y));

            x >>= ((int)CountTrailingZeros(x));

            do {
                y >>= ((int)CountTrailingZeros(y));

                if (x > y) {
                    var z = x;

                    x = y;
                    y = z;
                }

                y = (y - x);
            } while (0 != y);

            return (x << g);
        }
        /// <summary>
        /// Counts the total number of one bits in the binary representation of the integer.
        /// </summary>
        /// <param name="value">The integer that will have its bits aggregated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long HammingWeight(long value) => ((long)HammingWeight((ulong)value));
        /// <summary>
        /// Counts the total number of one bits in the binary representation of the integer.
        /// </summary>
        /// <param name="value">The integer that will have its bits aggregated.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong HammingWeight(ulong value) {
            value -= ((value >> 1) & 0x5555555555555555UL);
            value = (((value >> 2) & 0x3333333333333333UL) + (value & 0x3333333333333333UL));

            return ((((value + (value >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }
        /// <summary>
        /// Indicates whether the integer has the least significant bit set.
        /// </summary>
        /// <param name="value">The integer that will be tested.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(byte value) => IsOdd((ulong)value);
        /// <summary>
        /// Indicates whether the integer has the least significant bit set.
        /// </summary>
        /// <param name="value">The integer that will be tested.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(int value) => IsOdd((ulong)value);
        /// <summary>
        /// Indicates whether the integer has the least significant bit set.
        /// </summary>
        /// <param name="value">The integer that will be tested.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(long value) => IsOdd((ulong)value);
        /// <summary>
        /// Indicates whether the integer is odd.
        /// </summary>
        /// <param name="value">The integer that will be tested.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(short value) => IsOdd((ulong)value);
        /// <summary>
        /// Indicates whether the integer has the least significant bit set.
        /// </summary>
        /// <param name="value">The integer that will be tested.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(uint value) => IsOdd((ulong)value);
        /// <summary>
        /// Indicates whether the integer has the least significant bit set.
        /// </summary>
        /// <param name="value">The integer that will be tested.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(ulong value) => (1UL == (value & 1UL));
        /// <summary>
        /// Indicates whether the integer has the least significant bit set.
        /// </summary>
        /// <param name="value">The integer that will be tested.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(ushort value) => IsOdd((ulong)value);
        /// <summary>
        /// Indicates whether the integer is a power of two.
        /// </summary>
        /// <param name="value">The integer that will be tested.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOf2(long value) => ((0L < value) && (0L == (value & (value - 1L))));
        /// <summary>
        /// Indicates whether the integer is a power of two.
        /// </summary>
        /// <param name="value">The integer that will be tested.</param>
        /// <remarks>
        /// Algorithm derived from http://aggregate.org/MAGIC/#Is%20Power%20of%202; all credit goes to the original author.
        /// </remarks>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOf2(ulong value) => ((0UL < value) && (0UL == (value & (value - 1UL))));
        /// <summary>
        /// Calculates the least significant bit of the integer.
        /// </summary>
        /// <param name="value">The integer whose least significant bit will be calculated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LeastSignificantBit(long value) => ((long)LeastSignificantBit((ulong)value));
        /// <summary>
        /// Calculates the least significant bit of the integer.
        /// </summary>
        /// <param name="value">The integer whose least significant bit will be calculated.</param>
        /// <remarks>
        /// Algorithm derived from http://aggregate.org/MAGIC/#Least%20Significant%201%20Bit; all credit goes to the original author.
        /// </remarks>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong LeastSignificantBit(ulong value) => (value & ((ulong)-((long)value)));
        /// <summary>
        /// Constructs a strongly universal 32-bit hash result out of a 64-bit input.
        /// </summary>
        /// <param name="x">The first state integer (must be chosen uniformly at random).</param>
        /// <param name="y">The second state integer (must be chosen uniformly at random).</param>
        /// <param name="z">The third state integer (must be chosen uniformly at random).</param>
        /// <param name="value">The integer that will be hashed.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LemireHash(ulong x, ulong y, ulong z, ulong value) {
            var low = ExtractLow(value);
            var high = ExtractHigh(value);

            return ((uint)(((x * low) + (y * high) + z) >> 32));
        }
        /// <summary>
        /// Calculates the floor of the binary logarithm of the integer; rounds up.
        /// </summary>
        /// <param name="value">The integer whose logarithm will be calculated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Log2Ceiling(long value) => ((long)Log2Ceiling((ulong)value));
        /// <summary>
        /// Calculates the floor of the binary logarithm of the integer; rounds up.
        /// </summary>
        /// <param name="value">The integer whose logarithm will be calculated.</param>
        /// <remarks>
        /// Algorithm derived from http://aggregate.org/MAGIC/#Log2%20of%20an%20Integer; all credit goes to the original author.
        /// </remarks>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Log2Ceiling(ulong value) {
            var temp = ((long)(value & (value - 1UL)));

            temp |= -temp;
            temp >>= ((8 * sizeof(long)) - 1);

            return ((ulong)(((long)Log2Floor(value)) - temp));
        }
        /// <summary>
        /// Calculates the floor of the binary logarithm of the integer; rounds down.
        /// </summary>
        /// <param name="value">The integer whose logarithm will be calculated.</param>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Log2Floor(long value) => ((long)Log2Floor((ulong)value));
        /// <summary>
        /// Calculates the floor of the binary logarithm of the integer.
        /// </summary>
        /// <param name="value">The integer whose logarithm will be calculated.</param>
        /// <remarks>
        /// Algorithm derived from http://aggregate.org/MAGIC/#Log2%20of%20an%20Integer; all credit goes to the original author.
        /// </remarks>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Log2Floor(ulong value) => (HammingWeight(FoldBits(value) >> 1));
        /// <summary>
        /// Constructs the check digit for a given numeric string using the Luhn algorithm; also known as the "mod 10" algorithm.
        /// </summary>
        /// <param name="value">The string that a check digit will be calculated from.</param>
        public static int Mod10(string value) {
            const int validCodePointCount = 10;
            const int validCodePointOffset = 48;

            var index = value.Length;
            var parity = true;
            var sum = 0;

            while (0 < index--) {
                var digit = (value[index] - validCodePointOffset);

                if (parity && ((validCodePointCount - 1) < (digit <<= 1))) {
                    digit -= (validCodePointCount - 1);
                }

                parity = !parity;
                sum += digit;
            }

            return ((validCodePointCount - (sum % validCodePointCount)) % validCodePointCount);
        }
        /// <summary>
        /// Calculates the most significant bit of the integer.
        /// </summary>
        /// <param name="value">The integer whose most significant bit will be calculated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MostSignificantBit(long value) => ((long)MostSignificantBit((ulong)value));
        /// <summary>
        /// Calculates the most significant bit of the integer.
        /// </summary>
        /// <param name="value">The integer whose most significant bit will be calculated.</param>
        /// <remarks>
        /// Algorithm derived from http://aggregate.org/MAGIC/#Most%20Significant%201%20Bit; all credit goes to the original author.
        /// </remarks>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong MostSignificantBit(ulong value) {
            value = FoldBits(value);

            return (value & ~(value >> 1));
        }
        /// <summary>
        /// Multiplies two 32-bit signed integers into a 64-bit result.
        /// </summary>
        /// <param name="x">The first integer that will be multiplied.</param>
        /// <param name="y">The second integer that will be multiplied.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Multiply(int x, int y) => (((long)x) * y);
        /// <summary>
        /// Multiplies two 32-bit unsigned integers into a 64-bit result.
        /// </summary>
        /// <param name="x">The first integer that will be multiplied.</param>
        /// <param name="y">The second integer that will be multiplied.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Multiply(uint x, uint y) => (((ulong)x) * y);
        /// <summary>
        /// Calculates the unique integer that results in 1 when multiplied by the input; such a value only exists for odd inputs.
        /// </summary>
        /// <param name="value">The integer that will be inverted.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ModularInverse(ulong value) {
            ulong result;

            result = ((3UL * value) ^ 2UL);
            result *= (2UL - (value * result));
            result *= (2UL - (value * result));
            result *= (2UL - (value * result));
            result *= (2UL - (value * result));

            return result;
        }
        /// <summary>
        /// Calculates the next largest power of two relative to the integer.
        /// </summary>
        /// <param name="value">The integer whose next largets power of two will be calculated.</param>
        /// <remarks>
        /// Algorithm derived from http://aggregate.org/MAGIC/#Next%20Largest%20Power%20of%202; all credit goes to the original author.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long NextPowerOf2(long value) => ((long)NextPowerOf2((ulong)value));
        /// <summary>
        /// Calculates the next largest power of two relative to the integer.
        /// </summary>
        /// <param name="value">The integer whose next largets power of two will be calculated.</param>
        /// <remarks>
        /// Algorithm derived from http://aggregate.org/MAGIC/#Next%20Largest%20Power%20of%202; all credit goes to the original author.
        /// </remarks>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong NextPowerOf2(ulong value) => (FoldBits(value) + 1UL);
        /// <summary>
        /// Reverses the order of the bytes that make up a value.
        /// </summary>
        /// <param name="value">The value that will have its bytes reversed.</param>
        /// <remarks>
        /// Algorithm derived from http://aggregate.org/MAGIC/#Bit%20Reversal; all credit goes to the original author.
        /// </remarks>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReverseBits(uint value) {
            var mask = 0x55555555U;

            value = (((value >> 1) & mask) | ((value & mask) << 1));
            mask = 0x33333333U;
            value = (((value >> 2) & mask) | ((value & mask) << 2));
            mask = 0X0F0F0F0FU;
            value = (((value >> 4) & mask) | ((value & mask) << 4));
            mask = 0X00FF00FFU;
            value = (((value >> 8) & mask) | ((value & mask) << 8));

            return ((value >> 16) | (value << 16));
        }
        /// <summary>
        /// Reverses the order of the bytes that make up a value.
        /// </summary>
        /// <param name="value">The value that will have its bytes reversed.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReverseBytes(uint value) => (RotateLeft((value & 0xFF00FF00U), 8) + RotateRight((value & 0x00FF00FFU), 8));
        /// <summary>
        /// Reverses the order of the bytes that make up a value.
        /// </summary>
        /// <param name="value">The value that will have its bytes reversed.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReverseBytes(ulong value) => (((ulong)ReverseBytes((uint)value) << 32) | ReverseBytes((uint)(value >> 32)));
        /// <summary>
        /// Permutes a value using by executing a circular shift to the left.
        /// </summary>
        /// <param name="value">The integer that will have its bits rotated.</param>
        /// <param name="count">The amount of times the permutation will be performed.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(uint value, int count) => ((value << count) | (value >> ((-count) & ((sizeof(uint) * 8) - 1))));
        /// <summary>
        /// Permutes a value using by executing a circular shift to the left.
        /// </summary>
        /// <param name="value">The integer that will have its bits rotated.</param>
        /// <param name="count">The amount of times the permutation will be performed.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(ulong value, int count) => ((value << count) | (value >> ((-count) & ((sizeof(ulong) * 8) - 1))));
        /// <summary>
        /// Permutes a value using by executing a circular shift to the right.
        /// </summary>
        /// <param name="value">The integer that will have its bits rotated.</param>
        /// <param name="count">The amount of times the permutation will be performed.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateRight(uint value, int count) => ((value >> count) | (value << ((-count) & ((sizeof(uint) * 8) - 1))));
        /// <summary>
        /// Permutes a value using by executing a circular shift to the right.
        /// </summary>
        /// <param name="value">The integer that will have its bits rotated.</param>
        /// <param name="count">The amount of times the permutation will be performed.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateRight(ulong value, int count) => ((value >> count) | (value << ((-count) & ((sizeof(ulong) * 8) - 1))));
        /// <summary>
        /// Indicates whether the specified integer is less than, greater than, or equal to zero.
        /// </summary>
        /// <param name="value">The integer that will be tested.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Sign(long value) => ((value >> ((sizeof(long) * 8) - 1)) | ((((~value) + 1L) >> ((sizeof(long) * 8) - 1)) & 1L));
        /// <summary>
        /// Performs an exclusive or operation between the elements of two spans; only the destination span is modified.
        /// </summary>
        /// <param name="source">The source span.</param>
        /// <param name="destination">The destination span.</param>
        public static void Xor(ReadOnlySpan<byte> source, Span<byte> destination) {
            var count = (destination.Length < source.Length ? destination.Length : source.Length);
            var offset = 0;

            if (Vector.IsHardwareAccelerated) {
                var vectorDestination = MemoryMarshal.Cast<byte, Vector<byte>>(destination.Slice(0, count));
                var vectorSource = MemoryMarshal.Cast<byte, Vector<byte>>(source.Slice(0, count));
                var vectorCount = vectorDestination.Length;

                for (var i = offset; (i < vectorCount); i++) {
                    vectorDestination[i] ^= vectorSource[i];
                }

                offset = (Vector<byte>.Count * vectorCount);
            }

            for (var i = offset; (i < count); i++) {
                destination[i] ^= source[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong FoldBits(ulong value) {
            value |= (value >> 1);
            value |= (value >> 2);
            value |= (value >> 4);
            value |= (value >> 8);
            value |= (value >> 16);

            return (value | (value >> 32));
        }
    }
}
