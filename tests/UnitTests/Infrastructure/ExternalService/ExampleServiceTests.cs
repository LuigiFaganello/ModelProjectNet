using Application.DTO;
using Infrastructure.ExternalService;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using FluentAssertions;

namespace UnitTests.Infrastructure.ExternalService
{
    public class ExampleServiceTests
    {
        private readonly Mock<ILogger<ExampleService>> _loggerMock;

        public ExampleServiceTests()
        {
            _loggerMock = new Mock<ILogger<ExampleService>>();
        }

        private static HttpClient CreateMockHttpClient(HttpStatusCode statusCode, string content)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content),
                })
                .Verifiable();

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/")
            };
        }

        [Fact]
        public async Task GetAddressesAsync_ShouldReturnMappedData_OnSuccess()
        {
            // Arrange
            var jsonResponse = "[{\"cep\": \"12345-678\", \"localidade\": \"Sao Paulo\", \"uf\": \"SP\"}]";
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);

            var service = new ExampleService(_loggerMock.Object, httpClient);

            // Act
            var result = (await service.GetAddressesAsync("SP", "Sao Paulo", "Paulista", CancellationToken.None)).ToList();

            // Assert
            result.Should().HaveCount(1);
            result[0].ZipCode.Should().Be("12345-678");
            result[0].City.Should().Be("Sao Paulo"); // mapeado de 'localidade'
            result[0].State.Should().Be("SP");        // mapeado de 'uf'
        }

        [Fact]
        public async Task GetAddressesAsync_ShouldReturnEmptyList_OnError()
        {
            // Arrange
            var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError, "");

            var service = new ExampleService(_loggerMock.Object, httpClient);

            // Act
            var result = await service.GetAddressesAsync("SP", "Sao Paulo", "Paulista", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("Erro ao obter endereços do serviço externo")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }
    }
}
