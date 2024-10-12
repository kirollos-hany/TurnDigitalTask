using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Features.Products.Handlers;
using TurnDigital.Application.Features.Products.Queries;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Infrastructure.DataAccess;
using TurnDigital.Infrastructure.DataAccess.Seed;

namespace TurnDigital.UnitTest.Features.Products;

public class GetProductsTests
{
    private readonly IReadRepository _readRepository;

    private readonly IRepository _repository;

    public GetProductsTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<TurnDigitalDbContext>()
            .UseInMemoryDatabase("TurnDigitalTestDb")
            .Options;

        var dbContext = new TurnDigitalDbContext(dbContextOptions);

        _readRepository = new ReadRepository(dbContext);

        _repository = new Repository(dbContext);
    }

    [Fact]
    public async Task GetById_Handle_ShouldReturnNotFound()
    {
        // Arrange

        var query = new GetProductById(int.MaxValue);
        var handler = new GetProductByIdHandler(_readRepository);

        // Act
        var result = await handler.Handle(query, default);


        // Assert
        result.Case.Should().BeOfType(typeof(FailureResponse<NotFoundResponse>)).And.Subject
            .As<FailureResponse<NotFoundResponse>>().ResponseData.Should().BeOfType(typeof(NotFoundResponse));
    }

    [Fact]
    public async Task GetById_Handle_ShouldReturnProduct()
    {
        // Arrange
        var category = _repository.GetEntitySet<Category>().Add(new Category
        {
            Name = "category"
        }).Entity;

        var product = _repository.GetEntitySet<Product>().Add(new Product
        {
            Image = "product/default.png",
            Category = category,
            Description = null,
            Name = "Product",
            Price = 500,
            Slug = "Product"
        }).Entity;

        await _repository.SaveChangesAsync();

        var query = new GetProductById(product.Id);
        var handler = new GetProductByIdHandler(_readRepository);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.Case.Should().BeOfType<ProductDto>();

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Category.Id.Should().Be(product.CategoryId);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Name.Should().Be(product.Name);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Description.Should().Be(product.Description);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Price.Should().Be(product.Price);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Slug.Should().Be(product.Slug);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .UtcCreatedDate.Should().Be(product.UtcCreatedDate);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Image.Should().Be(product.Image);
        
        _repository.GetEntitySet<Product>().Remove(product);
        _repository.GetEntitySet<Category>().Remove(category);

        await _repository.SaveChangesAsync();
    }
    
    [Fact]
    public async Task GetBySlug_Handle_ShouldReturnNotFound()
    {
        // Arrange
        await _repository.SeedFakeData();

        var query = new GetProductBySlug("some random slug");
        var handler = new GetProductBySlugHandler(_readRepository);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.Case.Should().BeOfType(typeof(FailureResponse<NotFoundResponse>)).And.Subject
            .As<FailureResponse<NotFoundResponse>>().ResponseData.Should().BeOfType(typeof(NotFoundResponse));
    }

    [Fact]
    public async Task GetBySlug_Handle_ShouldReturnProduct()
    {
        // Arrange
        var category = _repository.GetEntitySet<Category>().Add(new Category
        {
            Name = "category"
        }).Entity;

        var product = _repository.GetEntitySet<Product>().Add(new Product
        {
            Image = "product/default.png",
            Category = category,
            Description = null,
            Name = "Product",
            Price = 500,
            Slug = "Product"
        }).Entity;

        await _repository.SaveChangesAsync();

        var query = new GetProductBySlug(product.Slug);
        var handler = new GetProductBySlugHandler(_readRepository);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.Case.Should().BeOfType<ProductDto>();

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Category.Id.Should().Be(product.CategoryId);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Name.Should().Be(product.Name);
        
        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Id.Should().Be(product.Id);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Description.Should().Be(product.Description);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Price.Should().Be(product.Price);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Slug.Should().Be(product.Slug);

        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .UtcCreatedDate.Should().Be(product.UtcCreatedDate);
        
        result.Case.Should().BeOfType<ProductDto>()
            .Subject
            .Image.Should().Be(product.Image);

        _repository.GetEntitySet<Product>().Remove(product);
        _repository.GetEntitySet<Category>().Remove(category);

        await _repository.SaveChangesAsync();
    }
}