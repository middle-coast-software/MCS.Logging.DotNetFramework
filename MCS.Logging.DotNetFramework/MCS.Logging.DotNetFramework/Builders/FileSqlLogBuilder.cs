using MCS.Logging.DotNetFramework.Builders.Utility;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Logging.DotNetFramework.Builders
{
    public static class FileSqlLogBuilder
    {
        internal static void BuildLogger(ref Serilog.ILogger _perfLogger,ref Serilog.ILogger _usageLogger,ref Serilog.ILogger _errorLogger, ref Serilog.ILogger _diagnosticLogger, string logLocation, string connectionString, int batchPostingLimit = 1)
        {
            _perfLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{logLocation}\\perf-{DateTime.Now.ToString("MMddyyyy")}.txt")
               .WriteTo.MSSqlServer(connectionString, "PerfLogs", autoCreateSqlTable: true,
                    columnOptions: SqlColumns.GetSqlColumnOptions(), batchPostingLimit: batchPostingLimit)
               .CreateLogger();

            _usageLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{logLocation}\\usage-{DateTime.Now.ToString("MMddyyyy")}.txt")
                .WriteTo.MSSqlServer(connectionString, "UsageLogs", autoCreateSqlTable: true,
                    columnOptions: SqlColumns.GetSqlColumnOptions(), batchPostingLimit: batchPostingLimit)
                .CreateLogger();

            _errorLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{logLocation}\\error-{DateTime.Now.ToString("MMddyyyy")}.txt")
                .WriteTo.MSSqlServer(connectionString, "ErrorLogs", autoCreateSqlTable: true,
                    columnOptions: SqlColumns.GetSqlColumnOptions(), batchPostingLimit: batchPostingLimit)
                .CreateLogger();

            _diagnosticLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{logLocation}\\diagnostic-{DateTime.Now.ToString("MMddyyyy")}.txt")
                .WriteTo.MSSqlServer(connectionString, "DiagnosticLogs", autoCreateSqlTable: true,
                    columnOptions: SqlColumns.GetSqlColumnOptions(), batchPostingLimit: batchPostingLimit)
                .CreateLogger();
        }
    }
}
