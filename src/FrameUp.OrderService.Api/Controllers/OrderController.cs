using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrameUp.OrderService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController(ILogger<OrderController> logger, ICreateProcessingOrder createProcessingOrder) : ControllerBase
    {
        private readonly ILogger<OrderController> _logger = logger;

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<CreateProcessingOrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult<CreateProcessingOrderResponse>> Post([FromForm] ProcessVideoBodyRequest request)
        {
            var processVideoRequest = new CreateProcessingOrderRequest()
            {
                Videos = request.Videos.Select(video => new VideoRequest
                {
                    ContentStream = video.OpenReadStream(),
                    Metadata = new VideoMetadataRequest
                    {
                        Name = video.FileName,
                        Size = video.Length,
                        ContentType = video.ContentType
                    }
                })
            };

            var response = await createProcessingOrder.Execute(processVideoRequest);
            
            return Ok(response);
        }
    }
    
    public class ProcessVideoBodyRequest
    {
        public IEnumerable<IFormFile> Videos { get; set; } = [];
    }
}
