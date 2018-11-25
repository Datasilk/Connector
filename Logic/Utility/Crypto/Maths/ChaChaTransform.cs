using System;
using System.Security.Cryptography;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents a cipher from the <a href="https://en.wikipedia.org/wiki/Salsa20#ChaCha_variant">ChaCha family of stream ciphers</a>.
    /// </summary>
    public sealed class ChaChaTransform : ICryptoTransform
    {
        private readonly uint m_numRounds;
        private readonly uint[] m_state;

        private ulong m_iv;

        /// <summary>
        /// Gets a value indicating whether multiple blocks can be transformed.
        /// </summary>
        public bool CanTransformMultipleBlocks => true;
        /// <summary>
        /// Gets a value indicating whether the current transform can be reused.
        /// </summary>
        public bool CanReuseTransform => true;
        /// <summary>
        /// Gets the input block size (in bytes).
        /// </summary>
        public int InputBlockSize => ChaCha.BlockLength;
        /// <summary>
        ///  Gets the output block size (in bytes).
        /// </summary>
        public int OutputBlockSize => ChaCha.BlockLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaChaTransform"/> class.
        /// </summary>
        /// <param name="state">The state used during initialization.</param>
        /// <param name="iv">The initial value parameter.</param>
        /// <param name="numRounds">The number of rounds to perform per block.</param>
        private ChaChaTransform(ReadOnlySpan<uint> state, ulong iv, uint numRounds) {
            m_iv = iv;
            m_numRounds = numRounds;
            m_state = state.ToArray();
        }

        /// <summary>
        /// Releases all resources used by this <see cref="ChaChaTransform"/> instance.
        /// </summary>
        public void Dispose() { }
        /// <summary>
        /// Transforms the specified region of the data array and copies the resulting transform to the specified region of the destination array.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataOffset"></param>
        /// <param name="dataCount"></param>
        /// <param name="destination"></param>
        /// <param name="destinationOffset"></param>
        public int TransformBlock(byte[] data, int dataOffset, int dataCount, byte[] destination, int destinationOffset) {
            var dataBuffer = data.AsSpan(dataOffset);
            var destinationBuffer = destination.AsSpan(destinationOffset);
            var keyStreamBuffer = (Span<byte>)stackalloc byte[ChaCha.BlockLength];
            var keyStreamPosition = m_iv;
            var numBytesTransformed = 0;

            while (ChaCha.BlockLength <= dataCount) {
                var destinationBufferSlice = destinationBuffer.Slice(numBytesTransformed, ChaCha.BlockLength);

                dataBuffer.Slice(numBytesTransformed, ChaCha.BlockLength).CopyTo(destinationBufferSlice);
                ChaCha.WriteKeyStreamBlock(m_state, checked(keyStreamPosition++), m_numRounds, keyStreamBuffer);
                Operations.Xor(keyStreamBuffer, destinationBufferSlice);

                dataCount -= ChaCha.BlockLength;
                numBytesTransformed += ChaCha.BlockLength;
            }

            if (0 < dataCount) {
                var destinationBufferSlice = destinationBuffer.Slice(numBytesTransformed, dataCount);

                dataBuffer.Slice(numBytesTransformed, dataCount).CopyTo(destinationBufferSlice);
                ChaCha.WriteKeyStreamBlock(m_state, checked(keyStreamPosition++), m_numRounds, keyStreamBuffer);
                Operations.Xor(keyStreamBuffer.Slice(0, dataCount), destinationBufferSlice);

                numBytesTransformed += dataCount;
            }

            m_iv = keyStreamPosition;

            return numBytesTransformed;
        }
        /// <summary>
        /// Transforms the specified region of the specified array.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataOffset"></param>
        /// <param name="dataCount"></param>
        public byte[] TransformFinalBlock(byte[] data, int dataOffset, int dataCount) {
            var result = new byte[dataCount];

            TransformBlock(data, dataOffset, dataCount, result, 0);

            return result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaChaTransform"/> class.
        /// </summary>
        /// <param name="state">The state used during initialization.</param>
        /// <param name="iv">The initial value parameter.</param>
        /// <param name="numRounds">The number of rounds to perform per block.</param>
        [CLSCompliant(false)]
        public static ChaChaTransform New(ReadOnlySpan<uint> state, ulong iv, uint numRounds) => new ChaChaTransform(state, iv, numRounds);
    }
}
