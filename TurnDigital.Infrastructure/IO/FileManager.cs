using Microsoft.AspNetCore.Hosting;
using TurnDigital.Domain.IO;
using TurnDigital.Domain.IO.Interfaces;
using TurnDigital.Domain.Web.Interfaces;

namespace TurnDigital.Infrastructure.IO;
internal sealed class FileManager : IFileManager
{
    private readonly IContentTypeProvider _contentTypeProvider;

    private readonly IBaseUrlProvider _baseUrlProvider;

    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileManager(IContentTypeProvider contentTypeProvider, IBaseUrlProvider baseUrlProvider, IWebHostEnvironment webHostEnvironment)
    {
        _contentTypeProvider = contentTypeProvider;
        _baseUrlProvider = baseUrlProvider;
        _webHostEnvironment = webHostEnvironment;
    }
    
    public async Task<string> SaveAsync(FileModel file, string fileDirectory = "")
    {
        var rootDirectory = _webHostEnvironment.WebRootPath;

        var fileExtension = new FileInfo(file.Name).Extension;

        var fileId = Guid.NewGuid() + fileExtension;

        var completePath = Path.Combine(rootDirectory, fileDirectory, fileId);
        
        var directoryPath = Path.Combine(rootDirectory, fileDirectory);
        
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        await using var fileStream = new FileStream(completePath, FileMode.OpenOrCreate);

        await using var memoryStream = new MemoryStream();

        await file.Stream.CopyToAsync(memoryStream);

        await fileStream.WriteAsync(memoryStream.ToArray());

        return (fileDirectory + "/" + fileId).ToLower();
    }

    public Task DeleteAsync(string fileId)
    {
        var rootDirectory = _webHostEnvironment.WebRootPath;

        var filePath = Path.Combine(rootDirectory, fileId);

        if (!string.IsNullOrEmpty(fileId) && Path.Exists(filePath))
        {
            File.Delete(filePath);
        }
        
        return Task.CompletedTask;
    }

    public Task<string> GetUrlAsync(string fileId)
    {
        return Task.FromResult((_baseUrlProvider.GetBaseUrl() + "/" + fileId).ToLower());
    }

    public string GetContentType(string fileName)
    {
        if (!_contentTypeProvider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }
}