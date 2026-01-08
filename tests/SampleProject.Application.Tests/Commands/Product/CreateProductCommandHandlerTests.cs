using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SampleProject.Application.Commands.Product;
using SampleProject.Application.DTOs;
using SampleProject.Domain.Entities;
using SampleProject.Domain.Exceptions;
using SampleProject.Domain.Interfaces;
using Xunit;

namespace SampleProject.Application.Tests.Commands.Product;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldCreateProduct()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var loggerMock = new Mock<ILogger<CreateProductCommandHandler>>();

        Domain.Entities.Product? savedProduct = null;
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Product>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Product, CancellationToken>((p, ct) => savedProduct = p)
            .Returns(Task.CompletedTask);
        repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateProductCommandHandler(repositoryMock.Object, loggerMock.Object);
        var command = new CreateProductCommand("Test Product", "Test Description", 99.99m);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);
        result.BasePrice.Should().Be(command.BasePrice);
        result.ProductId.Should().NotBeEmpty();

        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Product>(), It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        savedProduct.Should().NotBeNull();
        savedProduct!.Name.Should().Be(command.Name);
        savedProduct!.ProductId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task HandleAsync_WithInvalidCommand_ShouldThrowDomainException()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var loggerMock = new Mock<ILogger<CreateProductCommandHandler>>();

        var handler = new CreateProductCommandHandler(repositoryMock.Object, loggerMock.Object);
        var command = new CreateProductCommand("", "Description", 10m); // Empty name

        // Act
        var act = async () => await handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
        repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
