using Flunt.Notifications;
using FrameUp.OrderService.Domain.Enums;

namespace FrameUp.OrderService.Application.Models;

public class ProcessVideoResponse: Notifiable<Notification>
{
    public ProcessingStatus Status { get; set; }
}