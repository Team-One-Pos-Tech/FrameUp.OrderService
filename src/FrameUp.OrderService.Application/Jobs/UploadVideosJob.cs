using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Domain.Entities;

namespace FrameUp.OrderService.Application.Jobs;

public record UploadVideosJob(Order Order, FileBucketRequest UploadRequest);
