using FrameUp.OrderService.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrameUp.OrderService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessVideoController(ILogger<ProcessVideoController> logger) : ControllerBase
    {
        private readonly ILogger<ProcessVideoController> _logger = logger;

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<CreateProcessingOrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CreateProcessingOrderResponse> Post(ProcessVideoBodyRequest request)
        {
            var stream = request.Video.OpenReadStream();

            var processVideoRequest = new CreateProcessingOrderRequest()
            {
                Video = stream,
                VideoMetadata = new VideoMetadataRequest
                {
                    ContentType = request.Video.ContentType,
                    Size = request.Video.Length
                }
            };
            
            return Ok();
        }
    }
    
    public class ProcessVideoBodyRequest
    {
        public IFormFile Video { get; set; }
    }
}
