using Microsoft.AspNetCore.Http;

namespace Main.Application.Interfaces.ImgBBStorage
{
    public interface IImgBBStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
