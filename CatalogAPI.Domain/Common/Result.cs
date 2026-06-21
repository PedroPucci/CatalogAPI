using FluentValidation.Results;

namespace CatalogAPI.Domain.Common
{
    public class Result<T>
    {
        public Result(bool success, string? message, T? data)
        {
            Success = success;
            Message = message ?? string.Empty;
            Data = data;
            Errors = Enumerable.Empty<ValidationFailure>();
        }

        public Result(bool success, IEnumerable<ValidationFailure>? errors)
        {
            Success = success;
            Errors = errors ?? Enumerable.Empty<ValidationFailure>();
            Message = string.Empty;
            Data = default;
        }

        public Result()
        {
            Message = string.Empty;
            Errors = Enumerable.Empty<ValidationFailure>();
            Data = default;
        }

        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? AccessToken { get; set; }
        public string Message { get; set; }
        public IEnumerable<ValidationFailure> Errors { get; set; }

        public static Result<T> Ok(T data) =>
            new() { Success = true, Data = data, Message = string.Empty };

        public static Result<T> Ok(string? responseMessage = null, T? responseData = default)
        {
            return new Result<T>(true, responseMessage, responseData);
        }

        public static Result<T> Error(string responseMessage)
        {
            return new Result<T>(false, responseMessage, default);
        }
    }
}