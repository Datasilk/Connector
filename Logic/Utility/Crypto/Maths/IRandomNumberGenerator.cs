namespace Crypto.Maths
{
    /// <summary>
    /// Represents a <a href="https://en.wikipedia.org/wiki/Pseudorandom_number_generator">pseudo-random number generator</a>.
    /// </summary>
    public interface IRandomNumberGenerator
    {
        /// <summary>
        /// Moves the state of the generator forwards or backwards by the specificed number of steps.
        /// </summary>
        /// <param name="count">The number of states that will be jumped.</param>
        void Jump(long count);
        /// <summary>
        /// Generates a uniformly distributed double precision floating point value between the exclusive range (0, 1).
        /// </summary>
        double NextDouble();
        /// <summary>
        /// Generates a uniformly distributed single precision floating point value between the exclusive range (0, 1).
        /// </summary>
        float NextSingle();
        /// <summary>
        /// Generates a uniformly distributed 32-bit signed integer between the range of int.MinValue and int.MaxValue.
        /// </summary>
        int NextInt32();
        /// <summary>
        /// Generates a uniformly distributed 32-bit signed integer between the inclusive range [x, y].
        /// </summary>
        /// <param name="x">The value of x.</param>
        /// <param name="y">The value of y.</param>
        int NextInt32(int x, int y);
    }
}
