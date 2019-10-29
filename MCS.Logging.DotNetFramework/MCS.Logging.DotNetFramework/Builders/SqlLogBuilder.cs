using MCS.Logging.DotNetFramework.Builders.Utility;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Logging.DotNetFramework.Builders
{
    public static class SqlLogBuilder
    {
        internal static void BuildLogger(ref Serilog.ILogger _perfLogger, ref Serilog.ILogger _usageLogger, ref Serilog.ILogger _errorLogger, ref Serilog.ILogger _diagnosticLogger, string connectionString, int batchPostingLimit = 1)
        {
            _perfLogger = new LoggerConfiguration()
               .WriteTo.MSSqlServer(connectionString, "PerfLogs", autoCreateSqlTable: true,
                    columnOptions: SqlColumns.GetSqlColumnOptions(), batchPostingLimit: batchPostingLimit)
               .CreateLogger();

            _usageLogger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(connectionString, "UsageLogs", autoCreateSqlTable: true,
                    columnOptions: SqlColumns.GetSqlColumnOptions(), batchPostingLimit: batchPostingLimit)
                .CreateLogger();

            _errorLogger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(connectionString, "ErrorLogs", autoCreateSqlTable: true,
                    columnOptions: SqlColumns.GetSqlColumnOptions(), batchPostingLimit: batchPostingLimit)
                .CreateLogger();

            _diagnosticLogger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(connectionString, "DiagnosticLogs", autoCreateSqlTable: true,
                    columnOptions: SqlColumns.GetSqlColumnOptions(), batchPostingLimit: batchPostingLimit)
                .CreateLogger();
        }        
    }
}
