using Crypto.Extensions;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents an instance of the <a href="https://github.com/Cyan4973/xxHash/blob/dev/doc/xxhash_spec.md">XxHash digest algorithm</a>.
    /// </summary>
    /// <remarks>
    /// https://cyan4973.github.io/xxHash/
    /// </remarks>
    public class XxHash : HashAlgorithm
    {
        private const int BLOCK_LENGTH_IN_BYTES = (WORD_LENGTH_IN_BYTES * 4);
        private const ulong PRIME0 = 0x9E3779B185EBCA87UL;
        private const ulong PRIME1 = 0xC2B2AE3D27D4EB4FUL;
        private const ulong PRIME2 = 0x165667B19E3779F9UL;
        private const ulong PRIME3 = 0x85EBCA77C2B2AE63UL;
        private const ulong PRIME4 = 0x27D4EB2F165667C5UL;
        private const int WORD_LENGTH_IN_BYTES = sizeof(ulong);

        private readonly byte[] m_buffer;
        private readonly ulong m_seed;

        private ulong m_accumulator0;
        private ulong m_accumulator1;
        private ulong m_accumulator2;
        private ulong m_accumulator3;
        private ulong m_count;
        private ulong m_hash;

        /// <summary>
        /// Gets the size, in bits, of the computed hash code.
        /// </summary>
        public override int HashSize => (WORD_LENGTH_IN_BYTES << 3);

        /// <summary>
        /// Initializes a new instance of the <see cref="XxHash"/> class.
        /// </summary>
        /// <param name="seed">The initial state of the digest algorithm.</param>
        private XxHash(ulong seed) {
            m_buffer = new byte[(BLOCK_LENGTH_IN_BYTES - 1)];
            m_seed = seed;

            Initialize();
        }

        /// <summary>
        /// Computes the hash value of the specified array using the XxHash algorithm.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <param name="dataOffset">The offset into the byte array from which to begin using data.</param>
        /// <param name="count">The number of bytes in the array to use as data.</param>
        public long ComputeHashAsLong(byte[] data, int dataOffset, int count) => ((long)ComputeHashAsULong(data, dataOffset, count));
        /// <summary>
        /// Computes the hash value of the specified array using the XxHash algorithm.
        /// </summary>
        /// <param name="data">The input data.</param>
        public long ComputeHashAsLong(byte[] data) => ((long)ComputeHashAsULong(data));
        /// <summary>
        /// Computes the hash value of the specified <see cref="Stream"/> using the XxHash algorithm.
        /// </summary>
        /// <param name="stream">The input data.</param>
        public long ComputeHashAsLong(Stream stream) => ((long)ComputeHashAsULong(stream));
        /// <summary>
        /// Computes the hash value of the specified array using the XxHash algorithm.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <param name="dataOffset">The offset into the byte array from which to begin using data.</param>
        /// <param name="count">The number of bytes in the array to use as data.</param>
        [CLSCompliant(false)]
        public ulong ComputeHashAsULong(byte[] data, int dataOffset, int count) {
            HashCore(data, dataOffset, count);

            return FinalizeAndReset();
        }
        /// <summary>
        /// Computes the hash value of the specified array using the XxHash algorithm.
        /// </summary>
        /// <param name="data">The input data.</param>
        [CLSCompliant(false)]
        public ulong ComputeHashAsULong(byte[] data) => ComputeHashAsULong(data, 0, data.Length);
        /// <summary>
        /// Computes the hash value of the specified <see cref="Stream"/> using the XxHash algorithm.
        /// </summary>
        /// <param name="stream">The input data.</param>
        [CLSCompliant(false)]
        public ulong ComputeHashAsULong(Stream stream) {
            if (stream.IsNotNull()) {
                var buffer = new byte[4096];
                var bufferLength = buffer.Length;
                var numBytesRead = 0;

                while (0 < (numBytesRead = stream.Read(buffer, 0, bufferLength))) {
                    HashCore(buffer, 0, numBytesRead);
                }
            }

            return FinalizeAndReset();
        }
        /// <summary>
        /// Routes data written to the object into the XxHash algorithm for computing the hash value.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <param name="dataOffset">The offset into the byte array from which to begin using data.</param>
        /// <param name="count">The number of bytes in the array to use as data.</param>
        protected override void HashCore(byte[] data, int dataOffset, int count) {
            if (data.IsNotNull()) {
                if (checked(dataOffset + count) > data.Length) {
                    throw new ArgumentOutOfRangeException(actualValue: (dataOffset + count), message: "(dataOffset + count) cannot exceed data length", paramName: nameof(dataOffset));
                }

                m_count = checked(m_count + ((ulong)count));

                while (BLOCK_LENGTH_IN_BYTES <= count) {
                    unsafe {
                        fixed (byte* d = &data[dataOffset]) {
                            if (BitConverter.IsLittleEndian) {
                                m_accumulator0 = Round(m_accumulator0, *(((ulong*)d) + 0));
                                m_accumulator1 = Round(m_accumulator1, *(((ulong*)d) + 1));
                                m_accumulator2 = Round(m_accumulator2, *(((ulong*)d) + 2));
                                m_accumulator3 = Round(m_accumulator3, *(((ulong*)d) + 3));
                            }
                            else {
                                m_accumulator0 = Round(m_accumulator0, Operations.ReverseBytes(*(((ulong*)d) + 0)));
                                m_accumulator1 = Round(m_accumulator1, Operations.ReverseBytes(*(((ulong*)d) + 1)));
                                m_accumulator2 = Round(m_accumulator2, Operations.ReverseBytes(*(((ulong*)d) + 2)));
                                m_accumulator3 = Round(m_accumulator3, Operations.ReverseBytes(*(((ulong*)d) + 3)));
                            }
                        }
                    }

                    checked {
                        count -= BLOCK_LENGTH_IN_BYTES;
                        dataOffset += BLOCK_LENGTH_IN_BYTES;
                    }
                }

                if (0 < count) {
                    Buffer.BlockCopy(data, dataOffset, m_buffer, 0, count);
                }
            }
        }
        /// <summary>
        /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
        /// </summary>
        protected override byte[] HashFinal() {
            var result = new byte[WORD_LENGTH_IN_BYTES];

            BinaryPrimitives.WriteUInt64BigEndian(result.AsSpan(), Finalize());

            return result;
        }
        /// <summary>
        /// Initializes internal state using the XxHash algorithm.
        /// </summary>
        public override void Initialize() {
            m_accumulator0 = (m_seed + PRIME0 + PRIME1);
            m_accumulator1 = (m_seed + PRIME1);
            m_accumulator2 = (m_seed + 0UL);
            m_accumulator3 = (m_seed - PRIME0);
            m_count = 0UL;
            m_hash = 0UL;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XxHash"/> class.
        /// </summary>
        /// <param name="seed">The initial state of the digest algorithm.</param>
        [CLSCompliant(false)]
        public static XxHash New(ulong seed) => new XxHash(seed);
        /// <summary>
        /// Initializes a new instance of the <see cref="XxHash"/> class.
        /// </summary>
        /// <param name="seed">The initial state of the digest algorithm.</param>
        public static XxHash New(long seed) => New((ulong)seed);
        /// <summary>
        /// Initializes a new instance of the <see cref="XxHash"/> class.
        /// </summary>
        public static XxHash New() => New(0UL);

        private ulong Finalize() {
            var buffer = m_buffer;
            var count = m_count;
            var hash = m_hash;
            var offset = 0;
            var remaining = (count & (BLOCK_LENGTH_IN_BYTES - 1));

            if (BLOCK_LENGTH_IN_BYTES <= count) {
                hash = (
                    Operations.RotateLeft(m_accumulator0, 1)
                  + Operations.RotateLeft(m_accumulator1, 7)
                  + Operations.RotateLeft(m_accumulator2, 12)
                  + Operations.RotateLeft(m_accumulator3, 18)
                );
                hash = MergeRound(hash, m_accumulator0);
                hash = MergeRound(hash, m_accumulator1);
                hash = MergeRound(hash, m_accumulator2);
                hash = MergeRound(hash, m_accumulator3);
            }
            else {
                hash = (m_seed + PRIME4);
            }

            hash += count;

            while (WORD_LENGTH_IN_BYTES <= remaining) {
                hash ^= Round(0, BinaryPrimitives.ReadUInt64LittleEndian(buffer.AsSpan(offset)));
                hash = ((Operations.RotateLeft(hash, 27) * PRIME0) + PRIME3);

                offset += WORD_LENGTH_IN_BYTES;
                remaining -= WORD_LENGTH_IN_BYTES;
            }

            while ((WORD_LENGTH_IN_BYTES >> 1) <= remaining) {
                hash ^= (BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(offset)) * PRIME0);
                hash = ((Operations.RotateLeft(hash, 23) * PRIME1) + PRIME2);

                offset += (WORD_LENGTH_IN_BYTES >> 1);
                remaining -= (WORD_LENGTH_IN_BYTES >> 1);
            }

            while (0 < remaining--) {
                hash ^= (buffer[offset++] * PRIME4);
                hash = (Operations.RotateLeft(hash, 11) * PRIME0);
            }

            return (m_hash = Avalanche(hash));
        }
        private ulong FinalizeAndReset() {
            var result = Finalize();

            HashValue = null;

            Initialize();

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Avalanche(ulong value) {
            value ^= value >> 33;
            value *= PRIME1;
            value ^= value >> 29;
            value *= PRIME2;

            return (value ^ (value >> BLOCK_LENGTH_IN_BYTES));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong MergeRound(ulong accumulator, ulong value) {
            value = Round(0UL, value);
            accumulator ^= value;

            return ((accumulator * PRIME0) + PRIME3);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Round(ulong accumulator, ulong value) {
            accumulator += (value * PRIME1);
            accumulator = Operations.RotateLeft(accumulator, (BLOCK_LENGTH_IN_BYTES - 1));

            return (accumulator * PRIME0);
        }
    }
}
