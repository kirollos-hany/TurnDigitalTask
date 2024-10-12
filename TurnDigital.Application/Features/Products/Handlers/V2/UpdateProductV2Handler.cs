using FluentValidation;
using LanguageExt;
using Mapster;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Products.Commands.V2;
using TurnDigital.Application.Responses;
using TurnDigital.Application.Validations.Mapping;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Domain.Features.Products.Specifications;
using TurnDigital.Domain.IO.Interfaces;
using TurnDigital.Domain.Utilities;

namespace TurnDigital.Application.Features.Products.Handlers.V2;

internal sealed class UpdateProductV2Handler : ICommandHandler<UpdateProductV2, Either<FailureResponse, ProductDto>>
{
    private readonly IFileManager _fileManager;

    private readonly IRepository _repository;

    private readonly IValidator<UpdateProductV2> _validator;

    public UpdateProductV2Handler(IRepository repository, IFileManager fileManager,
        IValidator<UpdateProductV2> validator)
    {
        _fileManager = fileManager;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Either<FailureResponse, ProductDto>> Handle(UpdateProductV2 request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResponse();
        }
        var product = await _repository.GetEntitySet<Product>().WithCategory().ById(request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return FailureResponse<NotFoundResponse>.NotFound(new NotFoundResponse(nameof(Product),
                Constants.ValidationMessages.ProductMessages.ProductNotFound));
        }

        if (request.Image is not null)
        {
            if (product.Image != Product.DefaultImage)
            {
                await _fileManager.DeleteAsync(product.Image);
            }
            
            var imageId = await _fileManager.SaveAsync(request.Image, nameof(Product));

            product.Image = imageId;
        }

        if (request.Name is not null && product.Name != request.Name)
        {
            product.Name = request.Name;

            product.Slug = request.Name.Slugify();
        }

        if (request.Price is not null && product.Price != request.Price!.Value)
        {
            product.Price = request.Price.Value;
        }

        if (request.Description is not null && request.Description != product.Description)
        {
            product.Description = request.Description;
        }

        await _repository.SaveChangesAsync(cancellationToken);

        return product.Adapt<ProductDto>();
    }
}