namespace TurnDigital.Domain.IO.Interfaces;

public interface IFileManager
{
  Task<string> SaveAsync(FileModel file, string fileDirectory = "");
  Task DeleteAsync(string fileId);
  Task<string> GetUrlAsync(string fileId);
  string GetContentType(string fileName);
}