using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Jobs;
using System.Threading.Channels;

namespace FrameUp.OrderService.Application.Channels;

public class UploadVideosChannel : IUploadVideosChannel
{
    private readonly Channel<UploadVideosJob> _channel = Channel.CreateUnbounded<UploadVideosJob>();

    public ChannelWriter<UploadVideosJob> Writer => _channel.Writer;

    public ValueTask WriteAsync(UploadVideosJob job)
    {
        throw new NotImplementedException();
    }
}
