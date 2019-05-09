using Kokosoft.SwimmingPoolTracker.ImportSchedule;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ImportScheduleTests
{
    public class DownloadFileServiceShould
    {
        public HttpClient GetMockClient()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns((HttpRequestMessage request, CancellationToken cancellationToken) => GetMockResponse(request, cancellationToken));
            return new HttpClient(mockHttpMessageHandler.Object);
        }

        private Task<HttpResponseMessage> GetMockResponse(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            return Task.FromResult(response);
        }

        [Fact]
        public async Task ReturnFalseOnBadRequest()
        {
            var mockLogger = new Mock<ILogger<DownloadFileService>>();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));


            var mockClient = new HttpClient(mockHttpMessageHandler.Object);
            DownloadFileService fileService = new Kokosoft.SwimmingPoolTracker.ImportSchedule.DownloadFileService(mockClient, mockLogger.Object);
            Assert.False(await fileService.DownloadFile("http://localhost:3000", ""));
        }

        [Fact]
        public async Task ReturnFalseOnException()
        {
            var mockLogger = new Mock<ILogger<DownloadFileService>>();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(() => throw new Exception("HttpClient exception"));


            var mockClient = new HttpClient(mockHttpMessageHandler.Object);
            DownloadFileService fileService = new Kokosoft.SwimmingPoolTracker.ImportSchedule.DownloadFileService(mockClient, mockLogger.Object);
            Assert.False(await fileService.DownloadFile("http://localhost:3000", ""));
        }

        [Fact]
        public async Task LogErrorOnException()
        {
            var mockLogger = new Mock<TestLogger<DownloadFileService>>();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(() => throw new Exception("HttpClient exception"));


            var mockClient = new HttpClient(mockHttpMessageHandler.Object);
            DownloadFileService fileService = new Kokosoft.SwimmingPoolTracker.ImportSchedule.DownloadFileService(mockClient, mockLogger.Object);
            await fileService.DownloadFile("http://localhost:3000", "");
            mockLogger.Verify(l => l.Log(LogLevel.Error, It.IsAny<Exception>(), "Error during http://localhost:3000 file download."));
        }
    }
}
