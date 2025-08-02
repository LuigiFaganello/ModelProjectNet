using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Application.Common;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Middleware
{
    [ExcludeFromCodeCoverage]
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            LogException(exception, context);

            context.Response.ContentType = "application/json";

            var (statusCode, errorResponse) = CreateCleanErrorResponse(context, exception);
            context.Response.StatusCode = statusCode;

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            });

            await context.Response.WriteAsync(jsonResponse);
        }

        private void LogException(Exception exception, HttpContext context)
        {
            var logLevel = GetLogLevel(exception);
            var message = "An exception occurred during request processing";

            var logData = new
            {
                RequestPath = context.Request.Path,
                RequestMethod = context.Request.Method,
                TraceId = context.TraceIdentifier,
                UserId = context.User?.Identity?.Name,
                ExceptionType = exception.GetType().Name,
                ExceptionMessage = exception.Message
            };

            _logger.Log(logLevel, exception, message + ". {@LogData}", logData);
        }

        private static LogLevel GetLogLevel(Exception exception)
        {
            return exception switch
            {
                DomainException => LogLevel.Warning,
                ValidationException => LogLevel.Warning,
                Application.Common.ApplicationException => LogLevel.Warning,
                UnauthorizedAccessException => LogLevel.Warning,
                ArgumentException => LogLevel.Warning,
                InvalidOperationException => LogLevel.Warning,
                DbUpdateConcurrencyException => LogLevel.Warning,
                TimeoutException => LogLevel.Warning,
                TaskCanceledException => LogLevel.Information,
                OperationCanceledException => LogLevel.Information,
                _ => LogLevel.Error
            };
        }
        private (int StatusCode, object ErrorResponse) CreateCleanErrorResponse(HttpContext context, Exception exception)
        {
            var (statusCode, errorCode, message, validationErrors) = GetErrorDetails(exception);

            var baseResponse = new
            {
                code = errorCode,
                message = message,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.Value,
                method = context.Request.Method,
                traceId = context.TraceIdentifier
            };

            if (validationErrors != null && validationErrors.Any())
            {
                var responseWithDetails = new
                {
                    baseResponse.code,
                    baseResponse.message,
                    baseResponse.timestamp,
                    baseResponse.path,
                    baseResponse.method,
                    baseResponse.traceId,
                    details = validationErrors
                };
                return (statusCode, responseWithDetails);
            }

            return (statusCode, baseResponse);
        }

        private (int StatusCode, string ErrorCode, string Message, Dictionary<string, string[]>? ValidationErrors)
            GetErrorDetails(Exception exception)
        {
            return exception switch
            {
                // Domain Exceptions
                EntityNotFoundException ex => (404, ex.ErrorCode, ex.Message, null),
                BusinessRuleViolationException ex => (422, ex.ErrorCode, ex.Message, null),

                // Application Exceptions
                ValidationException ex => (400, ex.ErrorCode, ex.Message, ex.Errors),
                Application.Common.ApplicationException ex => (500, ex.ErrorCode, ex.Message, null),

                // Framework Exceptions (ordem importante: mais específico primeiro)
                UnauthorizedAccessException => (401, "UNAUTHORIZED", "Access denied", null),
                ArgumentNullException ex => (400, "MISSING_PARAMETER", $"Required parameter '{ex.ParamName}' is missing", null),
                ArgumentException ex => (400, "INVALID_ARGUMENT", ex.Message, null),
                InvalidOperationException ex => (409, "INVALID_OPERATION", ex.Message, null),

                // Database Exceptions
                DbUpdateConcurrencyException => (409, "CONCURRENCY_CONFLICT", "The record was modified by another user. Please refresh and try again.", null),
                DbUpdateException ex when IsUniqueConstraintViolation(ex) => (409, "DUPLICATE_RECORD", "A record with this information already exists", null),
                DbUpdateException ex when IsForeignKeyViolation(ex) => (409, "FOREIGN_KEY_CONSTRAINT", "Cannot delete this record because it is referenced by other records", null),
                DbUpdateException => (500, "DATABASE_ERROR", "A database error occurred", null),

                // Timeout Exceptions
                TimeoutException => (408, "TIMEOUT", "The operation timed out", null),
                TaskCanceledException => (408, "REQUEST_CANCELLED", "The request was cancelled", null),
                OperationCanceledException => (408, "OPERATION_CANCELLED", "The operation was cancelled", null),

                // Default case
                _ => (500, "INTERNAL_ERROR", GetSafeErrorMessage(exception), null)
            };
        }

        private string GetSafeErrorMessage(Exception exception)
        {
            // Em produção, não expor detalhes internos
            if (_environment.IsProduction())
                return "An internal server error occurred";

            // Em desenvolvimento, mostrar detalhes
            return exception.Message;
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            var message = ex.InnerException?.Message?.ToLowerInvariant() ?? string.Empty;
            return message.Contains("unique") ||
                   message.Contains("duplicate") ||
                   message.Contains("cannot insert duplicate");
        }

        private static bool IsForeignKeyViolation(DbUpdateException ex)
        {
            var message = ex.InnerException?.Message?.ToLowerInvariant() ?? string.Empty;
            return message.Contains("foreign key") ||
                   message.Contains("reference constraint") ||
                   message.Contains("delete statement conflicted");
        }
    }
}
