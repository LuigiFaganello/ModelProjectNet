using Infrastructure.ExternalService;
using Infrastructure.ExternalService.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using FluentAssertions;
using Infrastructure.Configuration;

namespace UnitTests.Infrastructure.ExternalService
{
    public class ExampleServiceTests
    {
        private readonly Mock<ILogger<ExampleService>> _loggerMock;
        private readonly Mock<IOptions<AppSettings>> _optionsMock;
        private readonly AppSettings _appSettings;

        public ExampleServiceTests()
        {
            _loggerMock = new Mock<ILogger<ExampleService>>();
            _optionsMock = new Mock<IOptions<AppSettings>>();
            _appSettings = new AppSettings
            {
                Viacep = new Viacep
                {
                    BaseUrl = "http://test.com/",
                    TimeOut = 10
                }
            };
            _optionsMock.Setup(o => o.Value).Returns(_appSettings);
        }

        private HttpClient CreateMockHttpClient(HttpStatusCode statusCode, string content)
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

            return new HttpClient(handlerMock.Object);
        }

        [Fact]
        public async Task GetCityByCountry_ShouldReturnData_OnSuccess()
        {
            // Arrange
            var country = "Brazil";
            var city = "SaoPaulo";
            var expectedDto = new List<ExampleServiceDTO> { new ExampleServiceDTO { Cep = "12345-678" } };
            var jsonResponse = "[{\"cep\": \"12345-678\"}]";
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);

            var service = new ExampleService(_loggerMock.Object, httpClient, _optionsMock.Object);

            // Act
            var result = await service.GetCityByCountry(country, city);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDto);
        }

        [Fact]
        public async Task GetCityByCountry_ShouldReturnEmptyList_OnError()
        {
            // Arrange
            var country = "Brazil";
            var city = "SaoPaulo";
            var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError, "");

            var service = new ExampleService(_loggerMock.Object, httpClient, _optionsMock.Object);

            // Act
            var result = await service.GetCityByCountry(country, city);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Erro ao obter todos os exemplos")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}