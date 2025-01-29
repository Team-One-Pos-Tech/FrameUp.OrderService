using FrameUp.OrderService.Api.Extensions;
using FrameUp.OrderService.Api.Mappers;
using FrameUp.OrderService.Api.Models;
using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;
using FrameUp.OrderService.Domain.Contracts;
using FrameUp.OrderService.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FrameUp.OrderService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController(
        ICreateProcessingOrder createProcessingOrder,
        IUpdateProcessingOrder updateProcessingOrder,
        IGetProcessingOrder getProcessingOrder,
        IAuthenticatedUser authenticatedUser) : ControllerBase
    {

        [HttpPost]
        [ProducesResponseType(typeof(CreateProcessingOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult<CreateProcessingOrderResponse>> Post([FromForm] ProcessVideoBodyRequest request)
        {
            var processVideoRequest = CreateProcessingOrderRequestMapper.Map(request);

            processVideoRequest.OwnerId = authenticatedUser.UserId;

            var response = await createProcessingOrder.Execute(processVideoRequest);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("{orderId:guid}")]
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

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetProcessingOrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CreateProcessingOrderResponse>> Get()
        {
            var response = await getProcessingOrder.GetAll(new GetProcessingOrderRequest
            {
                OrderId = Guid.NewGuid()
            });

            return Ok(response);
        }
        
        [HttpPut("Cancel/{orderId:guid}")]
        [ProducesResponseType(typeof(UpdateProcessingOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UpdateProcessingOrderResponse>> Put(Guid orderId)
        {
            var request = new UpdateProcessingOrderRequest
            {
                OrderId = orderId,
                Status = ProcessingStatus.Cancelled
            };

            var response = await updateProcessingOrder.Execute(request);
            
            if (!response.IsValid)
                return BadRequest(response);
            
            return Ok(response);
        }
    }
}
