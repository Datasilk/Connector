using Crypto.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents an instance of the <a href="https://en.wikipedia.org/wiki/BLAKE_(hash_function)">Blake2b message authentication code generator</a>.
    /// </summary>
    /// <remarks>
    /// https://blake2.net/
    /// https://github.com/BLAKE2/BLAKE2/blob/master/testvectors/blake2b-kat.txt
    /// https://tools.ietf.org/html/rfc7693
    /// </remarks>
    public sealed class Blake2b : KeyedHashAlgorithm
    {
        private const int BLOCK_LENGTH_IN_BYTES = (NUM_WORDS_IN_BLOCK * sizeof(ulong));
        private const int HASH_LENGTH_IN_BYTES_MAX = (BLOCK_LENGTH_IN_BYTES >> 1);
        private const ulong IV0 = 0x6A09E667F3BCC908UL;
        private const ulong IV1 = 0xBB67AE8584CAA73BUL;
        private const ulong IV2 = 0x3C6EF372FE94F82BUL;
        private const ulong IV3 = 0xA54FF53A5F1D36F1UL;
        private const ulong IV4 = 0x510E527FADE682D1UL;
        private const ulong IV5 = 0x9B05688C2B3E6C1FUL;
        private const ulong IV6 = 0x1F83D9ABFB41BD6BUL;
        private const ulong IV7 = 0x5BE0CD19137E2179UL;
        private const int KEY_LENGTH_IN_BYTES_MAX = HASH_LENGTH_IN_BYTES_MAX;
        private const int NUM_WORDS_IN_BLOCK = 16;

        private readonly byte[] m_buffer;
        private readonly int m_hashLengthInBytes;
        private readonly ulong[] m_state;

        private bool m_compressFirstBlock;
        private ulong m_iv0;
        private ulong m_iv1;

        /// <summary>
        /// Gets the length of the computed hash code (in bits).
        /// </summary>
        public override int HashSize => (m_hashLengthInBytes << 3);

        /// <summary>
        /// Initializes a new instance of the <see cref="Blake2b"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="hashLength">The length of the hash value (in bytes).</param>
        private Blake2b(ReadOnlySpan<byte> key, int hashLength) {
            m_buffer = new byte[BLOCK_LENGTH_IN_BYTES];
            m_hashLengthInBytes = hashLength;
            m_state = new ulong[NUM_WORDS_IN_BLOCK];

            Key = key.ToArray();

            Initialize();
        }

        /// <summary>
        /// Routes data written to the object into the Blake2s algorithm for computing the hash value.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <param name="dataOffset">The offset into the byte array from which to begin using data.</param>
        /// <param name="count">The number of bytes in the array to use as data.</param>
        protected override void HashCore(byte[] data, int dataOffset, int count) {
            if (data.IsNotNull()) {
                var buffer = m_buffer;
                var iv0 = m_iv0;
                var iv1 = m_iv1;
                var state = m_state;

                // compress key block
                if ((0 < count) && m_compressFirstBlock) {
                    Compress(buffer, state, iv0, iv1, IV6);

                    m_compressFirstBlock = false;
                }

                // compress data blocks
                while (BLOCK_LENGTH_IN_BYTES < count) {
                    iv0 += BLOCK_LENGTH_IN_BYTES;
                    iv1 += ((iv0 < BLOCK_LENGTH_IN_BYTES) ? 1UL : 0UL);

                    Compress(data.AsSpan(dataOffset, BLOCK_LENGTH_IN_BYTES), state, iv0, iv1, IV6);

                    count -= BLOCK_LENGTH_IN_BYTES;
                    dataOffset += BLOCK_LENGTH_IN_BYTES;
                }

                // read last data chunk
                if (0 < count) {
                    Buffer.BlockCopy(data, dataOffset, buffer, 0, count);

                    // zero unused buffer indices
                    for (var i = count; (i < BLOCK_LENGTH_IN_BYTES); i++) {
                        buffer[i] = 0;
                    }

                    iv0 += ((ulong)count);
                    iv1 += ((0UL == iv0) ? 1UL : 0UL);
                }

                m_iv0 = iv0;
                m_iv1 = iv1;
            }
        }
        /// <summary>
        /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
        /// </summary>
        protected override byte[] HashFinal() {
            // compress final block
            Compress(m_buffer, m_state, m_iv0, m_iv1, ~IV6);

            // convert state to little endian format and return hash value
            var hashLength = m_hashLengthInBytes;
            var result = new byte[hashLength];

            unsafe {
                fixed (ulong* s = &m_state[0]) {
                    for (var i = 0; (i < hashLength); i++) {
                        result[i] = ((byte)(((*(s + (i >> 3))) >> (8 * (i & 7))) & 0xFF));
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Initializes internal state using the Blake2b algorithm.
        /// </summary>
        public override void Initialize() {
            var blockBuffer = m_buffer;
            var bufferLength = blockBuffer.Length;
            var compressFirstBlock = false;
            var hashLength = m_hashLengthInBytes;
            var iv0 = 0UL;
            var iv1 = 0UL;
            var keyLength = 0;
            var keyValue = KeyValue;
            var state = m_state;

            if (keyValue.IsNotNull() && (0 < keyValue.Length)) {
                // set key as first data block
                keyLength = keyValue.Length;

                for (var i = 0; (i < keyLength); i++) {
                    blockBuffer[i] = keyValue[i];
                }

                compressFirstBlock = true;
                iv0 += BLOCK_LENGTH_IN_BYTES;
            }

            // zero unused buffer indices
            for (var i = keyLength; (i < bufferLength); i++) {
                blockBuffer[i] = 0;
            }

            m_compressFirstBlock = compressFirstBlock;
            m_iv0 = iv0;
            m_iv1 = iv1;

            Initialize(state, hashLength, keyLength);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blake2b"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="hashLength">The length of the hash value (in bytes).</param>
        public static Blake2b New(ReadOnlySpan<byte> key, int hashLength) => new Blake2b(key, hashLength);
        /// <summary>
        /// Initializes a new instance of the <see cref="Blake2b"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        public static Blake2b New(ReadOnlySpan<byte> key) => New(key, HASH_LENGTH_IN_BYTES_MAX);
        /// <summary>
        /// Initializes a new instance of the <see cref="Blake2b"/> class.
        /// </summary>
        public static Blake2b New() => New(SecureRandom.Default.GetBytes(KEY_LENGTH_IN_BYTES_MAX));

        private static void Compress(ReadOnlySpan<byte> data, Span<ulong> state, ulong ivLow, ulong ivHigh, ulong finalizer) {
            var dataAsULong = MemoryMarshal.Cast<byte, ulong>(data);
            var d0 = dataAsULong[0];
            var d1 = dataAsULong[1];
            var d2 = dataAsULong[2];
            var d3 = dataAsULong[3];
            var d4 = dataAsULong[4];
            var d5 = dataAsULong[5];
            var d6 = dataAsULong[6];
            var d7 = dataAsULong[7];
            var d8 = dataAsULong[8];
            var d9 = dataAsULong[9];
            var dA = dataAsULong[10];
            var dB = dataAsULong[11];
            var dC = dataAsULong[12];
            var dD = dataAsULong[13];
            var dE = dataAsULong[14];
            var dF = dataAsULong[15];
            var t0 = state[0];
            var t1 = state[1];
            var t2 = state[2];
            var t3 = state[3];
            var t4 = state[4];
            var t5 = state[5];
            var t6 = state[6];
            var t7 = state[7];
            var t8 = IV0;
            var t9 = IV1;
            var tA = IV2;
            var tB = IV3;
            var tC = (IV4 ^ ivLow);
            var tD = (IV5 ^ ivHigh);
            var tE = finalizer;
            var tF = IV7;

            // round 0
            t0 += t4;
            t0 += d0;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 32);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 24);
            t0 += t4;
            t0 += d1;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 16);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 63);

            t1 += t5;
            t1 += d2;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 32);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 24);
            t1 += t5;
            t1 += d3;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 16);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 63);

            t2 += t6;
            t2 += d4;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 32);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 24);
            t2 += t6;
            t2 += d5;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 16);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 63);

            t3 += t7;
            t3 += d6;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 32);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 24);
            t3 += t7;
            t3 += d7;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 16);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 63);

            t0 += t5;
            t0 += d8;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 32);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 24);
            t0 += t5;
            t0 += d9;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 16);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 63);

            t1 += t6;
            t1 += dA;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 32);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 24);
            t1 += t6;
            t1 += dB;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 16);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 63);

            t2 += t7;
            t2 += dC;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 32);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 24);
            t2 += t7;
            t2 += dD;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 16);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 63);

            t3 += t4;
            t3 += dE;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 32);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 24);
            t3 += t4;
            t3 += dF;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 16);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 63);

            // round 1
            t0 += t4;
            t0 += dE;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 32);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 24);
            t0 += t4;
            t0 += dA;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 16);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 63);

            t1 += t5;
            t1 += d4;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 32);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 24);
            t1 += t5;
            t1 += d8;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 16);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 63);

            t2 += t6;
            t2 += d9;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 32);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 24);
            t2 += t6;
            t2 += dF;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 16);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 63);

            t3 += t7;
            t3 += dD;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 32);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 24);
            t3 += t7;
            t3 += d6;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 16);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 63);

            t0 += t5;
            t0 += d1;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 32);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 24);
            t0 += t5;
            t0 += dC;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 16);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 63);

            t1 += t6;
            t1 += d0;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 32);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 24);
            t1 += t6;
            t1 += d2;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 16);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 63);

            t2 += t7;
            t2 += dB;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 32);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 24);
            t2 += t7;
            t2 += d7;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 16);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 63);

            t3 += t4;
            t3 += d5;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 32);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 24);
            t3 += t4;
            t3 += d3;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 16);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 63);

            // round 2
            t0 += t4;
            t0 += dB;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 32);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 24);
            t0 += t4;
            t0 += d8;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 16);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 63);

            t1 += t5;
            t1 += dC;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 32);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 24);
            t1 += t5;
            t1 += d0;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 16);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 63);

            t2 += t6;
            t2 += d5;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 32);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 24);
            t2 += t6;
            t2 += d2;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 16);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 63);

            t3 += t7;
            t3 += dF;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 32);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 24);
            t3 += t7;
            t3 += dD;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 16);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 63);

            t0 += t5;
            t0 += dA;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 32);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 24);
            t0 += t5;
            t0 += dE;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 16);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 63);

            t1 += t6;
            t1 += d3;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 32);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 24);
            t1 += t6;
            t1 += d6;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 16);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 63);

            t2 += t7;
            t2 += d7;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 32);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 24);
            t2 += t7;
            t2 += d1;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 16);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 63);

            t3 += t4;
            t3 += d9;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 32);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 24);
            t3 += t4;
            t3 += d4;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 16);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 63);

            // round 3
            t0 += t4;
            t0 += d7;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 32);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 24);
            t0 += t4;
            t0 += d9;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 16);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 63);

            t1 += t5;
            t1 += d3;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 32);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 24);
            t1 += t5;
            t1 += d1;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 16);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 63);

            t2 += t6;
            t2 += dD;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 32);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 24);
            t2 += t6;
            t2 += dC;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 16);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 63);

            t3 += t7;
            t3 += dB;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 32);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 24);
            t3 += t7;
            t3 += dE;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 16);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 63);

            t0 += t5;
            t0 += d2;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 32);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 24);
            t0 += t5;
            t0 += d6;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 16);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 63);

            t1 += t6;
            t1 += d5;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 32);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 24);
            t1 += t6;
            t1 += dA;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 16);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 63);

            t2 += t7;
            t2 += d4;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 32);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 24);
            t2 += t7;
            t2 += d0;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 16);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 63);

            t3 += t4;
            t3 += dF;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 32);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 24);
            t3 += t4;
            t3 += d8;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 16);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 63);

            // round 4
            t0 += t4;
            t0 += d9;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 32);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 24);
            t0 += t4;
            t0 += d0;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 16);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 63);

            t1 += t5;
            t1 += d5;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 32);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 24);
            t1 += t5;
            t1 += d7;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 16);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 63);

            t2 += t6;
            t2 += d2;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 32);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 24);
            t2 += t6;
            t2 += d4;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 16);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 63);

            t3 += t7;
            t3 += dA;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 32);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 24);
            t3 += t7;
            t3 += dF;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 16);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 63);

            t0 += t5;
            t0 += dE;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 32);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 24);
            t0 += t5;
            t0 += d1;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 16);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 63);

            t1 += t6;
            t1 += dB;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 32);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 24);
            t1 += t6;
            t1 += dC;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 16);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 63);

            t2 += t7;
            t2 += d6;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 32);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 24);
            t2 += t7;
            t2 += d8;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 16);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 63);

            t3 += t4;
            t3 += d3;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 32);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 24);
            t3 += t4;
            t3 += dD;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 16);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 63);

            // round 5
            t0 += t4;
            t0 += d2;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 32);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 24);
            t0 += t4;
            t0 += dC;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 16);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 63);

            t1 += t5;
            t1 += d6;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 32);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 24);
            t1 += t5;
            t1 += dA;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 16);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 63);

            t2 += t6;
            t2 += d0;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 32);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 24);
            t2 += t6;
            t2 += dB;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 16);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 63);

            t3 += t7;
            t3 += d8;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 32);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 24);
            t3 += t7;
            t3 += d3;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 16);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 63);

            t0 += t5;
            t0 += d4;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 32);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 24);
            t0 += t5;
            t0 += dD;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 16);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 63);

            t1 += t6;
            t1 += d7;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 32);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 24);
            t1 += t6;
            t1 += d5;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 16);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 63);

            t2 += t7;
            t2 += dF;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 32);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 24);
            t2 += t7;
            t2 += dE;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 16);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 63);

            t3 += t4;
            t3 += d1;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 32);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 24);
            t3 += t4;
            t3 += d9;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 16);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 63);

            // round 6
            t0 += t4;
            t0 += dC;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 32);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 24);
            t0 += t4;
            t0 += d5;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 16);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 63);

            t1 += t5;
            t1 += d1;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 32);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 24);
            t1 += t5;
            t1 += dF;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 16);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 63);

            t2 += t6;
            t2 += dE;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 32);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 24);
            t2 += t6;
            t2 += dD;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 16);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 63);

            t3 += t7;
            t3 += d4;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 32);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 24);
            t3 += t7;
            t3 += dA;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 16);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 63);

            t0 += t5;
            t0 += d0;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 32);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 24);
            t0 += t5;
            t0 += d7;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 16);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 63);

            t1 += t6;
            t1 += d6;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 32);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 24);
            t1 += t6;
            t1 += d3;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 16);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 63);

            t2 += t7;
            t2 += d9;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 32);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 24);
            t2 += t7;
            t2 += d2;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 16);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 63);

            t3 += t4;
            t3 += d8;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 32);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 24);
            t3 += t4;
            t3 += dB;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 16);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 63);

            // round 7
            t0 += t4;
            t0 += dD;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 32);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 24);
            t0 += t4;
            t0 += dB;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 16);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 63);

            t1 += t5;
            t1 += d7;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 32);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 24);
            t1 += t5;
            t1 += dE;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 16);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 63);

            t2 += t6;
            t2 += dC;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 32);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 24);
            t2 += t6;
            t2 += d1;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 16);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 63);

            t3 += t7;
            t3 += d3;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 32);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 24);
            t3 += t7;
            t3 += d9;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 16);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 63);

            t0 += t5;
            t0 += d5;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 32);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 24);
            t0 += t5;
            t0 += d0;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 16);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 63);

            t1 += t6;
            t1 += dF;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 32);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 24);
            t1 += t6;
            t1 += d4;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 16);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 63);

            t2 += t7;
            t2 += d8;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 32);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 24);
            t2 += t7;
            t2 += d6;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 16);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 63);

            t3 += t4;
            t3 += d2;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 32);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 24);
            t3 += t4;
            t3 += dA;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 16);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 63);

            // round 8
            t0 += t4;
            t0 += d6;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 32);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 24);
            t0 += t4;
            t0 += dF;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 16);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 63);

            t1 += t5;
            t1 += dE;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 32);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 24);
            t1 += t5;
            t1 += d9;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 16);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 63);

            t2 += t6;
            t2 += dB;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 32);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 24);
            t2 += t6;
            t2 += d3;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 16);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 63);

            t3 += t7;
            t3 += d0;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 32);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 24);
            t3 += t7;
            t3 += d8;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 16);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 63);

            t0 += t5;
            t0 += dC;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 32);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 24);
            t0 += t5;
            t0 += d2;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 16);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 63);

            t1 += t6;
            t1 += dD;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 32);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 24);
            t1 += t6;
            t1 += d7;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 16);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 63);

            t2 += t7;
            t2 += d1;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 32);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 24);
            t2 += t7;
            t2 += d4;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 16);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 63);

            t3 += t4;
            t3 += dA;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 32);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 24);
            t3 += t4;
            t3 += d5;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 16);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 63);

            // round 9
            t0 += t4;
            t0 += dA;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 32);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 24);
            t0 += t4;
            t0 += d2;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 16);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 63);

            t1 += t5;
            t1 += d8;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 32);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 24);
            t1 += t5;
            t1 += d4;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 16);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 63);

            t2 += t6;
            t2 += d7;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 32);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 24);
            t2 += t6;
            t2 += d6;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 16);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 63);

            t3 += t7;
            t3 += d1;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 32);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 24);
            t3 += t7;
            t3 += d5;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 16);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 63);

            t0 += t5;
            t0 += dF;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 32);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 24);
            t0 += t5;
            t0 += dB;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 16);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 63);

            t1 += t6;
            t1 += d9;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 32);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 24);
            t1 += t6;
            t1 += dE;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 16);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 63);

            t2 += t7;
            t2 += d3;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 32);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 24);
            t2 += t7;
            t2 += dC;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 16);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 63);

            t3 += t4;
            t3 += dD;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 32);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 24);
            t3 += t4;
            t3 += d0;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 16);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 63);

            // round 10
            t0 += t4;
            t0 += d0;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 32);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 24);
            t0 += t4;
            t0 += d1;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 16);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 63);

            t1 += t5;
            t1 += d2;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 32);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 24);
            t1 += t5;
            t1 += d3;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 16);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 63);

            t2 += t6;
            t2 += d4;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 32);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 24);
            t2 += t6;
            t2 += d5;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 16);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 63);

            t3 += t7;
            t3 += d6;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 32);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 24);
            t3 += t7;
            t3 += d7;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 16);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 63);

            t0 += t5;
            t0 += d8;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 32);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 24);
            t0 += t5;
            t0 += d9;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 16);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 63);

            t1 += t6;
            t1 += dA;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 32);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 24);
            t1 += t6;
            t1 += dB;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 16);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 63);

            t2 += t7;
            t2 += dC;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 32);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 24);
            t2 += t7;
            t2 += dD;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 16);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 63);

            t3 += t4;
            t3 += dE;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 32);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 24);
            t3 += t4;
            t3 += dF;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 16);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 63);

            // round 11
            t0 += t4;
            t0 += dE;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 32);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 24);
            t0 += t4;
            t0 += dA;
            tC ^= t0;
            tC = Operations.RotateRight(tC, 16);
            t8 += tC;
            t4 ^= t8;
            t4 = Operations.RotateRight(t4, 63);

            t1 += t5;
            t1 += d4;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 32);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 24);
            t1 += t5;
            t1 += d8;
            tD ^= t1;
            tD = Operations.RotateRight(tD, 16);
            t9 += tD;
            t5 ^= t9;
            t5 = Operations.RotateRight(t5, 63);

            t2 += t6;
            t2 += d9;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 32);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 24);
            t2 += t6;
            t2 += dF;
            tE ^= t2;
            tE = Operations.RotateRight(tE, 16);
            tA += tE;
            t6 ^= tA;
            t6 = Operations.RotateRight(t6, 63);

            t3 += t7;
            t3 += dD;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 32);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 24);
            t3 += t7;
            t3 += d6;
            tF ^= t3;
            tF = Operations.RotateRight(tF, 16);
            tB += tF;
            t7 ^= tB;
            t7 = Operations.RotateRight(t7, 63);

            t0 += t5;
            t0 += d1;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 32);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 24);
            t0 += t5;
            t0 += dC;
            tF ^= t0;
            tF = Operations.RotateRight(tF, 16);
            tA += tF;
            t5 ^= tA;
            t5 = Operations.RotateRight(t5, 63);

            t1 += t6;
            t1 += d0;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 32);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 24);
            t1 += t6;
            t1 += d2;
            tC ^= t1;
            tC = Operations.RotateRight(tC, 16);
            tB += tC;
            t6 ^= tB;
            t6 = Operations.RotateRight(t6, 63);

            t2 += t7;
            t2 += dB;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 32);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 24);
            t2 += t7;
            t2 += d7;
            tD ^= t2;
            tD = Operations.RotateRight(tD, 16);
            t8 += tD;
            t7 ^= t8;
            t7 = Operations.RotateRight(t7, 63);

            t3 += t4;
            t3 += d5;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 32);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 24);
            t3 += t4;
            t3 += d3;
            tE ^= t3;
            tE = Operations.RotateRight(tE, 16);
            t9 += tE;
            t4 ^= t9;
            t4 = Operations.RotateRight(t4, 63);

            state[0] ^= (t0 ^ t8);
            state[1] ^= (t1 ^ t9);
            state[2] ^= (t2 ^ tA);
            state[3] ^= (t3 ^ tB);
            state[4] ^= (t4 ^ tC);
            state[5] ^= (t5 ^ tD);
            state[6] ^= (t6 ^ tE);
            state[7] ^= (t7 ^ tF);
        }
        private static void Initialize(Span<ulong> state, int hashLengthInBytes, int keyLengthInBytes) {
            var param0 = 0UL;

            param0 |= (((ulong)hashLengthInBytes) << 0);
            param0 |= (((ulong)keyLengthInBytes) << 8);
            param0 |= (1UL << 16); // tree fanout
            param0 |= (1UL << 24); // tree depth

            state[0] = (IV0 ^ param0);
            state[1] = IV1;
            state[2] = IV2;
            state[3] = IV3;
            state[4] = IV4;
            state[5] = IV5;
            state[6] = IV6;
            state[7] = IV7;
        }
    }
}
