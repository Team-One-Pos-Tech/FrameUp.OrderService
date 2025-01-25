﻿using FrameUp.OrderService.Domain.Enums;
using MassTransit;

namespace FrameUp.OrderService.Application.Models.Events;

[MessageUrn("frameup-order-service")]
[EntityName("update-order-status")]
public record UpdateOrderStatus(Guid OrderId, ProcessingStatus Status);
