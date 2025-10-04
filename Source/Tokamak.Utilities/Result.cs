using System;
using System.Diagnostics.CodeAnalysis;

namespace Tokamak.Utilities
{
    /// <summary>
    /// A basic Result return type.
    /// </summary>
    /// <remarks>
    /// Might be worth while exploring something like FluentResults or ErrorOn in the future.
    /// </remarks>
    /// <typeparam name="TError">The type of error to use on failure.</typeparam>
    public class Result<TError>
    {
        protected Result(bool isSuccess, TError? error = default)
        {
            IsSuccess = isSuccess;

            if (!IsSuccess && error == null)
                throw new ArgumentNullException(nameof(error));

            Error = error;
        }

        public bool IsSuccess { get; }

        public TError? Error { get; }

        public static Result<TError> Success() => new(true);

        public static Result<TError> Failure(TError error) => new(false, error);

        public static implicit operator Result<TError>(TError error) => Failure(error);
    }

    /// <summary>
    /// A basic Result type with a return type.
    /// </summary>
    /// <typeparam name="TValue">The type of value to be returned on success.</typeparam>
    /// <typeparam name="TError">The type of value that indicates error details on failure.</typeparam>
    public class Result<TValue, TError> : Result<TError>
    {
        private TValue? m_value;

        private Result(TValue? value)
            : base(true)
        {
            m_value = value ?? throw new ArgumentNullException(nameof(value));
        }

        private Result(TError? error)
            : base(false, error)
        {
            m_value = default;
        }

        public TValue Value
        {
            get
            {
                if (!IsSuccess)
                    throw new Exception("Attempt to access value of failed result.");

                return m_value!;
            }
        }

        public TValue? ValueOrDefault
        {
            get => IsSuccess ? m_value : default;
        }

        public static Result<TValue, TError> Success(TValue value) => new(value);


        public static implicit operator Result<TValue, TError>(TValue value) => Success(value);

        public static implicit operator Result<TValue, TError>(TError error) => new(error);
    }
}
