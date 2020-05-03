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
            if (_Level <= LEVEL.DEBUG) Console.WriteLine(message);
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

        public void Info(string message)
        {
            if (_Level <= LEVEL.INFO)
            {
                Console.WriteLine(message);
            }
            throw new NotImplementedException();
        }

        public void Warn(string message)
        {
            if (_Level <= LEVEL.WARN)
            {
                Console.WriteLine(message);
            }
        }
    }
}
