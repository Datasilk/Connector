using System;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents a <a href="https://en.wikipedia.org/wiki/Key_derivation_function">key derivation function</a>.
    /// </summary>
    public interface IKeyDerivationFunction
    {

        /// <summary>
        /// Derives a cryptographic key value.
        /// </summary>
        /// <param name="keyLength">The length of the derived key value (in bytes).</param>
        byte[] Generate(int keyLength);
        /// <summary>
        /// Verifies a password by comparing it against the derived cryptographic key value; returns false if the values are not equal.
        /// </summary>
        /// <param name="keyLength">The length of the derived key value (in bytes).</param>
        /// <param name="hash">The password hash that will be verified.</param>
        bool Validate(int keyLength, ReadOnlySpan<byte> hash);
    }
}
