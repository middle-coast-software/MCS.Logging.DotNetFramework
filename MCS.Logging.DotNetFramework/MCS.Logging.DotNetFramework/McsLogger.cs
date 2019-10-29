using Serilog;
using Serilog.Events;
using System;
using System.Configuration;
using MCS.Logging.DotNetFramework.Models;
using System.Data.SqlClient;
using MCS.Logging.DotNetFramework.Builders;

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
            string logDestinationType = ConfigurationManager.AppSettings["McsFileLogDestinationTypes"] ?? "";
            string logFolderLocation = ConfigurationManager.AppSettings["LogFolderLocation"] ?? ".\\Logs\\";
            string logConnectionString = ConfigurationManager.ConnectionStrings["LogConnection"].ConnectionString ?? "";
            string logBatchSize = ConfigurationManager.AppSettings["LogBatchSize"] ?? "1";

            if (!int.TryParse(logBatchSize, out var batchSize))
                batchSize = 1;

            var logToFile = logDestinationType.ToUpper().Contains("FILE");
            var logToSQL = logDestinationType.ToUpper().Contains("SQL");

            if (logToSQL && logToFile)
            {
                FileSqlLogBuilder.BuildLogger(ref _perfLogger, ref _usageLogger, ref _errorLogger, ref _diagnosticLogger, logFolderLocation, logConnectionString, batchSize);             
            }
            else if (logToFile)
                FileLogBuilder.BuildLogger(ref _perfLogger, ref _usageLogger, ref _errorLogger, ref _diagnosticLogger, logFolderLocation);
            else if (logToSQL)
            {                
                SqlLogBuilder.BuildLogger(ref _perfLogger, ref _usageLogger, ref _errorLogger, ref _diagnosticLogger, logConnectionString, batchSize);                
            }
           
        }

        public static void WritePerf(LogDetail infoToLog)
        {
            //_perfLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
            _perfLogger.Write(LogEventLevel.Information,
                       "{Timestamp}{Product}{Layer}{Location}{Message}{Hostname}{UserId}{UserName}{Exception}{ElapsedMilliseconds}{CorrelationId}{CustomException}{AdditionalInfo}",
                        infoToLog.Timestamp,
                        infoToLog.Product, infoToLog.Layer, infoToLog.Location, infoToLog.Message, infoToLog.Hostname,
                        infoToLog.UserId, infoToLog.UserName, infoToLog.Exception?.ToBetterString(),
                        infoToLog.ElapsedMilliseconds,
                        infoToLog.CorrelationId,
                        infoToLog.CustomException,
                        infoToLog.AdditionalInfo
                        );
        }
        public static void WriteUsage(LogDetail infoToLog)
        {
            //_usageLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
            _usageLogger.Write(LogEventLevel.Information,
                    "{Timestamp}{Product}{Layer}{Location}{Message}{Hostname}{UserId}{UserName}{Exception}{ElapsedMilliseconds}{CorrelationId}{CustomException}{AdditionalInfo}",
                        infoToLog.Timestamp,
                        infoToLog.Product, infoToLog.Layer, infoToLog.Location, infoToLog.Message, infoToLog.Hostname,
                        infoToLog.UserId, infoToLog.UserName, infoToLog.Exception?.ToBetterString(),
                        infoToLog.ElapsedMilliseconds,
                        infoToLog.CorrelationId,
                        infoToLog.CustomException,
                        infoToLog.AdditionalInfo
                        );
        }
        public static void WriteError(LogDetail infoToLog)
        {
            if (infoToLog.Exception != null)
            {
                var procName = FindProcName(infoToLog.Exception);
                infoToLog.Location = string.IsNullOrEmpty(procName) ? infoToLog.Location : procName;
                infoToLog.Message = GetMessageFromException(infoToLog.Exception);
            }
            //_errorLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
            _errorLogger.Write(LogEventLevel.Error,
                    "{Timestamp}{Product}{Layer}{Location}{Message}{Hostname}{UserId}{UserName}{Exception}{ElapsedMilliseconds}{CorrelationId}{CustomException}{AdditionalInfo}",
                    infoToLog.Timestamp,
                    infoToLog.Product, infoToLog.Layer, infoToLog.Location, infoToLog.Message, infoToLog.Hostname,
                    infoToLog.UserId, infoToLog.UserName, infoToLog.Exception?.ToBetterString(),
                    infoToLog.ElapsedMilliseconds,
                    infoToLog.CorrelationId,
                    infoToLog.CustomException,
                    infoToLog.AdditionalInfo
                    );


        }
        public static void WriteDiagnostic(LogDetail infoToLog)
        {
            var writeDiagnostics = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableDiagnostics"]);
            if (!writeDiagnostics)
                return;

            //_diagnosticLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
            _diagnosticLogger.Write(LogEventLevel.Information,
                   "{Timestamp}{Product}{Layer}{Location}{Message}{Hostname}{UserId}{UserName}{Exception}{ElapsedMilliseconds}{CorrelationId}{CustomException}{AdditionalInfo}",
                        infoToLog.Timestamp,
                        infoToLog.Product, infoToLog.Layer, infoToLog.Location, infoToLog.Message, infoToLog.Hostname,
                        infoToLog.UserId, infoToLog.UserName, infoToLog.Exception?.ToBetterString(),
                        infoToLog.ElapsedMilliseconds,
                        infoToLog.CorrelationId,
                        infoToLog.CustomException,
                        infoToLog.AdditionalInfo
                        );
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
