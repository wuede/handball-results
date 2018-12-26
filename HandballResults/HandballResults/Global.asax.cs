using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HandballResults
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            System.Diagnostics.Trace.TraceError("uncaught exception: {0}", exception);
        }
    }
}
