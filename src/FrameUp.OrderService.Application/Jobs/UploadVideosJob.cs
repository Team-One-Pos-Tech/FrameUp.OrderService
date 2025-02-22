using FrameUp.OrderService.Application.Models.Requests;

namespace FrameUp.OrderService.Application.Jobs;

public record UploadVideosJob(FileBucketRequest UploadRequest);
