using System;

namespace ServicePerfectCV.Application.Common
{
    /// <summary>
    /// Represents an optional value that may or may not be present.
    /// Used to distinguish between "not provided" (no value) and "explicitly set to null" (has value, but value is null).
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    public class Optional<T>
    {
        /// <summary>
        /// Gets the value of the optional if it has one.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Gets a value indicating whether the optional has a value.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// Creates a new optional with no value.
        /// </summary>
        private Optional()
        {
            HasValue = false;
            Value = default;
        }

        /// <summary>
        /// Creates a new optional with the specified value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        public Optional(T value)
        {
            Value = value;
            HasValue = true;
        }

        /// <summary>
        /// Creates an optional with no value.
        /// </summary>
        /// <returns>An optional with no value.</returns>
        public static Optional<T> None() => new Optional<T>();

        /// <summary>
        /// Creates an optional from the specified value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <returns>An optional containing the value.</returns>
        public static Optional<T> From(T value) => new Optional<T>(value);

        /// <summary>
        /// Implicitly converts a value to an optional containing that value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator Optional<T>(T value) => new Optional<T>(value);

        /// <summary>
        /// Gets the string representation of this optional.
        /// </summary>
        /// <returns>A string representation of this optional.</returns>
        public override string ToString()
        {
            return HasValue ? $"Some({Value})" : "None";
        }
    }
}