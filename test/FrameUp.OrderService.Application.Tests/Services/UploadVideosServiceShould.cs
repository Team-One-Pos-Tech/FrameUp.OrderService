using FluentAssertions;
using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Jobs;
using FrameUp.OrderService.Application.Models.Events;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Services;
using FrameUp.OrderService.Domain.Contracts;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FrameUp.OrderService.Application.Tests.Services;

public class UploadVideosServiceShould
{
    private UploadVideosService _uploadVideosService;
    private Mock<ILogger<UploadVideosService>> _loggerMock;
    private Mock<IServiceProvider> _serviceProviderMock;
    private Mock<IFileBucketRepository> _fileBucketRepositoryMock;
    private Mock<ILocalStoreRepository> _localStoreRepositoryMock;
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<IUploadVideosChannel> _channel;
    private Mock<IPublishEndpoint> _publishEndpointMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<UploadVideosService>>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _fileBucketRepositoryMock = new Mock<IFileBucketRepository>();
        _localStoreRepositoryMock = new Mock<ILocalStoreRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _channel = new Mock<IUploadVideosChannel>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();

        var serviceScopeMock = new Mock<IServiceScope>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();

        serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);

        _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactoryMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IFileBucketRepository))).Returns(_fileBucketRepositoryMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(ILocalStoreRepository))).Returns(_localStoreRepositoryMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IOrderRepository))).Returns(_orderRepositoryMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IPublishEndpoint))).Returns(_publishEndpointMock.Object);

        _uploadVideosService = new UploadVideosService(_loggerMock.Object, _channel.Object, _serviceProviderMock.Object);
    }

    [Test]
    public async Task Process_Upload_Video_Job()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Status = ProcessingStatus.Processing,
            Videos = new List<VideoMetadata>
            {
                new VideoMetadata { Name = "video1.mp4", ContentType = "video/mp4", Size = 1000 },
                new VideoMetadata { Name = "video2.mp4", ContentType = "video/mp4", Size = 2000 }
            }
        };

        var job = new UploadVideosJob(order);

        var jobsList = GetAsyncEnumerable(job);

        _channel.Setup(x => x.ReadAllAsync(It.IsAny<CancellationToken>()))
            .Returns(jobsList);

        // Act
        await _uploadVideosService.StartExecuteAsync(CancellationToken.None);

        // Assert
        _fileBucketRepositoryMock.Setup(repository => repository.Upload(
            It.Is<FileBucketRequest>(request => request.OrderId == order.Id && request.Files.Count() == order.Videos.Count()))); 

    }

    [Test]
    public async Task Emit_ReadyToProcessVideo_Event_With_Parameters()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Status = ProcessingStatus.Processing,
            ExportResolution = ResolutionTypes.HD,
            Videos = new List<VideoMetadata>
            {
                new VideoMetadata { Name = "video1.mp4", ContentType = "video/mp4", Size = 1000 },
                new VideoMetadata { Name = "video2.mp4", ContentType = "video/mp4", Size = 2000 }
            }
        };

        var job = new UploadVideosJob(order);

        var jobsList = GetAsyncEnumerable(job);

        _channel.Setup(x => x.ReadAllAsync(It.IsAny<CancellationToken>()))
            .Returns(jobsList);

        // Act
        await _uploadVideosService.StartExecuteAsync(CancellationToken.None);

        // Assert
        _publishEndpointMock.Verify(publishEndpoint => publishEndpoint.Publish(
            It.Is<ReadyToProcessVideo>(message =>
                message.Parameters.ExportResolution == ResolutionTypes.HD &&
                message.Parameters.CaptureInterval == order.CaptureInterval
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

    }

    [Test]
    public async Task Update_Order_Status()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Status = ProcessingStatus.Received,
            Videos = new List<VideoMetadata>
            {
                new VideoMetadata { Name = "video1.mp4", ContentType = "video/mp4", Size = 1000 },
                new VideoMetadata { Name = "video2.mp4", ContentType = "video/mp4", Size = 2000 }
            }
        };

        var job = new UploadVideosJob(order);

        var jobsList = GetAsyncEnumerable(job);

        _channel.Setup(x => x.ReadAllAsync(It.IsAny<CancellationToken>()))
            .Returns(jobsList);

        // Act
        await _uploadVideosService.StartExecuteAsync(CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(repository => repository.Update(
            It.IsAny<Order>()
        ), Times.Exactly(2));

    }

    private async IAsyncEnumerable<UploadVideosJob> GetAsyncEnumerable(UploadVideosJob job)
    {
        yield return job;
        await Task.CompletedTask;
    }
}
