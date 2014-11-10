using Microsoft.WindowsAzure.Mobile.Service;
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
            Configure();
        }

        /// <summary></summary>
        public static HttpConfiguration Configure()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            return config;
        }
    }
}