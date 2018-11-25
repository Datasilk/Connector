using Crypto.Extensions;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents an instance of the <a href="https://en.wikipedia.org/wiki/Salsa20#ChaCha_variant">ChaCha family of stream ciphers</a>.
    /// </summary>
    /// <remarks>
    /// http://loup-vaillant.fr/tutorials/chacha20-design
    /// https://cr.yp.to/chacha/chacha-20080128.pdf
    /// https://cr.yp.to/snuffle/spec.pdf
    /// https://eprint.iacr.org/2013/759.pdf
    /// https://tools.ietf.org/html/rfc7539#section-2.3
    /// </remarks>
    public abstract class ChaCha : SymmetricAlgorithm
    {
        private const int BLOCK_LENGTH_IN_BITS = (BlockLength << 3);
        private const string KEY_LENGTH_ERROR = ("key length must be exactly 32 bytes");
        private const int KEY_LENGTH_IN_BITS = (KeyLength << 3);
        private const string NONCE_LENGTH_ERROR = ("nonce length must be exactly 12 bytes");
        private const uint SIGMA0 = 0x61707865U;
        private const uint SIGMA1 = 0x3320646EU;
        private const uint SIGMA2 = 0x79622D32U;
        private const uint SIGMA3 = 0x6B206574U;

        /// <summary>
        /// The length of a ChaCha block (in bytes).
        /// </summary>
        public const int BlockLength = (NumWordsPerBlock * WordLength);
        /// <summary>
        /// The length of a ChaCha key (in bytes).
        /// </summary>
        public const int KeyLength = 32;
        /// <summary>
        /// The length of a ChaCha nonce (in bytes).
        /// </summary>
        public const int NonceLength = 12;
        /// <summary>
        /// The number of words in a single ChaCha block.
        /// </summary>
        public const int NumWordsPerBlock = 16;
        /// <summary>
        /// The length of a ChaCha word (in bytes).
        /// </summary>
        public const int WordLength = sizeof(int);

        private static readonly KeySizes[] m_legalBlockSizes = { new KeySizes(BLOCK_LENGTH_IN_BITS, BLOCK_LENGTH_IN_BITS, 0), };
        private static readonly KeySizes[] m_legalKeySizes = { new KeySizes(KEY_LENGTH_IN_BITS, KEY_LENGTH_IN_BITS, 0), };

        /// <summary>
        /// Gets the length of a block (in bits).
        /// </summary>
        public override int BlockSize => base.BlockSize;
        /// <summary>
        /// Gets the amount of feedback added to each block (in bits).
        /// </summary>
        public override int FeedbackSize => base.FeedbackSize;
        /// <summary>
        /// Gets the one-time use state parameter as an array of bytes.
        /// </summary>
        public override byte[] IV => base.IV;
        /// Gets the key as an array of bytes.
        public override byte[] Key => base.Key;
        /// <summary>
        /// Gets the length of the key (in bits).
        /// </summary>
        public override int KeySize => base.KeySize;
        /// <summary>
        /// Gets the mode of operation.
        /// </summary>
        public override CipherMode Mode => base.Mode;
        /// <summary>
        /// Gets the padding mode.
        /// </summary>
        public override PaddingMode Padding => base.Padding;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaCha"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="iv">The initial value parameter.</param>
        protected ChaCha(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv) {
            BlockSizeValue = BLOCK_LENGTH_IN_BITS;
            FeedbackSizeValue = 0;
            IVValue = iv.ToArray();
            KeySizeValue = KEY_LENGTH_IN_BITS;
            KeyValue = key.ToArray();
            LegalBlockSizesValue = ((KeySizes[])m_legalBlockSizes.Clone());
            LegalKeySizesValue = ((KeySizes[])m_legalKeySizes.Clone());
            ModeValue = CipherMode.ECB;
            PaddingValue = PaddingMode.None;
        }

        /// <summary>
        /// Performs a cryptographic transform using the ChaCha stream cipher.
        /// </summary>
        /// <param name="source">The input data stream.</param>
        /// <param name="destination">The output data stream.</param>
        public void Transform(Stream source, Stream destination) {
            if (source.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(source));
            }

            if (!source.CanRead) {
                throw new ArgumentException(message: "source stream is not able to be read from", paramName: nameof(source));
            }

            if (destination.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(destination));
            }

            if (!destination.CanWrite) {
                throw new ArgumentException(message: "destination stream is not able to be written to", paramName: nameof(destination));
            }

            if (ReferenceEquals(source, destination) && (!destination.CanSeek)) {
                throw new ArgumentException(message: "stream must support seeking when it is used as both source and destination", paramName: nameof(source));
            }

            CryptoStream transformer = null;

            try {
                transformer = new CryptoStream(source, CreateEncryptor(Key, IV), CryptoStreamMode.Read);

                var bufferSize = 4096;
                var buffer = new byte[bufferSize];
                var numBytesRead = 0;

                while (0 < (numBytesRead = transformer.Read(buffer, 0, bufferSize))) {
                    if (source == destination) {
                        destination.Position -= numBytesRead;
                    }

                    destination.Write(buffer, 0, numBytesRead);
                }
            }
            finally {
                if (!transformer.HasFlushedFinalBlock) {
                    transformer.FlushFinalBlock();
                }
            }
        }
        /// <summary>
        /// Performs a cryptographic transform using the ChaCha stream cipher.
        /// </summary>
        /// <param name="stream">The data stream that will be transformed.</param>
        public void Transform(Stream stream) => Transform(stream, stream);
        /// <summary>
        /// Performs a cryptographic transform using the ChaCha stream cipher.
        /// </summary>
        /// <param name="data">The byte array that will be transformed.</param>
        public void Transform(byte[] data) {
            using (var memoryStream = new MemoryStream(data, true)) {
                Transform(memoryStream);
            }
        }

        /// <summary>
        /// Computes a hash of the key and nonce using the HChaCha algorithm.
        /// </summary>
        /// <param name="key">The secret that will be used during decryption.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        public static byte[] ComputeHash(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce) {
            var hash = new byte[(BlockLength >> 1)];
            var hashAsUInt = MemoryMarshal.Cast<byte, uint>(hash);
            var state = Initialize(key, nonce.Slice(WordLength), BinaryPrimitives.ReadUInt32LittleEndian(nonce.Slice(0, WordLength)));
            var stateSpan = state.AsSpan();
            var t0 = state[0];
            var t1 = state[1];
            var t2 = state[2];
            var t3 = state[3];
            var t4 = state[4];
            var t5 = state[5];
            var t6 = state[6];
            var t7 = state[7];
            var t8 = state[8];
            var t9 = state[9];
            var tA = state[10];
            var tB = state[11];
            var tC = state[12];
            var tD = state[13];
            var tE = state[14];
            var tF = state[15];

            for (var i = 0U; (i < 10U); i++) {
                DoubleRound(
                    ref t0, ref t1, ref t2, ref t3,
                    ref t4, ref t5, ref t6, ref t7,
                    ref t8, ref t9, ref tA, ref tB,
                    ref tC, ref tD, ref tE, ref tF
                );
            }

            state[0] = t0;
            state[1] = t1;
            state[2] = t2;
            state[3] = t3;
            state[4] = t4;
            state[5] = t5;
            state[6] = t6;
            state[7] = t7;
            state[8] = t8;
            state[9] = t9;
            state[10] = tA;
            state[11] = tB;
            state[12] = tC;
            state[13] = tD;
            state[14] = tE;
            state[15] = tF;

            stateSpan.Slice(0, WordLength).CopyTo(hashAsUInt.Slice(0, WordLength));
            stateSpan.Slice((NumWordsPerBlock - WordLength), WordLength).CopyTo(hashAsUInt.Slice(WordLength));

            return hash;
        }
        /// <summary>
        /// Executes eight QuarterRound operations (four "column-rounds" and four "row-rounds").
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DoubleRound(
            ref uint s0, ref uint s1, ref uint s2, ref uint s3,
            ref uint s4, ref uint s5, ref uint s6, ref uint s7,
            ref uint s8, ref uint s9, ref uint sA, ref uint sB,
            ref uint sC, ref uint sD, ref uint sE, ref uint sF
        ) {
            QuarterRound(ref s0, ref s4, ref s8, ref sC);
            QuarterRound(ref s1, ref s5, ref s9, ref sD);
            QuarterRound(ref s2, ref s6, ref sA, ref sE);
            QuarterRound(ref s3, ref s7, ref sB, ref sF);
            QuarterRound(ref s0, ref s5, ref sA, ref sF);
            QuarterRound(ref s1, ref s6, ref sB, ref sC);
            QuarterRound(ref s2, ref s7, ref s8, ref sD);
            QuarterRound(ref s3, ref s4, ref s9, ref sE);
        }
        /// <summary>
        /// Initializes a new ChaCha state array.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        [CLSCompliant(false)]
        public static uint[] Initialize(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, uint iv) {
            if (key.Length != KeyLength) {
                throw new ArgumentOutOfRangeException(actualValue: key.Length, message: KEY_LENGTH_ERROR, paramName: nameof(key));
            }

            if (nonce.Length != NonceLength) {
                throw new ArgumentOutOfRangeException(actualValue: nonce.Length, message: NONCE_LENGTH_ERROR, paramName: nameof(nonce));
            }

            var keyAsUInt = MemoryMarshal.Cast<byte, uint>(key);
            var nonceAsUInt = MemoryMarshal.Cast<byte, uint>(nonce);
            var state = new uint[NumWordsPerBlock];

            state[0] = SIGMA0;
            state[1] = SIGMA1;
            state[2] = SIGMA2;
            state[3] = SIGMA3;
            state[4] = keyAsUInt[0];
            state[5] = keyAsUInt[1];
            state[6] = keyAsUInt[2];
            state[7] = keyAsUInt[3];
            state[8] = keyAsUInt[4];
            state[9] = keyAsUInt[5];
            state[10] = keyAsUInt[6];
            state[11] = keyAsUInt[7];
            state[12] = iv;
            state[13] = nonceAsUInt[0];
            state[14] = nonceAsUInt[1];
            state[15] = nonceAsUInt[2];

            return state;
        }
        /// <summary>
        /// Executes four SingleRound operations.
        /// </summary>
        /// <param name="w">The w state parameter.</param>
        /// <param name="x">The x state parameter.</param>
        /// <param name="y">The y state parameter.</param>
        /// <param name="z">The z state parameter.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuarterRound(ref uint w, ref uint x, ref uint y, ref uint z) {
            SingleRound(ref w, ref z, x, 16);
            SingleRound(ref y, ref x, z, 12);
            SingleRound(ref w, ref z, x, 8);
            SingleRound(ref y, ref x, z, 7);
        }
        /// <summary>
        /// Executes the basic operation of the ChaCha algorithm which "mixes" three state variables per invocation.
        /// </summary>
        /// <param name="x">The x state parameter.</param>
        /// <param name="z">The y state parameter.</param>
        /// <param name="y">The z state parameter.</param>
        /// <param name="count">The number of rotations.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SingleRound(ref uint x, ref uint y, uint z, int count) {
            y = Operations.RotateLeft((y ^ (x += z)), count);
        }
        /// <summary>
        /// Writes a key stream block to a span using the specified parameters.
        /// </summary>
        /// <param name="state">The ChaCha state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        /// <param name="numRounds">The number of rounds to perform per block.</param>
        /// <param name="destination">The destination span.</param>
        [CLSCompliant(false)]
        public static void WriteKeyStreamBlock(ReadOnlySpan<uint> state, ulong iv, uint numRounds, Span<byte> destination) {
            var destinationAsUInt = MemoryMarshal.Cast<byte, uint>(destination);
            var ivLow = ((uint)Operations.ExtractLow(iv));
            var ivHigh = ((uint)Operations.ExtractHigh(iv));
            var t0 = state[0];
            var t1 = state[1];
            var t2 = state[2];
            var t3 = state[3];
            var t4 = state[4];
            var t5 = state[5];
            var t6 = state[6];
            var t7 = state[7];
            var t8 = state[8];
            var t9 = state[9];
            var tA = state[10];
            var tB = state[11];
            var tC = ivLow;
            var tD = ivHigh;
            var tE = state[14];
            var tF = state[15];

            numRounds = (numRounds >> 1);

            for (var i = 0U; (i < numRounds); i++) {
                DoubleRound(
                    ref t0, ref t1, ref t2, ref t3,
                    ref t4, ref t5, ref t6, ref t7,
                    ref t8, ref t9, ref tA, ref tB,
                    ref tC, ref tD, ref tE, ref tF
                );
            }

            destinationAsUInt[0] = (t0 + state[0]);
            destinationAsUInt[1] = (t1 + state[1]);
            destinationAsUInt[2] = (t2 + state[2]);
            destinationAsUInt[3] = (t3 + state[3]);
            destinationAsUInt[4] = (t4 + state[4]);
            destinationAsUInt[5] = (t5 + state[5]);
            destinationAsUInt[6] = (t6 + state[6]);
            destinationAsUInt[7] = (t7 + state[7]);
            destinationAsUInt[8] = (t8 + state[8]);
            destinationAsUInt[9] = (t9 + state[9]);
            destinationAsUInt[10] = (tA + state[10]);
            destinationAsUInt[11] = (tB + state[11]);
            destinationAsUInt[12] = (tC + ivLow);
            destinationAsUInt[13] = (tD + ivHigh);
            destinationAsUInt[14] = (tE + state[14]);
            destinationAsUInt[15] = (tF + state[15]);
        }
    }
}
