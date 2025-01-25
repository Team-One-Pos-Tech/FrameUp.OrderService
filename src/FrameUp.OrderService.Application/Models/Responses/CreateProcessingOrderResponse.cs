using Flunt.Notifications;
using FrameUp.OrderService.Domain.Enums;

namespace FrameUp.OrderService.Application.Models.Responses;

public class CreateProcessingOrderResponse : Notifiable<Notification>
{
    public ProcessingStatus Status { get; set; }
    public Guid Id { get; internal set; }
}