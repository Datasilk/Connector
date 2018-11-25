using Crypto.Extensions;
using System;
using System.Security.Cryptography;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents an instance of the <a href="https://en.wikipedia.org/wiki/HKDF">Hkdf key derivation function</a>.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc5869
    /// </remarks>
    public class Hkdf : DeriveBytes
    {
        private const string KEY_LENGTH_ERROR = ("key length must be a positive integer less than {0}");

        private readonly byte[] m_data;
        private readonly KeyedHashAlgorithm m_keyedHashAlgorithm;
        private readonly int m_maxKeyLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hkdf"/> class.
        /// </summary>
        /// <param name="keyedHashAlgorithm">The class that will be used to perform key derivation.</param>
        /// <param name="data">The additional input data.</param>
        private Hkdf(KeyedHashAlgorithm keyedHashAlgorithm, byte[] data) {
            if (keyedHashAlgorithm.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(keyedHashAlgorithm));
            }

            m_data = (data ?? CompatibilityUtils.Array.Empty<byte>());
            m_keyedHashAlgorithm = keyedHashAlgorithm;
            m_maxKeyLength = (byte.MaxValue * (keyedHashAlgorithm.HashSize >> 3));
        }

        /// <summary>
        /// Derives a cryptographic key value using the Hkdf algorithm.
        /// </summary>
        /// <param name="keyLength">The length of the derived key value (in bytes).</param>
        public override byte[] GetBytes(int keyLength) {
            var maxKeyLength = m_maxKeyLength;

            if ((1 > keyLength) || (maxKeyLength < keyLength)) {
                throw new ArgumentOutOfRangeException(actualValue: keyLength, message: string.Format(KEY_LENGTH_ERROR, (maxKeyLength + 1)), paramName: nameof(keyLength));
            }

            var data = m_data;
            var hashAlgorithm = m_keyedHashAlgorithm;
            var keyHashSizeInBytes = (hashAlgorithm.HashSize >> 3);
            var buffer = new byte[((keyHashSizeInBytes + data.Length) + 1)];
            var result = new byte[keyLength];
            var remainder = (keyLength % keyHashSizeInBytes);
            var count = ((keyLength / keyHashSizeInBytes) + ((0 < remainder) ? 1 : 0));

            data.AsSpan().CopyTo(buffer.AsSpan(keyHashSizeInBytes));
            buffer[(buffer.Length - 1)] = 0x01;

            hashAlgorithm.ComputeHash(buffer.AsSpan(keyHashSizeInBytes).ToArray()).CopyTo(buffer.AsSpan());

            for (var i = 2; i <= count; i++) {
                buffer.AsSpan(0, keyHashSizeInBytes).CopyTo(result.AsSpan(((i - 2) * keyHashSizeInBytes), keyHashSizeInBytes));
                buffer[(buffer.Length - 1)] = ((byte)(0x01 * i));
                hashAlgorithm.ComputeHash(buffer).AsSpan().CopyTo(buffer);
            }

            buffer.AsSpan(0, remainder).CopyTo(result.AsSpan((count - 1) * keyHashSizeInBytes));

            return result;
        }
        /// <summary>
        /// Resets the internal state.
        /// </summary>
        public override void Reset() => m_keyedHashAlgorithm.Initialize();

        /// <summary>
        /// Derives a fixed-length pseudo-random key using the specified hash algorithm.
        /// </summary>
        /// <param name="keyedHashAlgorithm">The class that will be used to perform key derivation.</param>
        /// <param name="key">The key that a new value will be derived from.</param>
        public static byte[] Extract(KeyedHashAlgorithm keyedHashAlgorithm, byte[] key) => keyedHashAlgorithm.ComputeHash(key);
        /// <summary>
        /// Initializes a new instance of the <see cref="Hkdf"/> class.
        /// </summary>
        /// <param name="keyedHashAlgorithm">The class that will be used to perform key derivation.</param>
        /// <param name="data">The additional input data.</param>
        public static Hkdf New(KeyedHashAlgorithm keyedHashAlgorithm, byte[] data) => new Hkdf(keyedHashAlgorithm, data);
    }
}
