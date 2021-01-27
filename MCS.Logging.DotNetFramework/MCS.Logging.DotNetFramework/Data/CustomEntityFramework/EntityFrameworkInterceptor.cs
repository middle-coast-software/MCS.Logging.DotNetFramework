using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;

namespace MCS.Logging.DotNetFramework.Data.CustomEntityFramework
{
    public class EntityFrameworkInterceptor: IDbInterceptor
    {
        private Exception WrapEntityFrameworkException(DbCommand command, Exception ex)
        {
            var newException = new Exception("EntityFramework command failed!", ex);
            AddParamsToException(command.Parameters, newException);
            return newException;
        }

        private static void AddParamsToException(DbParameterCollection parameters, Exception exception)
        {
            foreach (DbParameter param in parameters)
            {
                exception.Data.Add(param.ParameterName, param.Value.ToString());
            }
        }
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            if (interceptionContext.Exception != null)
                interceptionContext.Exception = WrapEntityFrameworkException(command, interceptionContext.Exception);
        }
        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            if (interceptionContext.Exception != null)
                interceptionContext.Exception = WrapEntityFrameworkException(command, interceptionContext.Exception);
        }
        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            if (interceptionContext.Exception != null)
                interceptionContext.Exception = WrapEntityFrameworkException(command, interceptionContext.Exception);
        }


#pragma warning disable IDE0060 // Do not catch general exception types
        public static void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }
        public static void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
        }
        public static void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
        }
#pragma warning restore IDE0060 // Do not catch general exception types
    }
}
