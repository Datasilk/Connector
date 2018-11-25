using Crypto.Extensions;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Security.Cryptography;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents an instance of the <a href="https://tools.ietf.org/html/rfc7539">ChaCha20-Poly1305 AEAD algorithm</a>.
    /// </summary>
    public class AeadChaCha20Poly1305
    {
        private const string MESSAGE_AUTHENTICATION_ERROR = "unable to decrypt, failed to authenticate stream data";
        private const int TAG_LENGTH_IN_BYTES = 16;
        private const string WORK_STREAM_ERROR = "work stream must be different than the source and destination streams";

        private readonly byte[] m_aad;
        private readonly ChaCha20 m_chaCha20;
        private readonly Poly1305 m_poly1305;

        /// <summary>
        /// Initializes a new instance of the <see cref="AeadChaCha20Poly1305"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="aad">The arbitrary length additional authenticated data parameter.</param>
        public AeadChaCha20Poly1305(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> aad) {
            var ivLow = 0U;
            var ivHigh = BinaryPrimitives.ReadUInt32LittleEndian(nonce);
            var oneTimeKey = new byte[64];

            ChaCha.WriteKeyStreamBlock(ChaCha.Initialize(key, nonce, ivLow), Operations.ConcatBits(ivHigh, ivLow), 20U, oneTimeKey);

            m_aad = aad.ToArray();
            m_chaCha20 = ChaCha20.New(key, nonce, 1U);
            m_poly1305 = Poly1305.New(oneTimeKey.AsSpan(0, 32).ToArray());
        }

        /// <summary>
        /// Attempts to validate a message tag and then decrypts the message if validation was successful.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="work"></param>
        /// <param name="tag"></param>
        public void Decrypt(MemoryStream source, Stream destination, Stream work, ReadOnlySpan<byte> tag) {
            if (source.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(source));
            }

            if (destination.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(destination));
            }

            if (work.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(work));
            }

            ComputeTag(source, work, m_aad);
            source.Position = 0L;
            work.Position = 0L;

            if (Operations.CompareInConstantTime(m_poly1305.ComputeHash(work), tag)) {
                m_chaCha20.Transform(source, destination);
            }
            else {
                throw new CryptographicException(message: MESSAGE_AUTHENTICATION_ERROR);
            }
        }
        /// <summary>
        /// Encrypts a message and then generates a message authentication tag from the cipher.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="work"></param>
        public byte[] Encrypt(Stream source, Stream destination, Stream work) {
            if (source.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(source));
            }

            if (destination.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(destination));
            }

            if (work.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(work));
            }

            m_chaCha20.Transform(source, destination);
            destination.Position = 0L;

            return ComputeTag(destination, work, m_aad);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AeadChaCha20Poly1305"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="aad">The arbitrary length additional authenticated data parameter.</param>
        public static AeadChaCha20Poly1305 New(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> aad) => new AeadChaCha20Poly1305(key, nonce, aad);

        private byte[] ComputeTag(Stream source, Stream destination, byte[] aad) {
            if (source == destination) {
                throw new ArgumentException(message: "source and destination must be different streams", paramName: nameof(source));
            }

            var aadLength = aad.Length;
            var aadPadding = (TAG_LENGTH_IN_BYTES - (aadLength & (TAG_LENGTH_IN_BYTES - 1)));
            var cipherTextLength = source.Length;
            var cipherTextPadding = ((int)(TAG_LENGTH_IN_BYTES - (cipherTextLength % TAG_LENGTH_IN_BYTES)));
            var writeBuffer = new byte[TAG_LENGTH_IN_BYTES];

            destination.Position = 0L;
            destination.Write(aad, 0, aadLength);
            destination.Write(writeBuffer, 0, aadPadding);
            source.CopyTo(destination);
            destination.Write(writeBuffer, 0, cipherTextPadding);
            BinaryPrimitives.WriteUInt64LittleEndian(writeBuffer, checked((ulong)aadLength));
            destination.Write(writeBuffer, 0, sizeof(ulong));
            BinaryPrimitives.WriteUInt64LittleEndian(writeBuffer, checked((ulong)cipherTextLength));
            destination.Write(writeBuffer, 0, sizeof(ulong));
            destination.Position = 0L;

            return m_poly1305.ComputeHash(destination);
        }
    }
}
