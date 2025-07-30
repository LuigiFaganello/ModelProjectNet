using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Exceptions
{
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
