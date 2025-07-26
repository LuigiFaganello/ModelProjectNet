using API.Markdown.V1;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Authorize]
    [SwaggerTag("Controller de exemplo - V1")]
    public class ExampleController : Controller
    {
        private readonly IExampleAppService _exampleAppService;
        public ExampleController(IExampleAppService exampleAppService)
        {
            _exampleAppService = exampleAppService;
        }

        [HttpGet()]
        [SwaggerOperation(Summary = ExampleControllerMarkdown.GetAll.Summary, Description = ExampleControllerMarkdown.GetAll.Description)]
        [SwaggerResponse(200, GlobalControllerMarkdown.Description.StatusCode200, Type = typeof(string))]
        [SwaggerResponse(204, GlobalControllerMarkdown.Description.StatusCode204)]
        [SwaggerResponse(400, GlobalControllerMarkdown.Description.StatusCode400)]
        [SwaggerResponse(401, GlobalControllerMarkdown.Description.StatusCode401)]
        [SwaggerResponse(500, GlobalControllerMarkdown.Description.StatusCode500)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _exampleAppService.GetAll(cancellationToken));
        }

        [HttpGet("{zipCode}")]
        [SwaggerOperation(Summary = ExampleControllerMarkdown.GetById.Summary, Description = ExampleControllerMarkdown.GetById.Description)]
        [SwaggerResponse(200, GlobalControllerMarkdown.Description.StatusCode200, Type = typeof(string))]
        [SwaggerResponse(204, GlobalControllerMarkdown.Description.StatusCode204)]
        [SwaggerResponse(400, GlobalControllerMarkdown.Description.StatusCode400)]
        [SwaggerResponse(401, GlobalControllerMarkdown.Description.StatusCode401)]
        [SwaggerResponse(500, GlobalControllerMarkdown.Description.StatusCode500)]
        public async Task<IActionResult> GetById(string zipCode, CancellationToken cancellationToken)
        {
            return Ok(await _exampleAppService.GetByZipCode(zipCode, cancellationToken));
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = ExampleControllerMarkdown.Put.Summary, Description = ExampleControllerMarkdown.Put.Description)]
        [SwaggerResponse(200, GlobalControllerMarkdown.Description.StatusCode200, Type = typeof(string))]
        [SwaggerResponse(400, GlobalControllerMarkdown.Description.StatusCode400)]
        [SwaggerResponse(401, GlobalControllerMarkdown.Description.StatusCode401)]
        [SwaggerResponse(500, GlobalControllerMarkdown.Description.StatusCode500)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken)
        {
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = ExampleControllerMarkdown.Delete.Summary, Description = ExampleControllerMarkdown.Delete.Description)]
        [SwaggerResponse(200, GlobalControllerMarkdown.Description.StatusCode200, Type = typeof(string))]
        [SwaggerResponse(400, GlobalControllerMarkdown.Description.StatusCode400)]
        [SwaggerResponse(401, GlobalControllerMarkdown.Description.StatusCode401)]
        [SwaggerResponse(500, GlobalControllerMarkdown.Description.StatusCode500)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            return Ok();
        }
    }
}
