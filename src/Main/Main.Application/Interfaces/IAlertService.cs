using Main.Application.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
