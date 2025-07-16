using Microsoft.AspNetCore.Http;
using Shared.Utils;

namespace Core.Services;

public class FileUploadService : IFileUpload
{
    public async Task<string> UploadImage(string uploadFolder,string rootPath, string fileName,IFormFile file)
    {
        var pathBuilder = new PathBuilder();
        pathBuilder.AddRootPath(rootPath)
            .AddRootDirectory(uploadFolder);


        Directory.CreateDirectory(pathBuilder.ToString());

        var pathToUpload = pathBuilder.AddFileName(fileName)
            .ToString(); 

        using (var FileStream = new FileStream(pathToUpload, FileMode.Create))
        {
            await file.CopyToAsync(FileStream);
        };

        // Return the full URL to the uploaded file
        pathBuilder.AddScheme().AddHost(); // add scheme and host to the path
        return pathBuilder.ToString(); // e.g. "https://example.com/images/2023/10/filename.jpg"

    }
}
