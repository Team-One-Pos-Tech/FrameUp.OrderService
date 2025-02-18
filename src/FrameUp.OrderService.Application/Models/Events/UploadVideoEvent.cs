using FrameUp.OrderService.Application.Models.Requests;
using MassTransit;

namespace FrameUp.OrderService.Application.Models.Events;


[MessageUrn("frameup-order-service")]
[EntityName("upload-video")]
public class UploadVideoEvent
{
    public Guid OrderId { get; set; }

    public IEnumerable<FileRequest> Files { get; set; } = [];
}
