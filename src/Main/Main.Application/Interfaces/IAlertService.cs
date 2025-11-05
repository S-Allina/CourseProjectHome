using Main.Application.Dtos.Common;

namespace Main.Application.Interfaces
{
    public interface IAlertService
    {
        void Success(string message);
        void Error(string message);
        void Warning(string message);
        AlertModelDto? GetAlert();
    }
}
