using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger<BaseController> _logger;

        protected BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            return result.Match(
                onSuccess: data => Ok(data),
                onFailure: error => HandleError(error)
            );
        }

        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return NoContent();

            return HandleError(result.Error!);
        }

        protected IActionResult HandleNoContentOrNotFound(Result result)
        {
            if (result.IsSuccess)
                return NoContent();

            return HandleError(result.Error!);
        }

        protected IActionResult HandleNoContentOrNotFound<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return NoContent();

            return HandleError(result.Error!);
        }
        protected IActionResult HandleError(Error error)
        {
            var errorResponse = CreateStandardErrorResponse(error);

            return error.Code switch
            {
                "NOT_FOUND" => NotFound(errorResponse),
                "VALIDATION_ERROR" => BadRequest(errorResponse),
                "BUSINESS_RULE_VIOLATION" => UnprocessableEntity(errorResponse),
                "UNAUTHORIZED" => Unauthorized(errorResponse),
                "FORBIDDEN" => Forbid(),
                "CONFLICT" => Conflict(errorResponse),
                "TIMEOUT" => StatusCode(408, errorResponse),
                "DATABASE_UNAVAILABLE" => StatusCode(503, errorResponse),
                _ => StatusCode(500, errorResponse)
            };
        }
        private object CreateStandardErrorResponse(Error error)
        {
            var baseResponse = new
            {
                code = error.Code,
                message = error.Message,
                timestamp = DateTime.UtcNow,
                path = HttpContext.Request.Path.Value,
                method = HttpContext.Request.Method,
                traceId = HttpContext.TraceIdentifier
            };

            if (error.Details != null && error.Details.Any())
            {
                return new
                {
                    baseResponse.code,
                    baseResponse.message,
                    baseResponse.timestamp,
                    baseResponse.path,
                    baseResponse.method,
                    baseResponse.traceId,
                    details = error.Details
                };
            }

            return baseResponse;
        }
    }
}
