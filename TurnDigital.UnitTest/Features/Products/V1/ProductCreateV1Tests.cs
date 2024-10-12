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

public class ProductCreateV1Tests
{
    private readonly IRepository _repository;

    private readonly Mock<IFileManager> _fileManager;

    private readonly IValidator<CreateProductV1> _validator;

    public ProductCreateV1Tests()
    {
        _fileManager = new Mock<IFileManager>();

        var dbContextOptions = new DbContextOptionsBuilder<TurnDigitalDbContext>()
            .UseInMemoryDatabase("TurnDigitalTestDb")
            .Options;

        var dbContext = new TurnDigitalDbContext(dbContextOptions);

        _repository = new Repository(dbContext);

        _validator = new CreateProductV1Validator();

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
    }

    [Fact]
    public async Task Handle_ShouldFailValidation()
    {
        // Arrange
        
        var category = _repository.GetEntitySet<Category>().Add(new Category { Name = "Category" }).Entity;

        await _repository.SaveChangesAsync();
        
        await using var memoryStream = new MemoryStream();
        var handler = new CreateProductV1Handler(_repository, _fileManager.Object, _validator);
        var command = new CreateProductV1(new FileModel(memoryStream, "image.xls", long.MaxValue), null, null, -1, category.Id);

        //Act
        var result = await handler.Handle(command, default);


        //Assert
        result.Case.Should().BeOfType<FailureResponse<ValidationFailureResponse>>()
            .And.Subject.As<FailureResponse<ValidationFailureResponse>>().ResponseData.As<ValidationFailureResponse>()
            .ValidationMap.Should().ContainKeys("Image", "Price", "Name");
        
        _repository.GetEntitySet<Category>().Remove(category);

        await _repository.SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ShouldCreateProduct()
    {
        // Arrange
        var category = _repository.GetEntitySet<Category>().Add(new Category { Name = "Category" }).Entity;

        await _repository.SaveChangesAsync();
        
        await using var memoryStream = new MemoryStream();
        var handler = new CreateProductV1Handler(_repository, _fileManager.Object, _validator);
        var command = new CreateProductV1(new FileModel(memoryStream, "image.webp", long.MaxValue), "Product 1", "Some Description", 500, category.Id);
        
        //Mock file manager
        _fileManager
            .Setup(manager => manager.SaveAsync(It.IsAny<FileModel>(),
                It.Is<string>(nameof(Product), StringComparer.OrdinalIgnoreCase)))
            .ReturnsAsync("product/image.webp");
        
        // Act
        var result = await handler.Handle(command, default);
        
        //Assert
        result.Case.Should().BeOfType<ProductDto>()
            .Subject.Category.Name.Should().Be(category.Name);
        
        result.Case.Should().BeOfType<ProductDto>()
            .Subject.Category.Id.Should().Be(category.Id);
        
        result.Case.Should().BeOfType<ProductDto>()
            .Subject.Name.Should().Be("Product 1");
        
        result.Case.Should().BeOfType<ProductDto>()
            .Subject.Description.Should().Be("Some Description");
        
        result.Case.Should().BeOfType<ProductDto>()
            .Subject.Price.Should().Be(500);
        
        result.Case.Should().BeOfType<ProductDto>()
            .Subject.Slug.Should().Be("Product-1");

        var product = await _repository.GetEntitySet<Product>()
            .ById(result.Case.Should().BeOfType<ProductDto>().Subject.Id).FirstOrDefaultAsync();

        product.Should().NotBeNull();
        product?.Name.Should().Be("Product 1");
        product?.Description.Should().Be("Some Description");
        product?.CategoryId.Should().Be(category.Id);
        product?.Slug.Should().Be("Product-1");
        product?.Price.Should().Be(500);

        _repository.GetEntitySet<Category>().Remove(category);

        await _repository.SaveChangesAsync();
    }
}