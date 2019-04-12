using Microsoft.Extensions.Logging;
using System;

namespace WebVisualGame_MVC.Utilities.Logger
{
    public static class FileLoggerExtensions
    {
        public static ILoggerFactory AddFile(this ILoggerFactory factory, string _path)
        {
            factory.AddProvider(new FileLoggerProvider(_path));
            return factory;
        }

        public static ILoggerFactory AddFile(this ILoggerFactory factory, string _path, Func<string, LogLevel, bool> _filter)
        {
            factory.AddProvider(new FileLoggerProvider(_path, _filter));
            return factory;
        }
    }
}
