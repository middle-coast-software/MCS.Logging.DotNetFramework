using System;
using System.Web;
using System.Web.UI;

namespace MCS.Logging.DotNetFramework.Web
{
    public abstract class PerfLoggedPage : System.Web.UI.Page
    {
        protected PerfTracker Tracker;

        protected override void OnLoad(EventArgs e)
        {
            var name = Page.Request.Path + (IsPostBack ? "_POSTBACK" : "");

            var data = Helpers.GetWebLoggingData(out string userId, out string userName, out string location);

            Tracker = new PerfTracker(name, userId, userName, location, System.Web.Hosting.HostingEnvironment.ApplicationHost.GetSiteName(), "WebForms", data);
            
            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            if (Tracker != null)
                Tracker.Stop();
        }
    }
}
