using System;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents an instance of the XChaCha20 stream cipher.
    /// </summary>
    public sealed class XChaCha20 : ChaCha
    {
        private const string NONCE_LENGTH_ERROR = ("nonce length must be exactly 24 bytes");

        /// <summary>
        /// The length of an XChaCha nonce (in bytes).
        /// </summary>
        public new const int NonceLength = 24;

        private readonly ulong m_iv;

        /// <summary>
        /// Initializes a new instance of the <see cref="XChaCha20"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        private XChaCha20(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ulong iv) : base(key, nonce) {
            m_iv = iv;
        }

        /// <summary>
        /// Creates a symmetric decryptor object with the current Key property and one-time use state parameter.
        /// </summary>
        /// <param name="key">The secret that will be used during decryption.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        public override ICryptoTransform CreateDecryptor(byte[] key, byte[] nonce) {
            if (nonce.Length != XChaCha20.NonceLength) {
                throw new ArgumentOutOfRangeException(actualValue: nonce.Length, message: NONCE_LENGTH_ERROR, paramName: nameof(nonce));
            }

            var derivedKey = ComputeHash(key, nonce.AsSpan(0, 16));
            var derivedNonce = (Span<byte>)stackalloc byte[ChaCha.NonceLength];

            BinaryPrimitives.WriteUInt32BigEndian(derivedNonce, ((uint)Operations.ExtractHigh(m_iv)));
            nonce.AsSpan(16, 8).CopyTo(derivedNonce.Slice(WordLength));

            return ChaChaTransform.New(Initialize(derivedKey, derivedNonce, ((uint)Operations.ExtractLow(m_iv))), m_iv, 20U);
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
            IVValue = SecureRandom.Default.GetBytes(24);
        }
        /// <summary>
        /// Generates a key to use for the algorithm.
        /// </summary>
        public override void GenerateKey() {
            KeyValue = SecureRandom.Default.GetBytes(KeySize >> 3);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XChaCha20"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        [CLSCompliant(false)]
        public static XChaCha20 New(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ulong iv) => new XChaCha20(key, nonce, iv);
        /// <summary>
        /// Initializes a new instance of the <see cref="XChaCha20"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        public static XChaCha20 New(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, long iv) => New(key, nonce, checked((ulong)iv));
    }
}
