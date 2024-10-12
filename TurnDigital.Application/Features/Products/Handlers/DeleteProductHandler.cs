using LanguageExt;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Products.Commands;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Domain.Features.Products.Specifications;
using TurnDigital.Domain.IO.Interfaces;
using Unit = MediatR.Unit;

namespace TurnDigital.Application.Features.Products.Handlers;

internal sealed class DeleteProductHandler : ICommandHandler<DeleteProduct, Either<FailureResponse, Unit>>
{
    private readonly IRepository _repository;

    private readonly IFileManager _fileManager;

    public DeleteProductHandler(IRepository repository, IFileManager fileManager)
    {
        _repository = repository;
        _fileManager = fileManager;
    }

    public async Task<Either<FailureResponse, Unit>> Handle(DeleteProduct request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetEntitySet<Product>().ById(request.Id).FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return FailureResponse<NotFoundResponse>.NotFound(new NotFoundResponse(nameof(Product),
                Constants.ValidationMessages.ProductMessages.ProductNotFound));
        }

        if (product.Image != Product.DefaultImage)
        {
            await _fileManager.DeleteAsync(product.Image);
        }
        
        _repository.GetEntitySet<Product>().Remove(product);

        await _repository.SaveChangesAsync(cancellationToken);

        return new Unit();
    }
}