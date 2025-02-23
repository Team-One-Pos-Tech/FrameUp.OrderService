using FrameUp.OrderService.Application.Jobs;
using System.Threading.Channels;

namespace FrameUp.OrderService.Application.Contracts;

public interface IUploadVideosChannel
{
    ChannelWriter<UploadVideosJob> Writer { get; }

    ValueTask WriteAsync(UploadVideosJob job);
}
