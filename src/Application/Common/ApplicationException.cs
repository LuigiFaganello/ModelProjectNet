using System.Diagnostics.CodeAnalysis;

namespace Application.Common
{
    [ExcludeFromCodeCoverage]
    public class ApplicationException : Exception
    {
        public string ErrorCode { get; }

        public ApplicationException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }

    public class ValidationException : ApplicationException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("VALIDATION_ERROR", "One or more validation errors occurred")
        {
            Errors = errors;
        }
    }
}
