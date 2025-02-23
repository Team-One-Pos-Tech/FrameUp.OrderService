using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Jobs;
using System.Threading.Channels;

namespace FrameUp.OrderService.Application.Channels;

public class UploadVideosChannel(Channel<UploadVideosJob> _channel) : IUploadVideosChannel
{
    public ChannelWriter<UploadVideosJob> Writer => _channel.Writer;

    public IAsyncEnumerable<UploadVideosJob> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }

    public async ValueTask WriteAsync(UploadVideosJob job)
    {
        await _channel.Writer.WriteAsync(job);
    }
}
