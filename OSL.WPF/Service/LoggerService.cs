/* Copyright 2020 Nicolas Mayeur

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    https://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using GalaSoft.MvvmLight.Ioc;
using OSL.Common.Service;
using System;

namespace OSL.WPF.Service
{
    public class LoggerService : ILoggerService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public enum LEVEL { DEBUG = 1, INFO = 2, WARN = 3, ERROR = 4, FATAL = 5 };

        private readonly LEVEL _Level;

        public LoggerService(LEVEL level)
        {
            _Level = level;
        }

        [PreferredConstructor]
        public LoggerService()
        {
            _Level = LEVEL.DEBUG;
        }

        public void Debug(string message)
        {
            Logger.Debug(message);
        }

        public void Error(string message)
        {
            Logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            Logger.Error(exception, message);
        }

        public void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            Logger.Fatal(exception, message);
        }

        public void Info(string message)
        {
            Logger.Info(message);
        }

        public void Warn(string message)
        {
            Logger.Warn(message);
        }
    }
}
