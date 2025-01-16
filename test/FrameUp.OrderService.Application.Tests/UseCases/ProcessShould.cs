using System.Text;
using FluentAssertions;
using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Application.Repositories;
using FrameUp.OrderService.Application.UseCases;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using Moq;

namespace FrameUp.OrderService.Application.Tests.UseCases;

public class ProcessShould
{
    private ProcessVideo _processVideo;
    private Mock<IFileBucketRepository> _fileBucketMock;
    private Mock<IOrderRepository> _orderRepository;

    [SetUp]
    public void Setup()
    {
        _fileBucketMock = new Mock<IFileBucketRepository>();
        _orderRepository = new Mock<IOrderRepository>();
        _processVideo = new ProcessVideo(_fileBucketMock.Object,_orderRepository.Object);
    }

    [Test]
    public async Task Start_Process_Video()
    {
        #region Arrange

        var video = CreateFakeVideo();

        var request = new ProcessVideoRequest
        {
            Video = video,
            VideoMetadata = new VideoMetadataRequest
            {
                ContentType = "video/mp4"
            },
        };

        #endregion

        #region Act

        var response = await _processVideo.Execute(request);

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
        var request = new ProcessVideoRequest
        {
            Video = video,
            VideoMetadata = new VideoMetadataRequest
            {
                Name = videoName,
                ContentType = "video/mp4"
            },
        };

        #endregion

        #region Act

        var response = await _processVideo.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeTrue();

        _fileBucketMock.Verify(x => x.Save(
            It.IsAny<Stream>(),
            It.Is<VideoMetadataRequest>(v => v.Name == videoName && v.ContentType == "video/mp4")
        ), Times.Once);

        #endregion
    }

    [Test]
    public async Task Validate_Video_Size_When_Is_Bigger_Than_1gb()
    {
        #region Arrange

        var video = CreateFakeVideo();

        const string videoName = "marketing.mp4";
        var request = new ProcessVideoRequest
        {
            Video = video,
            VideoMetadata = new VideoMetadataRequest
            {
                Name = videoName,
                Size = 1024L * 1024L * 1024L + 512L, // 1 GB in bytes
                ContentType = "video/mp4"
            },
        };

        #endregion

        #region Act

        var response = await _processVideo.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeFalse();

        response.Status.Should().Be(ProcessingStatus.Refused);

        response.Notifications.First().Message.Should().Be("Video size is too large.");

        _fileBucketMock.Verify(x => x.Save(video, new VideoMetadataRequest()), Times.Never);

        #endregion
    }

    [Test]
    public async Task Validate_Video_Content_Type_When_Is_Not_Mp4()
    {
        #region Arrange

        var video = CreateFakeVideo();

        const string videoName = "marketing.txt";
        var request = new ProcessVideoRequest
        {
            Video = video,
            VideoMetadata = new VideoMetadataRequest
            {
                Name = videoName,
                Size = 1024L * 1024L,
                ContentType = "text/plain"
            },
        };

        #endregion

        #region Act

        var response = await _processVideo.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeFalse();

        response.Status.Should().Be(ProcessingStatus.Refused);

        response.Notifications.First().Message.Should().Be("File type not supported.");

        _fileBucketMock.Verify(x => x.Save(video, new VideoMetadataRequest()), Times.Never);

        #endregion
    }

    [Test]
    public async Task Persist_Order_With_Video_Metadata()
    {
        #region Arrange

        var video = CreateFakeVideo();

        const string videoName = "marketing.txt";
        var request = new ProcessVideoRequest
        {
            Video = video,
            VideoMetadata = new VideoMetadataRequest
            {
                Name = videoName,
                Size = 1024L * 1024L,
                ContentType = "video/mp4"
            },
        };

        #endregion

        #region Act

        var response = await _processVideo.Execute(request);

        #endregion

        #region Assert

        response.IsValid.Should().BeTrue();
        
        _orderRepository.Verify(repository => repository.Save(
            It.Is<Order>(order => order.VideoMetadata.Name == videoName && 
                                  order.VideoMetadata.Size == 1024L * 1024L &&
                                  order.VideoMetadata.ContentType == "text/plain")
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