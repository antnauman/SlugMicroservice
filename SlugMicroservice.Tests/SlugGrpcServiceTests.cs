using Castle.Core.Configuration;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Configuration;
using Moq;
using SlugMicroservice.Application;
using SlugMicroservice.Models;
using SlugService.Contract.v01;
using SlugMicroservice.Respositories;
using Xunit;

namespace SlugMicroservice.Tests;

public class SlugGrpcServiceTests
{
    [Fact]
    public async Task GenerateSlug_ShouldReturnCode_WhenRepoCreatesLink()
    {
        var mockRepo = new Mock<ILinkRepository>();
        var grpcService = new SlugGrpcService(mockRepo.Object);

        string url = "https://www.google.com";

        mockRepo.Setup(x => x.CreateLink(url, It.IsAny<string>())).ReturnsAsync(true);
        var slugRequest = new GenerateSlugRequest() { Url = url };
        var serverContext = TestServerCallContext.Create(
            method: "SayHello",
            host: "localhost",
            deadline: System.DateTime.UtcNow.AddMinutes(1),
            requestHeaders: new Metadata(),
            cancellationToken: System.Threading.CancellationToken.None,
            peer: "localhost",
            authContext: null,
            contextPropagationToken: null,
            writeHeadersFunc: null,
            writeOptionsGetter: null,
            writeOptionsSetter: null
        );

        var result = await grpcService.GenerateSlug(slugRequest, serverContext);
        Assert.False(string.IsNullOrWhiteSpace(result.Code));
        mockRepo.Verify(x => x.CreateLink(url, result.Code), Times.Once);
    }

    [Fact]
    public async Task ResolveSlug_ReturnsCode_IfSlugExists()
    {
        var mockRepo = new Mock<ILinkRepository>();
        var grpcService = new SlugGrpcService(mockRepo.Object);
        var serverContext = TestServerCallContext.Create(
            method: "SayHello",
            host: "localhost",
            deadline: System.DateTime.UtcNow.AddMinutes(1),
            requestHeaders: new Metadata(),
            cancellationToken: System.Threading.CancellationToken.None,
            peer: "localhost",
            authContext: null,
            contextPropagationToken: null,
            writeHeadersFunc: null,
            writeOptionsGetter: null,
            writeOptionsSetter: null
        );
        var url = "https://www.google.com";
        var code = "qweqwe";
        var link = new Link() { Id = new Guid(), TargetUrl = url, Code = code, CreatedUtc = DateTime.UtcNow };
        mockRepo.Setup(x => x.GetLinkByCode(code)).ReturnsAsync(link);
        var request = new ResolveSlugRequest() { Code = code };
        var result = await grpcService.ResolveSlug(request, serverContext);
        Assert.Equal(url, result.Url);
        mockRepo.Verify(x => x.GetLinkByCode(code), Times.Once);
    }
}
