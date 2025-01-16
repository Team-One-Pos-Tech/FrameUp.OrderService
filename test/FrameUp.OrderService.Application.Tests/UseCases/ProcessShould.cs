using System.Text;
using FluentAssertions;
using FrameUp.OrderService.Application.Enums;
using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Application.UseCases;

namespace FrameUp.OrderService.Application.Tests.UseCases;

public class ProcessShould
{
    private ProcessVideo processVideo;

    [SetUp]
    public void Setup()
    {
        processVideo = new ProcessVideo();
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

        var response = processVideo.Execute(request);

        #endregion

        #region Assert
        
        response.IsValid.Should().BeTrue();
        
        response.Status.Should().Be(ProcessingStatus.Received);

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