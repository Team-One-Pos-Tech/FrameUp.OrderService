﻿using Flunt.Notifications;
using FrameUp.OrderService.Domain.Enums;

namespace FrameUp.OrderService.Application.Models.Responses;

public class GetProcessingOrderResponse: Notifiable<Notification> 
{
    public Guid Id { get; set; }

    public ProcessingStatus Status { get; set; }

    public Guid OwnerId { get; set; }

    public IEnumerable<VideoMetadataResponse> Videos { get; set; } = [];

    public ResolutionTypes ExportResolution { get; set; }

    public int? CaptureInterval { get; set; }

    public PackageItemResponse[] Packages { get; set; } = [];
}
