using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MCS.Logging.DotNetFramework.Web.Attributes
{
    public sealed class ApiLoggerAttribute : ActionFilterAttribute
    {
        private readonly string _productName;

        public ApiLoggerAttribute(string productName)
        {
            _productName = productName;
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var dict = Helpers.GetWebLoggingData(out string userId, out string userName, out string location);

            actionContext.HttpContext.Items["PerfTracker"] = new PerfTracker(location,
                userId, userName, location, _productName, "API", dict);
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            try
            {
                if (actionExecutedContext.HttpContext.Items["PerfTracker"] is PerfTracker perfTracker)
                    perfTracker.Stop();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                // ignoring logging exceptions -- don't want an API call to fail 
                // if we run into logging problems. 
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
