using Microsoft.Extensions.Logging;
using System;

namespace WebVisualGame_MVC.Utilities.Logger
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private string Path { get; set; }

        private Func<string, LogLevel, bool> LogFilter { get; set; }

        public FileLoggerProvider(string _path)
        {
            Path = _path;
            LogFilter = (category, logFilter) => true;
        }

        public FileLoggerProvider(string _path, Func<string, LogLevel, bool> _logFilter)
        {
            Path = _path;
            LogFilter = _logFilter;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(Path, LogFilter);
        }

        public void Dispose()
        {

        }
    }
}
