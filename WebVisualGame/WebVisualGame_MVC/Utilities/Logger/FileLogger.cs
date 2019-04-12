using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace WebVisualGame_MVC.Utilities.Logger
{
    public class FileLogger : ILogger
    {
        private string Path { get; set; }

        private Func<string, LogLevel, bool> LogFilter { get; set; }

        private static readonly object _lock = new object();

        public FileLogger(string _path)
        {
            Path = _path;

            LogFilter = (category, logLevel) => true;
        }

        public FileLogger(string _path, Func<string, LogLevel, bool> _filter)
        {
            Path = _path;

            LogFilter = _filter;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return LogFilter("", logLevel);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private string GetTrace()
        {
            try
            {
                var stackTrace = new StackTrace(0);

                // first six frames on stack - logger functions
                var stackFrame = stackTrace.GetFrame(6);

                var method = stackFrame.GetMethod();

                return method.DeclaringType.FullName + "." + method.Name;
            }
            catch (Exception e)
            {
                return "UnknownTrace";
            }
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception ex,
            Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                string trace = GetTrace();

                bool shouldLog = LogFilter(trace, logLevel);

                if (shouldLog)
                {
                    lock (_lock)
                    {
                        try
                        {
                            string message = $"[{DateTime.Now.ToLongTimeString()}] {trace}: {formatter(state, ex)}{Environment.NewLine}";
                            File.AppendAllText(Path, message);
                        }
                        catch (Exception exception)
                        {
                            throw new Exception($"Seems like file '{Path}' cannot be opened now: " + exception.Message);
                        }
                    }
                }
            }
        }
    }
}
