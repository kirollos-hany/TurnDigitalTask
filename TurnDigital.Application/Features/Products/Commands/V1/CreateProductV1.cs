using LanguageExt;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.IO;

namespace TurnDigital.Application.Features.Products.Commands.V1;

public record CreateProductV1
(FileModel Image, string Name, string? Description, double Price,
    int CategoryId) : ICommand<Either<FailureResponse, ProductDto>>;