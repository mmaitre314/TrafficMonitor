using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security.Providers;
using System.Web.Http;
using System.Web.Routing;

namespace TrafficMonitorMobileService
{
    /// <summary></summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary></summary>
        protected void Application_Start()
        {
            WebApiConfig.Register();
        }
    }
}