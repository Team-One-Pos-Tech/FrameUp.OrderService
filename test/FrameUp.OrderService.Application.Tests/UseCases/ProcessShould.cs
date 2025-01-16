using System.Text;
using FluentAssertions;
using FrameUp.OrderService.Application.Enums;
using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Application.Repositories;
using FrameUp.OrderService.Application.UseCases;
using Moq;

namespace FrameUp.OrderService.Application.Tests.UseCases;

public class ProcessShould
{
    private ProcessVideo processVideo;
    private Mock<IFileBucket> fileBucketMock;

    [SetUp]
    public void Setup()
    {
        fileBucketMock = new Mock<IFileBucket>();
        processVideo = new ProcessVideo(fileBucketMock.Object);
    }
    
    [Test]
    public async Task Start_Process_Video()
    {
        #region Arrange
        
        var video = CreateFakeVideo();

        var request = new ProcessVideoRequest
        {
            Video = video
        };
        
        #endregion

        #region Act

        var response = await processVideo.Execute(request);

        #endregion

        #region Assert
        
        response.IsValid.Should().BeTrue();
        
        response.Status.Should().Be(ProcessingStatus.Received);

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
            VideoName = videoName
        };
        
        #endregion

        #region Act

        var response = await processVideo.Execute(request);

        #endregion

        #region Assert
        
        response.IsValid.Should().BeTrue();
        
        fileBucketMock.Verify(x => x.Save(video, videoName), Times.Once);

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
            VideoName = videoName,
            VideoSize = 1024L * 1024L * 1024L + 512L, // 1 GB in bytes
        };
        
        #endregion

        #region Act

        var response = await processVideo.Execute(request);

        #endregion

        #region Assert
        
        response.IsValid.Should().BeFalse();
        
        response.Status.Should().Be(ProcessingStatus.Refused);

        response.Notifications.First().Message.Should().Be("Video size is too large");
        
        fileBucketMock.Verify(x => x.Save(video, videoName), Times.Never);

        #endregion
    }
    
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
}