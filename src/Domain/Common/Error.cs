using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public record Error(string Code, string Message, Dictionary<string, string[]>? Details = null)
    {
        public static Error NotFound(string entity, object key) =>
            new("NOT_FOUND", $"{entity} with identifier '{key}' was not found");

        public static Error Validation(string message, Dictionary<string, string[]>? details = null) =>
            new("VALIDATION_ERROR", message, details);

        public static Error BusinessRule(string message) =>
            new("BUSINESS_RULE_VIOLATION", message);

        public static Error Unauthorized(string message = "Unauthorized access") =>
            new("UNAUTHORIZED", message);

        public static Error Forbidden(string message = "Access forbidden") =>
            new("FORBIDDEN", message);

        public static Error Conflict(string message) =>
            new("CONFLICT", message);

        public static Error Internal(string message = "An internal error occurred") =>
            new("INTERNAL_ERROR", message);
    }
}
