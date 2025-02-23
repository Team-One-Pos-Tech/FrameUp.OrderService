using System.Text;
using FluentAssertions;
using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Jobs;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.UseCases;
using FrameUp.OrderService.Domain.Contracts;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace FrameUp.OrderService.Application.Tests.UseCases;

public class CreateProcessingOrderShould
{
    private CreateProcessingOrder _createProcessingOrder;
    private Mock<IOrderRepository> _orderRepository;
    private Mock<ILogger<CreateProcessingOrder>> _loggerMock;
    private Mock<ILocalStoreRepository> _localStoreRepositoryMock;
    private Mock<IUploadVideosChannel> _channelMock;

    [SetUp]
    public void Setup()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _loggerMock = new Mock<ILogger<CreateProcessingOrder>>();
        _localStoreRepositoryMock = new Mock<ILocalStoreRepository>();
        _channelMock = new Mock<IUploadVideosChannel>();

        _createProcessingOrder = new CreateProcessingOrder(
            _loggerMock.Object,
            _orderRepository.Object,
            _channelMock.Object,
            _localStoreRepositoryMock.Object);
    }

    [Test]
    public async Task Start_Processing_Video()
    {
        #region Arrange

        var video = CreateFakeVideo();

        var request = new CreateProcessingOrderRequest
        {
            Videos = [
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = "marketing.mp4",
                        ContentType = "video/mp4",
                        Size = 1024L * 1024L
                    }
                }
            ]
        };

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeTrue();

        response.Status.Should().Be(ProcessingStatus.Processing);

        #endregion
    }

    [Test]
    public async Task Upload_Video()
    {
        #region Arrange

        var video = CreateFakeVideo();

        const string videoName = "marketing.mp4";
        var request = new CreateProcessingOrderRequest
        {
            Videos = [
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = videoName,
                        ContentType = "video/mp4",
                        Size = 1024L * 1024L
                    }
                },
            ]
        };

        var orderId = Guid.NewGuid();

        _orderRepository.Setup(repository => repository.Save(It.IsAny<Order>()))
            .ReturnsAsync(orderId);

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeTrue();

        _channelMock.Verify(mock => mock.WriteAsync(
            It.Is<UploadVideosJob>(fileRequest => fileRequest.Order.Id == orderId)
        ), Times.Once);

        #endregion
    }

    [Test]
    public async Task Upload_Many_Videos_Simultaneously()
    {
        #region Arrange

        var video1 = CreateFakeVideo();
        var video2 = CreateFakeVideo();

        const string video1Name = "marketing.mp4";
        const string video2Name = "commercial.mp4";

        var request = new CreateProcessingOrderRequest
        {
            Videos = [
                new VideoRequest
                {
                    ContentStream = video1,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = video1Name,
                        ContentType = "video/mp4",
                        Size = 1024L * 1024L
                    }
                },
                new VideoRequest
                {
                    ContentStream = video2,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = video2Name,
                        ContentType = "video/mp4",
                        Size = 1024L * 1024L
                    }
                }
            ]
        };

        var orderId = Guid.NewGuid();

        _orderRepository.Setup(repository => repository.Save(It.IsAny<Order>()))
            .ReturnsAsync(orderId);

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeTrue();

        _channelMock.Verify(mock => mock.WriteAsync(
            It.Is<UploadVideosJob>(fileRequest => fileRequest.Order.Id == orderId)
        ), Times.Once);

        #endregion
    }

    [Test]
    public async Task Validate_Video_Size_When_Is_Bigger_Than_1gb()
    {
        #region Arrange

        var video = CreateFakeVideo();

        const string videoName = "marketing.mp4";
        var request = new CreateProcessingOrderRequest
        {
            Videos = [
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = videoName,
                        Size = 1024L * 1024L * 1024L + 512L, // 1 GB in bytes
                        ContentType = "video/mp4"
                    }
                }
            ]
        };

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeFalse();

        response.Status.Should().Be(ProcessingStatus.Refused);

        response.Notifications.First().Message.Should().Be("Video size is too large.");

        _localStoreRepositoryMock.Verify(localStoreRepository => localStoreRepository.SaveFileAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<Stream>()
        ), Times.Never);

        #endregion
    }

    [Test]
    public async Task Validate_Video_Content_Type_When_Is_Not_Mp4()
    {
        #region Arrange

        var video = CreateFakeVideo();

        const string videoName = "marketing.txt";
        var request = new CreateProcessingOrderRequest
        {
            Videos = [
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = videoName,
                        ContentType = "text/plain",
                        Size = 1024L * 1024L
                    }
                }
            ]
        };

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeFalse();

        response.Status.Should().Be(ProcessingStatus.Refused);

        response.Notifications.First().Message.Should().Be("File type not supported.");

        _localStoreRepositoryMock.Verify(localStoreRepository => localStoreRepository.SaveFileAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<Stream>()
        ), Times.Never);

        #endregion
    }

    [Test]
    public async Task Validate_Video_Count_When_Is_Up_To_3()
    {
        #region Arrange

        var video = CreateFakeVideo();

        const string videoName = "marketing.txt";
        var request = new CreateProcessingOrderRequest
        {
            Videos = [
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = videoName,
                        ContentType = "text/plain",
                        Size = 1024L * 1024L
                    }
                },
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = videoName,
                        ContentType = "text/plain",
                        Size = 1024L * 1024L
                    }
                },
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = videoName,
                        ContentType = "text/plain",
                        Size = 1024L * 1024L
                    }
                },
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = videoName,
                        ContentType = "text/plain",
                        Size = 1024L * 1024L
                    }
                }
            ]
        };

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeFalse();

        response.Status.Should().Be(ProcessingStatus.Refused);

        response.Notifications.First().Message.Should().Be("Max supported videos processing is 3.");

        _localStoreRepositoryMock.Verify(localStoreRepository => localStoreRepository.SaveFileAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<Stream>()
        ), Times.Never);

        #endregion
    }

    [Test]
    public async Task Validate_When_Video_Content_Is_Empty()
    {
        #region Arrange

        var request = new CreateProcessingOrderRequest
        {
        };

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeFalse();

        response.Status.Should().Be(ProcessingStatus.Refused);

        response.Notifications.First().Message.Should().Be("At least one video is required.");

        _localStoreRepositoryMock.Verify(localStoreRepository => localStoreRepository.SaveFileAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<Stream>()
        ), Times.Never);

        #endregion
    }

    [Test]
    public async Task Validate_When_Capture_Interval_Is_Less_Than_1()
    {
        #region Arrange

        var request = new CreateProcessingOrderRequest
        {
            CaptureInterval = 0,
            Videos = [
                new VideoRequest
                {
                    ContentStream = CreateFakeVideo(),
                    Metadata = new VideoMetadataRequest
                    {
                        Name = "videoName",
                        ContentType = "text/plain",
                        Size = 1024L * 1024L
                    }
                },
            ]
        };

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeFalse();

        response.Status.Should().Be(ProcessingStatus.Refused);

        response.Notifications.First().Message.Should().Be("Capture interval should be more than 1.");

        _localStoreRepositoryMock.Verify(localStoreRepository => localStoreRepository.SaveFileAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<Stream>()
        ), Times.Never);

        #endregion
    }

    [Test]
    public async Task Persist_Order_With_Video_Metadata()
    {
        #region Arrange

        var video = CreateFakeVideo();

        const string videoName = "marketing.txt";
        var size = 1024L * 1024L;
        var contentType = "video/mp4";

        var request = new CreateProcessingOrderRequest
        {
            Videos = [
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = videoName,
                        ContentType = contentType,
                        Size = size
                    }
                }
            ]
        };

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeTrue();

        _orderRepository.Verify(repository => repository.Save(
            It.Is<Order>(order => order.Videos.First().Name == videoName &&
                                  order.Videos.First().Size == size &&
                                  order.Videos.First().ContentType == contentType)
        ), Times.Once);

        #endregion
    }

    [Test]
    public async Task Persist_Order_With_ExportResolution()
    {
        #region Arrange

        var video = CreateFakeVideo();

        const string videoName = "marketing.txt";
        var size = 1024L * 1024L;
        var contentType = "video/mp4";

        var request = new CreateProcessingOrderRequest
        {
            ExportResolution = ResolutionTypes.FullHD,
            Videos = [
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = videoName,
                        ContentType = contentType,
                        Size = size
                    }
                }
            ]
        };

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeTrue();

        _orderRepository.Verify(repository => repository.Save(
            It.Is<Order>(order => order.ExportResolution == ResolutionTypes.FullHD)
        ), Times.Once);

        #endregion
    }

    [Test]
    public async Task Persist_Order_With_ExportResolution_FullHD_When_Is_Null()
    {
        #region Arrange

        var video = CreateFakeVideo();

        const string videoName = "marketing.txt";
        var size = 1024L * 1024L;
        var contentType = "video/mp4";

        var request = new CreateProcessingOrderRequest
        {
            ExportResolution = null,
            Videos = [
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = videoName,
                        ContentType = contentType,
                        Size = size
                    }
                }
            ]
        };

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeTrue();

        _orderRepository.Verify(repository => repository.Save(
            It.Is<Order>(order => order.ExportResolution == ResolutionTypes.FullHD)
        ), Times.Once);

        #endregion
    }


    [Test]
    public async Task Persist_Order_With_CaptureInterval()
    {
        #region Arrange

        var video = CreateFakeVideo();

        const int captureInterval = 10;

        var request = new CreateProcessingOrderRequest
        {
            CaptureInterval = captureInterval,
            Videos = [
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = "marketing.txt",
                        ContentType = "video/mp4",
                        Size = 1024L * 1024L
                    }
                }
            ]
        };

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeTrue();

        _orderRepository.Verify(repository => repository.Save(
            It.Is<Order>(order => order.CaptureInterval == captureInterval)
        ), Times.Once);

        #endregion
    }

    [Test]
    public async Task Persist_Order_With_OwnerId()
    {
        #region Arrange

        var video = CreateFakeVideo();

        Guid ownerId = Guid.NewGuid();

        var request = new CreateProcessingOrderRequest
        {
            OwnerId = ownerId,
            Videos = [
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = "marketing.txt",
                        ContentType = "video/mp4",
                        Size = 1024L * 1024L
                    }
                }
            ]
        };

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeTrue();

        _orderRepository.Verify(repository => repository.Save(
            It.Is<Order>(order => order.OwnerId == ownerId)
        ), Times.Once);

        #endregion
    }

    [Test]
    public async Task Save_Video_Files_Locally()
    {
        #region Arrange

        var video = CreateFakeVideo();

        var request = new CreateProcessingOrderRequest
        {
            ExportResolution = ResolutionTypes.HD,
            CaptureInterval = 10,
            Videos = [
                new VideoRequest
                {
                    ContentStream = video,
                    Metadata = new VideoMetadataRequest
                    {
                        Name = "marketing.mp4",
                        ContentType = "video/mp4",
                        Size = 1024L * 1024L
                    }
                }
            ]
        };

        var orderId = Guid.NewGuid();

        _orderRepository.Setup(repository => repository.Save(It.IsAny<Order>()))
            .ReturnsAsync(orderId);

        #endregion

        #region Act

        var response = await _createProcessingOrder.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeTrue();

        _localStoreRepositoryMock.Verify(localStoreRepository => localStoreRepository.SaveFileAsync(
            orderId,
            request.Videos.First().Metadata.Name,
            video
        ), Times.Once);

        #endregion
    }

    #region Helpers

    private static MemoryStream CreateFakeVideo()
    {
        var content = "This is some content for the MemoryStream.";
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var filePath = "output.mp4";

        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            memoryStream.WriteTo(fileStream);
        }

        return memoryStream;
    }

    #endregion
}