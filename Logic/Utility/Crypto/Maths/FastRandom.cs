using System;
using System.Runtime.CompilerServices;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents an instance of a fast random number generator; relies on the <a href="https://en.wikipedia.org/wiki/Permuted_congruential_generator">Pcg32XshRr algorithm</a>.
    /// </summary>
    /// <remarks>
    /// http://pcg-random.org/paper.html
    /// https://en.wikipedia.org/wiki/Lehmer_random_number_generator
    /// https://en.wikipedia.org/wiki/Linear_congruential_generator
    /// https://github.com/lemire/fastrange
    /// </remarks>
    public sealed class FastRandom : IRandomNumberGenerator
    {
        private const ulong MAGIC_VALUE_DEFAULT = 6364136223846793005UL;
        private const string STREAM_VALUE_ERROR = "stream offset must be a positive integer less than 2^63";
        private const ulong STREAM_VALUE_MAX = ((1UL << 63) - 1UL);

        private readonly ulong m_magic;
        private readonly ulong m_stream;

        private ulong m_state;

        /// <summary>
        /// Gets the default <see cref="FastRandom"/> instance.
        /// </summary>
        public static readonly FastRandom Default = New();

        /// <summary>
        /// Initializes a new instance of the <see cref="FastRandom"/> class.
        /// </summary>
        /// <param name="state">The initial state of the random number generator.</param>
        /// <param name="stream">The stream offset of the random number generator.</param>
        /// <param name="magic">The multiplier of the random number generator.</param>
        private FastRandom(ulong state, ulong stream, ulong magic) {
            if (stream > STREAM_VALUE_MAX) {
                throw new ArgumentOutOfRangeException(actualValue: stream, message: STREAM_VALUE_ERROR, paramName: nameof(stream));
            }

            m_magic = magic;
            m_state = state;
            m_stream = ((((~(stream & 1UL)) << 63) | stream) | 1UL);
        }

        /// <summary>
        /// Moves the state of the generator forwards by the specificed number of steps.
        /// </summary>
        /// <param name="count">The number of states that will be jumped.</param>
        [CLSCompliant(false)]
        public void Jump(ulong count) {
            m_state = Jump(m_state, m_magic, m_stream, count);
        }
        /// <summary>
        /// Moves the state of the generator forwards or backwards by the specificed number of steps.
        /// </summary>
        /// <param name="count">The number of states that will be jumped.</param>
        public void Jump(long count) => Jump((ulong)count);
        /// <summary>
        /// Generates a uniformly distributed double precision floating point value between the exclusive range (0, 1).
        /// </summary>
        public double NextDouble() => Operations.DoornikDouble(NextInt32(), NextInt32());
        /// <summary>
        /// Generates a uniformly distributed 32-bit signed integer between the range of int.MinValue and int.MaxValue.
        /// </summary>
        public int NextInt32() => ((int)NextUInt32());
        /// <summary>
        /// Generates a uniformly distributed 32-bit signed integer between the inclusive range [x, y].
        /// </summary>
        /// <param name="x">The value of x.</param>
        /// <param name="y">The value of y.</param>
        public int NextInt32(int x, int y) {
            if (x > y) {
                var z = x;

                x = y;
                y = z;
            }

            var range = ((uint)(y - x));

            if (range != uint.MaxValue) {
                return ((int)(Sample(ref m_state, m_magic, m_stream, exclusiveHigh: (range + 1U)) + x));
            }
            else {
                return NextInt32();
            }
        }
        /// <summary>
        /// Generates a uniformly distributed single precision floating point value between the exclusive range (0, 1).
        /// </summary>
        public float NextSingle() => Operations.DoornikSingle(NextInt32());
        /// <summary>
        /// Generates a uniformly distributed 32-bit unsigned integer between the range of uint.MinValue and uint.MaxValue.
        /// </summary>
        [CLSCompliant(false)]
        public uint NextUInt32() => Next(ref m_state, m_magic, m_stream);
        /// <summary>
        /// Generates a uniformly distributed 32-bit unsigned integer between the inclusive range [x, y].
        /// </summary>
        /// <param name="x">The value of x.</param>
        /// <param name="y">The value of y.</param>
        [CLSCompliant(false)]
        public uint NextUInt32(uint x, uint y) {
            if (x > y) {
                var z = x;

                x = y;
                y = z;
            }

            var range = (y - x);

            if (range != uint.MaxValue) {
                return (Sample(ref m_state, m_magic, m_stream, exclusiveHigh: (range + 1U)) + x);
            }
            else {
                return NextUInt32();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastRandom"/> class.
        /// </summary>
        /// <param name="state">The initial state of the random number generator.</param>
        /// <param name="stream">The stream offset of the random number generator.</param>
        /// <param name="magic">The multiplier of the random number generator.</param>
        [CLSCompliant(false)]
        public static FastRandom New(ulong state, ulong stream, ulong magic) => new FastRandom(state, stream, magic);
        /// <summary>
        /// Initializes a new instance of the <see cref="FastRandom"/> class.
        /// </summary>
        /// <param name="state">The initial state of the random number generator.</param>
        /// <param name="stream">The stream offset of the random number generator.</param>
        /// <param name="magic">The multiplier of the random number generator.</param>
        public static FastRandom New(long state, long stream, long magic) => New(checked((ulong)state), checked((ulong)stream), checked((ulong)magic));
        /// <summary>
        /// Initializes a new instance of the <see cref="FastRandom"/> class.
        /// </summary>
        /// <param name="state">The initial state of the random number generator.</param>
        /// <param name="stream">The stream offset of the random number generator.</param>
        [CLSCompliant(false)]
        public static FastRandom New(ulong state, ulong stream) => New(state, stream, MAGIC_VALUE_DEFAULT);
        /// <summary>
        /// Initializes a new instance of the <see cref="FastRandom"/> class.
        /// </summary>
        /// <param name="state">The initial state of the random number generator.</param>
        /// <param name="stream">The stream offset of the random number generator.</param>
        public static FastRandom New(long state, long stream) => New(checked((ulong)state), checked((ulong)stream));
        /// <summary>
        /// Initializes a new instance of the <see cref="FastRandom"/> class.
        /// </summary>
        /// <param name="state">The initial state of the random number generator.</param>
        [CLSCompliant(false)]
        public static FastRandom New(ulong state) => New(state, SecureRandom.Default.NextUInt64(0UL, STREAM_VALUE_MAX));
        /// <summary>
        /// Initializes a new instance of the <see cref="FastRandom"/> class.
        /// </summary>
        /// <param name="state">The initial state of the random number generator.</param>
        public static FastRandom New(long state) => New(checked((ulong)state));
        /// <summary>
        /// Initializes a new instance of the <see cref="FastRandom"/> class.
        /// </summary>
        public static FastRandom New() => New(SecureRandom.Default.NextUInt64());

        private static ulong Jump(ulong state, ulong magic, ulong stream, ulong count) {
            var accMul = 1UL;
            var accAdd = 0UL;
            var curMul = magic;
            var curAdd = stream;

            while (count > 0UL) {
                if (0UL < (count & 1UL)) {
                    accMul *= curMul;
                    accAdd = ((accAdd * curMul) + curAdd);
                }

                curAdd = ((curMul + 1UL) * curAdd);
                curMul *= curMul;
                count >>= 1;
            }

            return ((accMul * state) + accAdd);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Next(ref ulong state, ulong magic, ulong stream) {
            state = ((state * magic) + stream);

            return Operations.RotateRight(value: ((uint)(((state >> 18) ^ state) >> 27)), count: ((int)(state >> 59)));
        }
        private static uint Sample(ref ulong state, ulong magic, ulong stream, uint exclusiveHigh) {
            ulong sample = Next(ref state, magic, stream);
            ulong result = (sample * exclusiveHigh);
            uint leftover = ((uint)result);

            if (leftover < exclusiveHigh) {
                uint threshold = ((((uint)(-exclusiveHigh)) % exclusiveHigh));

                while (leftover < threshold) {
                    sample = Next(ref state, magic, stream);
                    result = (sample * exclusiveHigh);
                    leftover = ((uint)result);
                }
            }

            return ((uint)(result >> 32));
        }
    }
}
