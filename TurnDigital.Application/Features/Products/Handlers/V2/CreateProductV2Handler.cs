using FluentValidation;
using LanguageExt;
using Mapster;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Products.Commands.V2;
using TurnDigital.Application.Responses;
using TurnDigital.Application.Validations.Mapping;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Categories.Specifications;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Domain.IO.Interfaces;
using TurnDigital.Domain.Utilities;

namespace TurnDigital.Application.Features.Products.Handlers.V2;

internal sealed class CreateProductV2Handler : ICommandHandler<CreateProductV2, Either<FailureResponse, ProductDto>>
{
    private readonly IRepository _repository;

    private readonly IFileManager _fileManager;

    private readonly IValidator<CreateProductV2> _validator;

    public CreateProductV2Handler(IRepository repository, IFileManager fileManager, IValidator<CreateProductV2> validator)
    {
        _repository = repository;
        _fileManager = fileManager;
        _validator = validator;
    }

    public async Task<Either<FailureResponse, ProductDto>> Handle(CreateProductV2 request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResponse();
        }
        
        var imageId = await _fileManager.SaveAsync(request.Image, nameof(Product));

        var category = await _repository.GetEntitySet<Category>().ById(request.CategoryId)
            .FirstAsync(cancellationToken);

        var product = new Product
        {
            Category = category,
            Description = request.Description,
            Image = imageId,
            Name = request.Name,
            Price = request.Price,
            Slug = request.Name.Slugify()
        };

        _repository.GetEntitySet<Product>().Add(product);

        await _repository.SaveChangesAsync(cancellationToken);

        return product.Adapt<ProductDto>();
    }
}