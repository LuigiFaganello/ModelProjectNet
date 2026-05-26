
using Application.DTO;
using Application.Interfaces;
using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application
{
    public class ExampleAppServiceTests
    {
        private readonly Mock<ILogger<ExampleAppService>> _loggerMock;
        private readonly Mock<IExampleRepository> _exampleRepositoryMock;
        private readonly Mock<IExampleService> _exampleServiceMock;
        private readonly ExampleAppService _exampleAppService;

        public ExampleAppServiceTests()
        {
            _loggerMock = new Mock<ILogger<ExampleAppService>>();
            _exampleRepositoryMock = new Mock<IExampleRepository>();
            _exampleServiceMock = new Mock<IExampleService>();
            _exampleAppService = new ExampleAppService(
                _loggerMock.Object,
                _exampleRepositoryMock.Object,
                _exampleServiceMock.Object
            );
        }

        [Fact]
        public async Task GetAll_ShouldReturnSuccessResult_WithListOfExamples()
        {
            // Arrange
            var examples = new List<Example>
            {
                new Example("12345-678", "Street 1", "Complement 1", "Unit 1", "Neighborhood 1", "City 1", "ST"),
                new Example("87654-321", "Street 2", "Complement 2", "Unit 2", "Neighborhood 2", "City 2", "ST")
            };
            _exampleRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(examples);

            // Act
            var result = await _exampleAppService.GetAll(CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByZipCode_ShouldReturnSuccessResult_WhenExampleExists()
        {
            // Arrange
            var zipCode = "12345-678";
            var example = new Example(zipCode, "Street 1", "Complement 1", "Unit 1", "Neighborhood 1", "City 1", "ST");
            _exampleRepositoryMock.Setup(x => x.GetByZipCodeAsync(zipCode, It.IsAny<CancellationToken>())).ReturnsAsync(example);

            // Act
            var result = await _exampleAppService.GetByZipCode(zipCode, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value?.ZipCode.Should().Be(zipCode);
        }

        [Fact]
        public async Task GetByZipCode_ShouldReturnFailureResult_WhenExampleDoesNotExist()
        {
            // Arrange
            var zipCode = "12345-678";
            _exampleRepositoryMock.Setup(x => x.GetByZipCodeAsync(zipCode, It.IsAny<CancellationToken>())).ReturnsAsync((Example?)null);

            // Act
            var result = await _exampleAppService.GetByZipCode(zipCode, CancellationToken.None);

            // Assert
            (!result.IsSuccess).Should().BeTrue();
            result.Error.Should().Be(Error.NotFound("zipCode", zipCode));
        }

        [Fact]
        public async Task SyncCity_ShouldAddRangeAndSaveChangesOnce()
        {
            // Arrange
            var addresses = new List<AddressDto>
            {
                new AddressDto { ZipCode = "12345-678", City = "Sao Paulo", State = "SP" },
                new AddressDto { ZipCode = "87654-321", City = "Sao Paulo", State = "SP" }
            };
            _exampleServiceMock
                .Setup(x => x.GetAddressesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(addresses);

            // Act
            await _exampleAppService.SyncCity("SP", "Sao Paulo", "Paulista", CancellationToken.None);

            // Assert
            _exampleRepositoryMock.Verify(
                x => x.AddRangeAsync(It.Is<IEnumerable<Example>>(e => e.Count() == 2), It.IsAny<CancellationToken>()),
                Times.Once);
            _exampleRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SyncCity_ShouldNotPersist_WhenNoAddressesReturned()
        {
            // Arrange
            _exampleServiceMock
                .Setup(x => x.GetAddressesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<AddressDto>());

            // Act
            await _exampleAppService.SyncCity("SP", "Sao Paulo", "Paulista", CancellationToken.None);

            // Assert
            _exampleRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<Example>>(), It.IsAny<CancellationToken>()), Times.Never);
            _exampleRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
