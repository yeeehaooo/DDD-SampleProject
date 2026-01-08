using FluentAssertions;
using SampleProject.Domain.Entities;
using SampleProject.Domain.Exceptions;
using Xunit;

namespace SampleProject.Domain.Tests.Entities;

public class ProductTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        var name = "Test Product";
        var description = "Test Description";
        var basePrice = 99.99m;

        // Act
        var product = new Product(name, description, basePrice);

        // Assert
        product.Should().NotBeNull();
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.BasePrice.Should().Be(basePrice);
        product.ProductId.Should().NotBeEmpty();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowDomainException()
    {
        // Arrange & Act
        var act = () => new Product("", "Description", 10m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void Constructor_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var act = () => new Product(null!, "Description", 10m);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNameExceedingMaxLength_ShouldThrowDomainException()
    {
        // Arrange
        var longName = new string('A', 201);

        // Act
        var act = () => new Product(longName, "Description", 10m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*cannot exceed 200 characters*");
    }

    [Fact]
    public void UpdateBasePrice_WithNegativeValue_ShouldThrowDomainException()
    {
        // Arrange
        var product = new Product("Test", "Description", 10m);

        // Act
        var act = () => product.UpdateBasePrice(-1m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void UpdateBasePrice_WithValidValue_ShouldUpdateBasePrice()
    {
        // Arrange
        var product = new Product("Test", "Description", 10m);
        var newBasePrice = 20m;

        // Act
        product.UpdateBasePrice(newBasePrice);

        // Assert
        product.BasePrice.Should().Be(newBasePrice);
        product.UpdatedAt.Should().NotBeNull();
    }
}
