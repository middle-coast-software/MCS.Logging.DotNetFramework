using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Logging.DotNetFramework.Web.Attributes
{
    public class ApiLoggerAttribute : ActionFilterAttribute
    {
        private string _productName;

        public ApiLoggerAttribute(string productName)
        {
            _productName = productName;
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var dict = new Dictionary<string, object>();

            string userId, userName;
            var user = actionContext.HttpContext.User;
            Helpers.GetUserData(dict, user, out userId, out userName);

            string location = actionContext.ActionDescriptor.AttributeRouteInfo.Template;

            actionContext.ActionDescriptor.Properties["PerfTracker"] = new PerfTracker(location,
                userId, userName, location, _productName, "API", dict);
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            try
            {
                var perfTracker = actionExecutedContext.ActionDescriptor.Properties["PerfTracker"]
                    as PerfTracker;

                if (perfTracker != null)
                    perfTracker.Stop();
            }
            catch (Exception)
            {
                // ignoring logging exceptions -- don't want an API call to fail 
                // if we run into logging problems. 
            }
        }
    }
}
