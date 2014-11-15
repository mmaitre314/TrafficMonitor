using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;

namespace TrafficMonitorMobileService
{
    /// <summary>Class required by Azure Mobile Services</summary>
    public static class WebApiConfig
    {
        /// <summary>Method called by Azure Mobile Services</summary>
        public static HttpConfiguration Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            return config;
        }
    }
}

