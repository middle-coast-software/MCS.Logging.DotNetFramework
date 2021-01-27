using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MCS.Logging.DotNetFramework.Web.Attributes
{
    public sealed class TrackPerformanceAttribute : ActionFilterAttribute
    {
        private readonly string _productName;
        private readonly string _layerName;

        // can use like [TrackPerformance("ToDos", "Mvc")]
        public TrackPerformanceAttribute(string product, string layer)
        {
            _productName = product;
            _layerName = layer;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var dict = Helpers.GetWebLoggingData(out string userId, out string userName, out string location);

            var type = filterContext.HttpContext.Request.HttpMethod;
            var perfName = filterContext.ActionDescriptor.ActionName + "_" + type;

            var stopwatch = new PerfTracker(perfName, userId, userName, location,
                _productName, _layerName, dict);
            filterContext.HttpContext.Items["Stopwatch"] = stopwatch;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var stopwatch = (PerfTracker)filterContext.HttpContext.Items["Stopwatch"];
            if (stopwatch != null)
                stopwatch.Stop();
        }
    }
}
