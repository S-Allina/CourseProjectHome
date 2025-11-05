namespace Main.Application.Interfaces
{
    public interface IUrlService
    {
        string GetAuthFrontUrl();
        string BuildFrontendUrl(string path);
    }
}
