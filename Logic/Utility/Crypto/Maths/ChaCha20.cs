using System;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents an instance of the ChaCha20 stream cipher.
    /// </summary>
    public sealed class ChaCha20 : ChaCha
    {
        private readonly uint m_iv;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaCha20"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        private ChaCha20(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, uint iv) : base(key, nonce) {
            m_iv = iv;
        }

        /// <summary>
        /// Creates a symmetric decryptor object with the current Key property and one-time use state parameter.
        /// </summary>
        /// <param name="key">The secret that will be used during decryption.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        public override ICryptoTransform CreateDecryptor(byte[] key, byte[] nonce) {
            var ivLow = m_iv;
            var ivHigh = BinaryPrimitives.ReadUInt32LittleEndian(nonce.AsSpan(0, WordLength));

            return ChaChaTransform.New(Initialize(key, nonce, m_iv), Operations.ConcatBits(ivHigh, ivLow), 20U);
        }
        /// <summary>
        /// Creates a symmetric decryptor object with the current Key property and one-time use state parameter.
        /// </summary>
        /// <param name="key">The secret that will be used during encryption.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        public override ICryptoTransform CreateEncryptor(byte[] key, byte[] nonce) => CreateDecryptor(key, nonce);
        /// <summary>
        /// Generates a one-time use state parameter to use for the algorithm.
        /// </summary>
        public override void GenerateIV() {
            IVValue = SecureRandom.Default.GetBytes(12);
        }
        /// <summary>
        /// Generates a key to use for the algorithm.
        /// </summary>
        public override void GenerateKey() {
            KeyValue = SecureRandom.Default.GetBytes(KeySize >> 3);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaCha20"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        [CLSCompliant(false)]
        public static ChaCha20 New(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, uint iv) => new ChaCha20(key, nonce, iv);
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaCha20"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        [CLSCompliant(false)]
        public static ChaCha20 New(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, int iv) => New(key, nonce, checked((uint)iv));
    }
}
