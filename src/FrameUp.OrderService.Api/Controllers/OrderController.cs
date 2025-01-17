using FrameUp.OrderService.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrameUp.OrderService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController(ILogger<OrderController> logger) : ControllerBase
    {
        private readonly ILogger<OrderController> _logger = logger;

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<CreateProcessingOrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CreateProcessingOrderResponse> Post(ProcessVideoBodyRequest request)
        {
            var stream = request.Video.OpenReadStream();

            var processVideoRequest = new CreateProcessingOrderRequest()
            {
                Videos = new List<VideoRequest>
                {
                    new VideoRequest
                    {
                        ContentStream = stream,
                        Metadata = new VideoMetadataRequest
                        {
                            Name = request.Video.FileName,
                            Size = request.Video.Length,
                            ContentType = request.Video.ContentType
                        }
                    }
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
