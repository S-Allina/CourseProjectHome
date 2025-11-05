using Main.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Main.Infrastructure.ImgBBStorage
{
    public class ImgBBStorageService : IImgBBStorageService
    {
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private const string ImageName = "image";
        private const string KeyName = "key";
        private const string ApiKey = "ImgBB:ApiKey";
        private const string ImgBBUrl = "ImgBB:Url";

        public ImgBBStorageService(IConfiguration configuration)
        {
            _apiKey = configuration[ApiKey]!;
            _apiUrl = configuration[ImgBBUrl]!;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty", nameof(file));

            try
            {
                byte[] fileBytes;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);

                    fileBytes = memoryStream.ToArray();
                }

                string base64Image = Convert.ToBase64String(fileBytes);

                using (HttpClient client = new HttpClient())
                {
                    var formData = new MultipartFormDataContent();

                    formData.Add(new StringContent(base64Image), ImageName);
                    formData.Add(new StringContent(_apiKey), KeyName);

                    HttpResponseMessage response = await client.PostAsync(_apiUrl, formData);

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    ImgBBResponse imgBBResponse = JsonConvert.DeserializeObject<ImgBBResponse>(responseBody);

                    if (imgBBResponse?.Success == true)
                    {
                        return imgBBResponse.Data.Url;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public record ImgBBResponse(Data Data, bool Success, int Status);

    public record Data(string Url, string DeleteUrl);
}
