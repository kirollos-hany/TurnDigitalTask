﻿using LanguageExt;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.IO;

namespace TurnDigital.Application.Features.Products.Commands.V2;

public record UpdateProductV2
(int Id, FileModel? Image, string? Name, string? Description,
    double? Price) : ICommand<Either<FailureResponse, ProductDto>>;