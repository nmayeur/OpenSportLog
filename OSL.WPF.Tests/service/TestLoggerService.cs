using GeoSports.Common.service;
using System;
using System.Threading.Tasks;

namespace GeoSports.Common.Tests.service
{
    public class TestLoggerService : ILoggerService
    {

        public enum LEVEL { DEBUG = 1, INFO = 2, WARN = 3, ERROR = 4, FATAL = 5 };

        private LEVEL _Level;

        public TestLoggerService(LEVEL level)
        {
            _Level = level;
        }

        public void Debug(string message)
        {
            if (_Level <= LEVEL.DEBUG) Console.WriteLine(message);
        }

        public async Task DebugAsync(string message)
        {
            await Task.Run(() => Debug(message));
        }

        public void Error(string message)
        {
            if (_Level <= LEVEL.ERROR) Console.WriteLine(message);
        }

        public void Error(string message, Exception exception)
        {
            if (_Level <= LEVEL.ERROR)
            {
                Console.WriteLine(message);
                Console.WriteLine(exception.StackTrace);
            }
        }

        public async Task ErrorAsync(string message)
        {
            await Task.Run(() => Error(message));
        }

        public async Task ErrorAsync(string message, Exception exception)
        {
            await Task.Run(() => Error(message, exception));
        }

        public void Fatal(string message)
        {
            if (_Level <= LEVEL.FATAL)
            {
                Console.WriteLine(message);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            if (_Level <= LEVEL.FATAL)
            {
                Console.WriteLine(message);
                Console.WriteLine(exception.StackTrace);
            }
        }

        public async Task FatalAsync(string message)
        {
            await Task.Run(() => Fatal(message));
        }

        public async Task FatalAsync(string message, Exception exception)
        {
            await Task.Run(() => Fatal(message, exception));
        }

        public void Info(string message)
        {
            if (_Level <= LEVEL.INFO)
            {
                Console.WriteLine(message);
            }
            throw new NotImplementedException();
        }

        public async Task InfoAsync(string message)
        {
            await Task.Run(() => Info(message));
        }

        public void Warn(string message)
        {
            if (_Level <= LEVEL.WARN)
            {
                Console.WriteLine(message);
            }
        }

        public async Task WarnAsync(string message)
        {
            await Task.Run(() => Warn(message));
        }
    }
}
