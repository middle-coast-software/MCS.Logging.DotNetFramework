using Serilog;
using Serilog.Events;
using System;
using System.Configuration;
using MCS.Logging.DotNetFramework.Models;
using System.Data.SqlClient;

namespace MCS.Logging.DotNetFramework
{
    public static class McsLogger
    {
        private static readonly ILogger _perfLogger;
        private static readonly ILogger _usageLogger;
        private static readonly ILogger _errorLogger;
        private static readonly ILogger _diagnosticLogger;

        static McsLogger()
        {
            string logDestinationType = ConfigurationManager.AppSettings["McsFileLogDestinationType"];
            
            //TODO: Add Switch Handler for other types of destinations once this is proven out

            string logFolderLocation = ConfigurationManager.AppSettings["LogFolderLocation"];
            _perfLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{logFolderLocation}\\perf.txt")
                .CreateLogger();

            _usageLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{logFolderLocation}\\usage.txt")
                .CreateLogger();

            _errorLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{logFolderLocation}\\error.txt")
                .CreateLogger();

            _diagnosticLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{logFolderLocation}\\diagnostic.txt")
                .CreateLogger();
        }

        public static void WritePerf(LogDetail infoToLog)
        {
            _perfLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
        }
        public static void WriteUsage(LogDetail infoToLog)
        {
            _usageLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
        }
        public static void WriteError(LogDetail infoToLog)
        {
            if (infoToLog.Exception != null)
            {
                var procName = FindProcName(infoToLog.Exception);
                infoToLog.Location = string.IsNullOrEmpty(procName) ? infoToLog.Location : procName;
                infoToLog.Message = GetMessageFromException(infoToLog.Exception);
            }
            _errorLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
        }
        public static void WriteDiagnostic(LogDetail infoToLog)
        {
            var writeDiagnostics = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableDiagnostics"]);
            if (!writeDiagnostics)
                return;

            _diagnosticLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
        }

        private static string GetMessageFromException(Exception ex)
        {
            if (ex.InnerException != null)
                return GetMessageFromException(ex.InnerException);

            return ex.Message;
        }

        private static string FindProcName(Exception ex)
        {
            if (ex is SqlException sqlEx)
            {
                var procName = sqlEx.Procedure;
                if (!string.IsNullOrEmpty(procName))
                    return procName;
            }

            if (!string.IsNullOrEmpty((string)ex.Data["Procedure"]))
            {
                return (string)ex.Data["Procedure"];
            }

            if (ex.InnerException != null)
                return FindProcName(ex.InnerException);

            return null;
        }
    }
}
