
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace UnitTests.Infrastructure.Repositories
{
    public class ExampleRepositoryTests
    {
        private readonly DbContextOptions<DataContext> _dbContextOptions;

        public ExampleRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [Fact]
        public async Task GetByZipCodeAsync_ShouldReturnExample_WhenExampleExists()
        {
            // Arrange
            var zipCode = "12345-678";
            var example = new Example(zipCode, "Street 1", "Complement 1", "Unit 1", "Neighborhood 1", "City 1", "ST");

            using (var context = new DataContext(_dbContextOptions))
            {
                context.Examples.Add(example);
                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(_dbContextOptions))
            {
                var repository = new ExampleRepository(context);

                // Act
                var result = await repository.GetByZipCodeAsync(zipCode, CancellationToken.None);

                // Assert
                result.Should().NotBeNull();
                result.ZipCode.Should().Be(zipCode);
            }
        }

        [Fact]
        public async Task GetByZipCodeAsync_ShouldReturnNull_WhenExampleDoesNotExist()
        {
            // Arrange
            var zipCode = "12345-678";

            using (var context = new DataContext(_dbContextOptions))
            {
                var repository = new ExampleRepository(context);

                // Act
                var result = await repository.GetByZipCodeAsync(zipCode, CancellationToken.None);

                // Assert
                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddAsync_ShouldAddExample()
        {
            // Arrange
            var example = new Example("12345-678", "Street 1", "Complement 1", "Unit 1", "Neighborhood 1", "City 1", "ST");

            using (var context = new DataContext(_dbContextOptions))
            {
                var repository = new ExampleRepository(context);

                // Act
                await repository.AddAsync(example, CancellationToken.None);
                await repository.SaveChangesAsync(CancellationToken.None);
            }

            // Assert
            using (var context = new DataContext(_dbContextOptions))
            {
                var result = await context.Examples.FirstOrDefaultAsync(x => x.ZipCode == "12345-678");
                result.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllExamples()
        {
            // Arrange
            using (var context = new DataContext(_dbContextOptions))
            {
                context.Examples.Add(new Example("12345-678", "Street 1", "Complement 1", "Unit 1", "Neighborhood 1", "City 1", "ST"));
                context.Examples.Add(new Example("87654-321", "Street 2", "Complement 2", "Unit 2", "Neighborhood 2", "City 2", "ST"));
                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(_dbContextOptions))
            {
                var repository = new ExampleRepository(context);

                // Act
                var result = await repository.GetAllAsync(CancellationToken.None);

                // Assert
                result.Should().HaveCount(2);
            }
        }
    }
}
