using API.Markdown.V1;
using Application.DTO;
using Application.Services;
using Asp.Versioning;
using Infrastructure.ExternalService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Web.Api.Controllers;

namespace API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Authorize]
    [SwaggerTag("Controller de exemplo - V1")]
    public class ExampleController : BaseController
    {
        private readonly IExampleAppService _exampleAppService;
        public ExampleController(IExampleAppService exampleAppService, ILogger<ExampleController> logger)
            : base(logger)
        {
            _exampleAppService = exampleAppService;
        }

        [HttpGet()]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = ExampleControllerMarkdown.GetAll.Summary, Description = ExampleControllerMarkdown.GetAll.Description)]
        [SwaggerResponse(200, GlobalControllerMarkdown.Description.StatusCode200, Type = typeof(IEnumerable<ExampleAppServiceDto>))]
        [SwaggerResponse(204, GlobalControllerMarkdown.Description.StatusCode204)]
        [SwaggerResponse(400, GlobalControllerMarkdown.Description.StatusCode400)]
        [SwaggerResponse(401, GlobalControllerMarkdown.Description.StatusCode401)]
        [SwaggerResponse(500, GlobalControllerMarkdown.Description.StatusCode500)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            return HandleResult(await _exampleAppService.GetAll(cancellationToken));
        }

        [HttpGet("{zipCode}")]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = ExampleControllerMarkdown.GetByZipCode.Summary, Description = ExampleControllerMarkdown.GetByZipCode.Description)]
        [SwaggerResponse(200, GlobalControllerMarkdown.Description.StatusCode200, Type = typeof(ExampleAppServiceDto))]
        [SwaggerResponse(204, GlobalControllerMarkdown.Description.StatusCode204)]
        [SwaggerResponse(400, GlobalControllerMarkdown.Description.StatusCode400)]
        [SwaggerResponse(401, GlobalControllerMarkdown.Description.StatusCode401)]
        [SwaggerResponse(500, GlobalControllerMarkdown.Description.StatusCode500)]
        public async Task<IActionResult> GetById(string zipCode, CancellationToken cancellationToken)
        {
            return HandleResult(await _exampleAppService.GetByZipCode(zipCode, cancellationToken));
        }

        [HttpPost()]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = ExampleControllerMarkdown.Post.Summary, Description = ExampleControllerMarkdown.Post.Description)]
        [SwaggerResponse(200, GlobalControllerMarkdown.Description.StatusCode201)]
        [SwaggerResponse(400, GlobalControllerMarkdown.Description.StatusCode400)]
        [SwaggerResponse(401, GlobalControllerMarkdown.Description.StatusCode401)]
        [SwaggerResponse(500, GlobalControllerMarkdown.Description.StatusCode500)]
        public async Task<IActionResult> Post([FromBody] ExampleAppServiceDto ExampleAppServiceDto, CancellationToken cancellationToken)
        {
            return Created();
        }
    }
}
