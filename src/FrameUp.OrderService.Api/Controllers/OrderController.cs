using FrameUp.OrderService.Api.Models;
using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrameUp.OrderService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController(
        ICreateProcessingOrder createProcessingOrder,
        IGetProcessingOrder getProcessingOrder) : ControllerBase
    {

        [HttpPost]
        [ProducesResponseType(typeof(CreateProcessingOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult<CreateProcessingOrderResponse>> Post([FromForm] ProcessVideoBodyRequest request)
        {
            var processVideoRequest = CreateProcessingOrderRequest(request);

            var response = await createProcessingOrder.Execute(processVideoRequest);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        private static CreateProcessingOrderRequest CreateProcessingOrderRequest(ProcessVideoBodyRequest request)
        {
            var processVideoRequest = new CreateProcessingOrderRequest()
            {
                CaptureInterval = request.CaptureInterval,
                ExportResolution = request.ExportResolution,
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
            return processVideoRequest;
        }

        [HttpGet("{orderId}")]
        [ProducesResponseType(typeof(GetProcessingOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CreateProcessingOrderResponse>> Get(Guid orderId)
        {
            var response = await getProcessingOrder.GetById(new GetProcessingOrderRequest
            {
                OrderId = orderId
            });

            if (response == null)
                return NotFound();

            return Ok(response);
        }
    }
}
