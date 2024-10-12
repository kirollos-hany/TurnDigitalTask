using Mapster;
using TurnDigital.Domain.IO;

namespace TurnDigital.Web.Mappers;

public class FormFileMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<IFormFile, FileModel>()
            .MapWith(formFile => new FileModel(formFile.OpenReadStream(), formFile.FileName, formFile.Length));
        config.NewConfig<IFormFile?, FileModel?>()
            .MapWith(formFile => formFile == null ? null : new FileModel(formFile.OpenReadStream(), formFile.FileName, formFile.Length));
    }
}