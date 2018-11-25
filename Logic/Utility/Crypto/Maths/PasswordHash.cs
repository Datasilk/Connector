using Crypto.Extensions;
using System;
using System.Security.Cryptography;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents an instance of a secure key derivation function.
    /// </summary>
    public sealed class PasswordHash : IKeyDerivationFunction
    {
        private readonly DeriveBytes m_keyDerivationFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordHash"/> class.
        /// </summary>
        /// <param name="keyDerivationFunction">The class that will be used to perform key derivation.</param>
        private PasswordHash(DeriveBytes keyDerivationFunction) {
            if (keyDerivationFunction.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(keyDerivationFunction));
            }

            m_keyDerivationFunction = keyDerivationFunction;
        }

        /// <summary>
        /// Derives a cryptographic key value.
        /// </summary>
        /// <param name="keyLength">The length of the derived key value (in bytes).</param>
        public byte[] Generate(int keyLength) => m_keyDerivationFunction.GetBytes(keyLength);
        /// <summary>
        /// Verifies a password by comparing it against the derived cryptographic key value; returns false if the values are not equal.
        /// </summary>
        /// <param name="keyLength">The length of the derived key value (in bytes).</param>
        /// <param name="hash">The password hash that will be verified.</param>
        public bool Validate(int keyLength, ReadOnlySpan<byte> hash) => Operations.CompareInConstantTime(Generate(keyLength), hash);

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordHash"/> class.
        /// </summary>
        /// <param name="keyDerivationFunction">The class that will be used to perform key derivation.</param>
        public static PasswordHash New(DeriveBytes keyDerivationFunction) => new PasswordHash(keyDerivationFunction);
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordHash"/> class using <see cref="Pbkdf2"/> as the key derivation function and <see cref="Blake2b"/> as the hash function.
        /// </summary>
        /// <param name="password">The secret that a new key will be derived from.</param>
        /// <param name="salt">The salt that will be combined with the passord during key derivation.</param>
        /// <param name="iterationCount">The number of iterations that will be performed to derive a new key.</param>
        public static PasswordHash New(ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, int iterationCount) => New(Pbkdf2.New(Blake2b.New(password), salt, iterationCount));
    }
}
