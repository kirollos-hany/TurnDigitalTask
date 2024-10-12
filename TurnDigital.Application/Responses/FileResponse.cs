namespace TurnDigital.Application.Responses;

public class FileResponse
{
    public FileResponse(Stream file, string contentType)
    {
        File = file;
        ContentType = contentType;
    }
    
    public Stream File { get; }
    
    public string ContentType { get; }
}