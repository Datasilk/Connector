using Crypto.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Crypto.Maths
{
    /// <summary>
    /// Represents a strongly typed collection that uses a random number generator to retrieve elements; relies on the <a href="https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle">Fisher–Yates shuffle algorithm</a>.
    /// </summary>
    /// <typeparam name="T">The type of elements encapsulated by the bag.</typeparam>
    public sealed class ShuffleBag<T> : IEnumerable<T>, IEnumerator<T>
    {
        private readonly int m_count;
        private readonly IList<T> m_list;
        private readonly int m_offset;
        private readonly IRandomNumberGenerator m_randomNumberGenerator;

        private int m_position;

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        object IEnumerator.Current => Current;
        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public T Current => m_list[m_position];

        /// <summary>
        /// Initializes a new instance of the <see cref="ShuffleBag{T}"/> class.
        /// </summary>
        /// <param name="randomNumberGenerator">The source of random numbers that will be used to perform the shuffle.</param>
        /// <param name="list">The list of values that will be shuffled.</param>
        /// <param name="offset">The index where shuffling will begin at.</param>
        /// <param name="count">The number of elements that will be shuffled.</param>
        private ShuffleBag(IRandomNumberGenerator randomNumberGenerator, IList<T> list, int offset, int count) {
            if (list.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(list));
            }

            if (randomNumberGenerator.IsNull()) {
                throw new ArgumentNullException(paramName: nameof(randomNumberGenerator));
            }

            m_count = count;
            m_list = list;
            m_offset = offset;
            m_position = (offset + count);
            m_randomNumberGenerator = randomNumberGenerator;
        }

        /// <summary>
        /// Releases all resources used by this <see cref="ShuffleBag"/> instance.
        /// </summary>
        public void Dispose() { }
        /// <summary>
        /// Returns an enumerator that randomly yields elements from the bag.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Returns an enumerator that randomly yields elements from the bag.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => this;
        /// <summary>
        /// Advances the enumerator to the next random element in the collection.
        /// </summary>
        public bool MoveNext() {
            if (m_offset < m_position) {
                var leftIndex = m_offset;
                var rightIndex = --m_position;

                if (leftIndex != rightIndex) {
                    var randomIndex = m_randomNumberGenerator.NextInt32(leftIndex, rightIndex);
                    var tempValue = m_list[randomIndex];

                    m_list[randomIndex] = m_list[rightIndex];
                    m_list[rightIndex] = tempValue;
                }

                return true;
            }
            else {
                return false;
            }
        }
        /// <summary>
        /// Sets the enumerator to its initial position.
        /// </summary>
        /// <param name="unshuffle">Indicates whether elements will be returned to their original, unshuffled, positions.</param>
        public void Reset(bool unshuffle) {
            if (unshuffle) {
                var lastIndex = (m_offset + m_count);
                var leftIndex = m_offset;
                var rightIndex = ++m_position;

                while (rightIndex < lastIndex) {
                    m_randomNumberGenerator.Jump(-1);

                    var randomIndex = m_randomNumberGenerator.NextInt32(leftIndex, rightIndex);
                    var tempValue = m_list[randomIndex];

                    m_list[randomIndex] = m_list[rightIndex];
                    m_list[rightIndex++] = tempValue;

                    m_randomNumberGenerator.Jump(-1);
                }
            }

            m_position = (m_offset + m_count);
        }
        /// <summary>
        /// Sets the enumerator to its initial position and returns elements to their original, unshuffled, positions.
        /// </summary>
        public void Reset() => Reset(unshuffle: true);


        /// <summary>
        /// Initializes a new instance of the <see cref="ShuffleBag{T}"/> class.
        /// </summary>
        /// <param name="randomNumberGenerator">The source of random numbers that will be used to perform the shuffle.</param>
        /// <param name="list">The list of values that will be shuffled.</param>
        /// <param name="offset">The index where shuffling will begin at.</param>
        /// <param name="count">The number of elements that will be shuffled.</param>
        public static ShuffleBag<T> New(IRandomNumberGenerator randomNumberGenerator, IList<T> list, int offset, int count) => new ShuffleBag<T>(randomNumberGenerator, list, offset, count);
    }

    /// <summary>
    /// A collection of methods that directly or indirectly augment the <see cref="ShuffleBag{T}"/> class.
    /// </summary>
    public static class ShuffleBag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShuffleBag{T}"/> class.
        /// </summary>
        /// <typeparam name="T">The type of elements encapsulated by the bag.</typeparam>
        /// <param name="randomNumberGenerator">The source of random numbers that will be used to perform the shuffle.</param>
        /// <param name="list">The list of values that will be shuffled.</param>
        /// <param name="offset">The index where shuffling will begin at.</param>
        /// <param name="count">The number of elements that will be shuffled.</param>
        public static ShuffleBag<T> New<T>(IRandomNumberGenerator randomNumberGenerator, IList<T> list, int offset, int count) => ShuffleBag<T>.New(randomNumberGenerator, list, offset, count);
        /// <summary>
        /// Initializes a new instance of the <see cref="ShuffleBag{T}"/> class using the default <see cref="FastRandom"/> instance for random number generation.
        /// </summary>
        /// <typeparam name="T">The type of elements encapsulated by the bag.</typeparam>
        /// <param name="list">The list of values that will be shuffled.</param>
        /// <param name="offset">The index where shuffling will begin at.</param>
        /// <param name="count">The number of elements that will be shuffled.</param>
        public static ShuffleBag<T> New<T>(IList<T> list, int offset, int count) => New(FastRandom.Default, list, offset, count);
        /// <summary>
        /// Initializes a new instance of the <see cref="ShuffleBag{T}"/> class using the default <see cref="FastRandom"/> instance for random number generation.
        /// </summary>
        /// <typeparam name="T">The type of elements encapsulated by the bag.</typeparam>
        /// <param name="list">The list of values that will be randomized.</param>
        public static ShuffleBag<T> New<T>(IList<T> list) => New(list, 0, list.Count);
    }
}
