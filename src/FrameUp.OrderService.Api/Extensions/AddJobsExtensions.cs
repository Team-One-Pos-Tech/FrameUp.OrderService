

using FrameUp.OrderService.Application.Channels;
using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Jobs;
using System.Threading.Channels;

namespace FrameUp.OrderService.Api.Extensions;

public static class AddJobsExtensions
{
    public static IServiceCollection AddJobs(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(_ =>
        {
            var channel = Channel.CreateBounded<UploadVideosJob>(new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait,
            });

            return channel;
        });

        serviceCollection.AddSingleton<IUploadVideosChannel, UploadVideosChannel>();

        return serviceCollection;
    }
}