using System;
using System.Threading.Tasks;

namespace GeoSports.Common.service
{
    public interface ILoggerService
    {
        void Debug(string message);
        Task DebugAsync(string message);

        void Info(string message);
        Task InfoAsync(string message);

        void Warn(string message);
        Task WarnAsync(string message);

        void Error(string message);
        Task ErrorAsync(string message);

        void Error(string message, Exception exception);
        Task ErrorAsync(string message, Exception exception);

        void Fatal(string message);
        Task FatalAsync(string message);

        void Fatal(string message, Exception exception);
        Task FatalAsync(string message, Exception exception);
    }
}
