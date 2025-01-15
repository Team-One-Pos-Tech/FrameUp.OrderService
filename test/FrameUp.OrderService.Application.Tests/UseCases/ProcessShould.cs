using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Application.UseCases;
using Moq;

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
        
        var content = "Mocked file content";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        
        var request = new ProcessVideoRequest
        {
            Video = stream
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
}