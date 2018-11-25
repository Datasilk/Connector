using Crypto.Extensions;
using System;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents an instance of the <a href="https://en.wikipedia.org/wiki/PBKDF2">Pbkdf2 key derivation function</a>.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc2898
    /// </remarks>
    public sealed class Pbkdf2 : DeriveBytes
    {
        private const string KEY_LENGTH_ERROR = ("key length must be a positive integer");

        private readonly uint m_iterationCount;
        private readonly KeyedHashAlgorithm m_keyedHashAlgorithm;
        private readonly byte[] m_salt;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pbkdf2"/> class.
        /// </summary>
        /// <param name="keyedHashAlgorithm">The class that will be used to perform key derivation.</param>
        /// <param name="salt">The salt that will be combined with the passord during key derivation.</param>
        /// <param name="iterationCount">The number of iterations that will be performed to derive a new key.</param>
        private Pbkdf2(KeyedHashAlgorithm keyedHashAlgorithm, ReadOnlySpan<byte> salt, uint iterationCount) {
            if (keyedHashAlgorithm.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(keyedHashAlgorithm));
            }

            m_iterationCount = iterationCount;
            m_keyedHashAlgorithm = keyedHashAlgorithm;
            m_salt = salt.ToArray();
        }

        /// <summary>
        /// Derives a cryptographic key value using the Pbkdf2 algorithm.
        /// </summary>
        /// <param name="keyLength">The length of the derived key value (in bytes).</param>
        public override byte[] GetBytes(int keyLength) {
            if (1 > keyLength) {
                throw new ArgumentOutOfRangeException(actualValue: keyLength, message: KEY_LENGTH_ERROR, paramName: nameof(keyLength));
            }

            var hashAlgorithm = m_keyedHashAlgorithm;
            var iterationCount = m_iterationCount;
            var saltValue = m_salt;

            var blockSizeInBytes = (hashAlgorithm.HashSize >> 3);
            var chunkCount = (((keyLength + blockSizeInBytes) - 1) / blockSizeInBytes);
            var chunkIndex = 0;
            var resultOffset = 0;
            var resultValue = new byte[keyLength];
            var saltLength = saltValue.Length;
            var tempValue = new byte[(saltLength + sizeof(int))];

            while (chunkIndex < chunkCount) {
                Buffer.BlockCopy(saltValue, 0, tempValue, 0, saltLength);
                BinaryPrimitives.WriteInt32BigEndian(tempValue.AsSpan(saltLength), ++chunkIndex);

                var hashIn = hashAlgorithm.ComputeHash(tempValue);
                var hashOut = hashIn;

                for (var iterationIndex = 1U; (iterationIndex < iterationCount); iterationIndex++) {
                    hashIn = hashAlgorithm.ComputeHash(hashIn);

                    Operations.Xor(hashIn, hashOut);
                }

                if (chunkIndex < chunkCount) {
                    Buffer.BlockCopy(hashOut, 0, resultValue, resultOffset, blockSizeInBytes);
                }
                else {
                    Buffer.BlockCopy(hashOut, 0, resultValue, resultOffset, (keyLength - resultOffset));
                }

                resultOffset += blockSizeInBytes;
            }

            return resultValue;
        }
        /// <summary>
        /// Resets the internal state.
        /// </summary>
        public override void Reset() => m_keyedHashAlgorithm.Initialize();

        /// <summary>
        /// Initializes a new instance of the <see cref="Pbkdf2"/> class.
        /// </summary>
        /// <param name="keyedHashAlgorithm">The class that will be used to perform key derivation.</param>
        /// <param name="salt">The salt that will be combined with the passord during key derivation.</param>
        /// <param name="iterationCount">The number of iterations that will be performed to derive a new key.</param>
        [CLSCompliant(false)]
        public static Pbkdf2 New(KeyedHashAlgorithm keyedHashAlgorithm, ReadOnlySpan<byte> salt, uint iterationCount) => new Pbkdf2(keyedHashAlgorithm, salt, iterationCount);
        /// <summary>
        /// Initializes a new instance of the <see cref="Pbkdf2"/> class.
        /// </summary>
        /// <param name="keyedHashAlgorithm">The class that will be used to perform key derivation.</param>
        /// <param name="salt">The salt that will be combined with the passord during key derivation.</param>
        /// <param name="iterationCount">The number of iterations that will be performed to derive a new key.</param>
        public static Pbkdf2 New(KeyedHashAlgorithm keyedHashAlgorithm, ReadOnlySpan<byte> salt, int iterationCount) => New(keyedHashAlgorithm, salt, checked((uint)iterationCount));
    }
}
