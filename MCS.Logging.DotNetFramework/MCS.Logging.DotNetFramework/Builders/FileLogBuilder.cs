using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Logging.DotNetFramework.Builders
{
    public static class FileLogBuilder
    {
        internal static void BuildLogger(ref Serilog.ILogger _perfLogger, ref Serilog.ILogger _usageLogger, ref Serilog.ILogger _errorLogger, ref Serilog.ILogger _diagnosticLogger, string logLocation)
        {
            _perfLogger = new LoggerConfiguration()
               .WriteTo.File(path: $"{logLocation}\\perf-{DateTime.Now.ToString("MMddyyyy")}.txt")
               .CreateLogger();

            _usageLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{logLocation}\\usage-{DateTime.Now.ToString("MMddyyyy")}.txt")
                .CreateLogger();

            _errorLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{logLocation}\\error-{DateTime.Now.ToString("MMddyyyy")}.txt")
                .CreateLogger();

            _diagnosticLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{logLocation}\\diagnostic-{DateTime.Now.ToString("MMddyyyy")}.txt")
                .CreateLogger();
        }
    }
}
