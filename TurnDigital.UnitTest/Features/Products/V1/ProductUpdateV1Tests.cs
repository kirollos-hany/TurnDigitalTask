using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using TurnDigital.Application.Features.Products.Commands.V1;
using TurnDigital.Application.Features.Products.Handlers.V1;
using TurnDigital.Application.Features.Products.Validators.V1;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Categories.Specifications;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Domain.Features.Products.Specifications;
using TurnDigital.Domain.IO;
using TurnDigital.Domain.IO.Interfaces;
using TurnDigital.Infrastructure.DataAccess;
using TurnDigital.Infrastructure.DataAccess.Seed;

namespace TurnDigital.UnitTest.Features.Products.V1;

public class ProductUpdateV1Tests
{
    private readonly IRepository _repository;

    private readonly Mock<IFileManager> _fileManager;

    private readonly IValidator<UpdateProductV1> _validator;

    public ProductUpdateV1Tests()
    {
        _fileManager = new Mock<IFileManager>();
        
        // Mock file manager
        _fileManager.Setup(manager => manager.DeleteAsync(It.IsAny<string>()));

        var dbContextOptions = new DbContextOptionsBuilder<TurnDigitalDbContext>()
            .UseInMemoryDatabase("TurnDigitalTestDb")
            .Options;

        var dbContext = new TurnDigitalDbContext(dbContextOptions);

        _repository = new Repository(dbContext);

        _validator = new UpdateProductV1Validator();

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
    }

    [Fact]
    public async Task Handle_ShouldFailValidation()
    {
        // Arrange
        var category = _repository.GetEntitySet<Category>().Add(new Category { Name = "Category" }).Entity;

        var productToUpdate = _repository.GetEntitySet<Product>().Add(new Product
        {          
            Image = "product/default.png",
            Category = category,
            Description = null,
            Name = "Product 2",
            Price = 500,
            Slug = "Product-2"
        }).Entity;

        await _repository.SaveChangesAsync();
        
        await using var memoryStream = new MemoryStream();
        var handler = new UpdateProductV1Handler(_repository, _fileManager.Object, _validator);
        var command = new UpdateProductV1(productToUpdate.Id, new FileModel(memoryStream, "image.xls", long.MaxValue), null, null, -1);

        //Act
        var result = await handler.Handle(command, default);


        //Assert
        result.Case.Should().BeOfType<FailureResponse<ValidationFailureResponse>>()
            .And.Subject.As<FailureResponse<ValidationFailureResponse>>().ResponseData.As<ValidationFailureResponse>()
            .ValidationMap.Should().ContainKeys("Image", "Price", "Name");

        _repository.GetEntitySet<Product>().Remove(productToUpdate);
        _repository.GetEntitySet<Category>().Remove(category);

        await _repository.SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound()
    {
        // Arrange
        await using var memoryStream = new MemoryStream();
        var handler = new UpdateProductV1Handler(_repository, _fileManager.Object, _validator);
        var command = new UpdateProductV1(int.MaxValue, new FileModel(memoryStream, "image.webp", long.MaxValue), "Product 1", null, 500);

        //Act
        var result = await handler.Handle(command, default);

        //Assert
        result.Case.Should().BeOfType<FailureResponse<NotFoundResponse>>()
            .And.Subject.As<FailureResponse<NotFoundResponse>>().ResponseData.Should().BeOfType<NotFoundResponse>();
    }
    
    [Fact]
    public async Task Handle_ShouldUpdateProduct()
    {
        // Arrange
        var category = _repository.GetEntitySet<Category>().Add(new Category { Name = "Category" }).Entity;

        var productToUpdate = _repository.GetEntitySet<Product>().Add(new Product
        {          
            Image = "product/default.png",
            Category = category,
            Description = null,
            Name = "Product 2",
            Price = 500,
            Slug = "Product-2"
        }).Entity;

        await _repository.SaveChangesAsync();

        await using var memoryStream = new MemoryStream();
                var handler = new UpdateProductV1Handler(_repository, _fileManager.Object, _validator);
        var command = new UpdateProductV1(productToUpdate.Id, new FileModel(memoryStream, "image.webp", long.MaxValue), "Product 1",
            "Some Description", 500);
        
        // mock file manager
        _fileManager
            .Setup(manager => manager.SaveAsync(It.IsAny<FileModel>(),
                It.Is<string>(nameof(Product), StringComparer.OrdinalIgnoreCase)))
            .ReturnsAsync("product/image.webp");

        // Act
        var result = await handler.Handle(command, default);

        //Assert
        result.Case.Should().BeOfType<ProductDto>()
            .Subject.Name.Should().Be("Product 1");

        result.Case.Should().BeOfType<ProductDto>()
            .Subject.Description.Should().Be("Some Description");

        result.Case.Should().BeOfType<ProductDto>()
            .Subject.Price.Should().Be(500);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject.Slug.Should().Be("Product-1");
        
        result.Case.Should().BeOfType<ProductDto>()
            .Subject.Image.Should().Be("product/image.webp");
        
        result.Case.Should().BeOfType<ProductDto>()
            .Subject.UtcCreatedDate.Should().Be(productToUpdate.UtcCreatedDate);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Id
            .Should().Be(productToUpdate.Id);

        var updatedProduct = await _repository.GetEntitySet<Product>()
            .ById(result.Case.Should().BeOfType<ProductDto>().Subject.Id)
            .FirstOrDefaultAsync();

        updatedProduct.Should().NotBeNull();
        updatedProduct?.Name.Should().Be("Product 1");
        updatedProduct?.Description.Should().Be("Some Description");
        updatedProduct?.Slug.Should().Be("Product-1");
        updatedProduct?.Price.Should().Be(500);
        updatedProduct?.Id.Should().Be(productToUpdate.Id);
        updatedProduct?.Image.Should().Be("product/image.webp");
        updatedProduct?.UtcCreatedDate.Should().Be(productToUpdate.UtcCreatedDate);

        _repository.GetEntitySet<Product>().Remove(productToUpdate);
        _repository.GetEntitySet<Category>().Remove(category);

        await _repository.SaveChangesAsync();
    }
}