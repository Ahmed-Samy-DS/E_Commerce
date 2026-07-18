using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }

        public IReadOnlyList<Error> Errors { get; }

        protected Result(bool isSuccess, IReadOnlyList<Error> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result OK() => new(true, Array.Empty<Error>());
        public static Result Fail(Error error) => new(false, new[]{error});
        public static Result Fail(IReadOnlyList<Error> errors) => new(false, errors);

    }

    public class Result<TValue> : Result
    {
        private readonly TValue _value;
        public TValue Value { get; set; }

        public TValue data => IsSuccess ? _value : throw new InvalidOperationException("Can Not Access The Value Of Failed Result");
        private Result(TValue value):base(true, Array.Empty<Error>())
        {
            _value = value;
        }

        private Result (Error error): base(false, new[]{error})
        {
            _value = default!;
        }
        private Result (IReadOnlyList<Error> error) : base(false, error)
        {
            _value = default!;
        }

        public static Result<TValue> OK(TValue value) => new Result<TValue>(value);
        public static Result<TValue> Fail(Error error) => new Result<TValue>(error);
        public static Result<TValue> Fail(IReadOnlyList<Error> error) => new Result<TValue>(error);

        public static implicit operator Result<TValue>(TValue value) => OK(value);
        public static implicit operator Result<TValue>(Error error) => Fail(error);

    }
}
