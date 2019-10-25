using MCS.Logging.DotNetFramework.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http.ExceptionHandling;

namespace MCS.Logging.DotNetFramework.Web.Services
{
    public class CustomApiExceptionLogger : ExceptionLogger
    {
        private string _productName;
        public CustomApiExceptionLogger(string productName)
        {
            _productName = productName;
        }
        public override void Log(ExceptionLoggerContext context)
        {
            var dict = new Dictionary<string, object>();

            string userId, userName;
            var user = context.RequestContext.Principal as ClaimsPrincipal;
            Helpers.GetUserData(dict, user, out userId, out userName);

            string location;
            Helpers.GetLocationForApiCall(context.RequestContext, dict, out location);

            var errorId = Guid.NewGuid().ToString();
            // This is here because the Logger is called BEFORE the Handler in the 
            //Web API exception pipeline   
            context.Exception.Data.Add("ErrorId", errorId);

            var logEntry = new LogDetail()
            {
                CorrelationId = errorId,
                Product = _productName,
                Layer = "API",
                Location = location,
                Hostname = Environment.MachineName,
                Exception = context.Exception,
                UserId = userId,
                UserName = userName,
                AdditionalInfo = dict
            };
            McsLogger.WriteError(logEntry);
        }
    }
}
