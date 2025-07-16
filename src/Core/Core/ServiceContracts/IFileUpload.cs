using Microsoft.AspNetCore.Http;

namespace Core.ServiceContracts;

public interface IFileUpload
{
    Task<string> UploadImage(string uploadFolder, string rootPath, string fileName, IFormFile file);
}
