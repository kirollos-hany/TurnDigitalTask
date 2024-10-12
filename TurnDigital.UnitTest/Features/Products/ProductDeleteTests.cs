using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using TurnDigital.Application.Features.Products.Commands;
using TurnDigital.Application.Features.Products.Handlers;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Domain.Features.Products.Specifications;
using TurnDigital.Domain.IO.Interfaces;
using TurnDigital.Infrastructure.DataAccess;

namespace TurnDigital.UnitTest.Features.Products;

public class ProductDeleteTests
{
    private readonly IRepository _repository;

    private readonly Mock<IFileManager> _fileManager;

    public ProductDeleteTests()
    {
        _fileManager = new Mock<IFileManager>();
        
        // Mock file manager
        _fileManager.Setup(manager => manager.DeleteAsync(It.IsAny<string>()));
        
        var dbContextOptions = new DbContextOptionsBuilder<TurnDigitalDbContext>()
            .UseInMemoryDatabase("TurnDigitalTestDb")
            .Options;

        var dbContext = new TurnDigitalDbContext(dbContextOptions);

        _repository = new Repository(dbContext);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound()
    {
        // Arrange 
        var category = _repository.GetEntitySet<Category>().Add(new Category { Name = "Category" }).Entity;

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

        var handler = new DeleteProductHandler(_repository, _fileManager.Object);
        var command = new DeleteProduct(int.MaxValue);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.Case.Should().BeOfType<FailureResponse<NotFoundResponse>>().Subject
            .ResponseData.Should().BeOfType<NotFoundResponse>();

        _repository.GetEntitySet<Product>().Remove(product);
        _repository.GetEntitySet<Category>().Remove(category);

        await _repository.SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ShouldDeleteProduct()
    {
        // Arrange 
        var category = _repository.GetEntitySet<Category>().Add(new Category { Name = "Category" }).Entity;

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

        var handler = new DeleteProductHandler(_repository, _fileManager.Object);
        var command = new DeleteProduct(product.Id);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.Case.Should().BeOfType<Unit>();

        var deletedProduct = await _repository.GetEntitySet<Product>().ById(product.Id).FirstOrDefaultAsync();

        deletedProduct.Should().BeNull();

        _repository.GetEntitySet<Category>().Remove(category);

        await _repository.SaveChangesAsync();
    }
}