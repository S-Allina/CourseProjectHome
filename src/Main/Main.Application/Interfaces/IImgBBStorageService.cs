using Microsoft.AspNetCore.Http;

namespace Main.Application.Interfaces
{
    public interface IImgBBStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
