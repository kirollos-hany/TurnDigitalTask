using FluentValidation;
using LanguageExt;
using Mapster;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Products.Commands.V1;
using TurnDigital.Application.Responses;
using TurnDigital.Application.Validations.Mapping;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Domain.Features.Products.Specifications;
using TurnDigital.Domain.IO.Interfaces;
using TurnDigital.Domain.Utilities;

namespace TurnDigital.Application.Features.Products.Handlers.V1;

internal sealed class UpdateProductV1Handler : ICommandHandler<UpdateProductV1, Either<FailureResponse, ProductDto>>
{
    private readonly IRepository _repository;

    private readonly IFileManager _fileManager;

    private readonly IValidator<UpdateProductV1> _validator;

    public UpdateProductV1Handler(IRepository repository, IFileManager fileManager,
        IValidator<UpdateProductV1> validator)
    {
        _repository = repository;
        _fileManager = fileManager;
        _validator = validator;
    }

    public async Task<Either<FailureResponse, ProductDto>> Handle(UpdateProductV1 request,
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

        product.Price = request.Price;

        product.Description = request.Description;

        product.Name = request.Name;

        product.Slug = request.Name.Slugify();

        await _repository.SaveChangesAsync(cancellationToken);

        return product.Adapt<ProductDto>();
    }
}