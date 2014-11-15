using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using TrafficMonitorMobileService;

namespace TrafficMonitorMobileServiceTests
{
    public class MockWebApp
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = WebApiConfig.Register();
            appBuilder.UseWebApi(config);
        }
    }
}
