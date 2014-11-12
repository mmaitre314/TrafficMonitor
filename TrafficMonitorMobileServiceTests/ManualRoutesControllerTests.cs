using System;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http;
using System.Net.Http;
using System.Threading;
using System.Net;
using Owin;
using Microsoft.WindowsAzure.Mobile.Service;
using TrafficMonitorMobileService;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using System.Net.Http.Headers;

namespace TrafficMonitorMobileServiceTests
{
    [TestClass]
    public class ManualRoutesControllerTests
    {
        const string m_baseAddress = "http://localhost:31415/";
        static readonly HttpClient m_client = new HttpClient();
        static Func<HttpRequestMessage, ApiServices, IDomainManager<ManualRoute>> m_initialDomainManagerFactory;
        static IDisposable m_webAppDispose;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            // Start the Web App
            m_webAppDispose = WebApp.Start<MockWebApp>(url: m_baseAddress);
            
            // Cut the dependency on Azure Tables
            m_initialDomainManagerFactory = ManualRoutesController.CreateDomainManager;
            ManualRoutesController.CreateDomainManager = (request, services) =>
            {
                return new MockDomainManager();
            };
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // Re-enable dependency on Azure Tables
            ManualRoutesController.CreateDomainManager = m_initialDomainManagerFactory;
            m_initialDomainManagerFactory = null;

            // Shutdow the Web App
            m_webAppDispose.Dispose();
            m_webAppDispose = null;
        }

        [TestMethod]
        public void TestGetAllRows()
        {
            HttpResponseMessage response = m_client.GetAsync(m_baseAddress + "tables/ManualRoutes").Result;
            Console.WriteLine("HTTP response:\n{0}", response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("HTTP response content:\n{0}", content);

            List<ManualRoute> entities = JsonConvert.DeserializeObject<List<ManualRoute>>(content);

            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual("PartitionKey", entities[0].PartitionKey);
            Assert.AreEqual("RowKey", entities[0].RowKey);
            Assert.AreEqual("DevelopmentUserId", entities[0].UserId);
        }

        [TestMethod]
        public void TestGetOneRow()
        {
            HttpResponseMessage response = m_client.GetAsync(m_baseAddress + "tables/ManualRoutes/'PartitionKey','RowKey'").Result;
            Console.WriteLine("HTTP response:\n{0}", response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("HTTP response content:\n{0}", content);

            ManualRoute entity = JsonConvert.DeserializeObject<ManualRoute>(content);

            Assert.AreEqual("PartitionKey", entity.PartitionKey);
            Assert.AreEqual("RowKey", entity.RowKey);
            Assert.AreEqual("DevelopmentUserId", entity.UserId);
        }

        [TestMethod]
        public void TestGetOneRowNoMatch()
        {
            HttpResponseMessage response = m_client.GetAsync(m_baseAddress + "tables/ManualRoutes/'BadPartitionKey','BadRowKey'").Result;
            Console.WriteLine("HTTP response:\n{0}", response);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void TestPostOneRow()
        {
            HttpContent request = new StringContent(
                JsonConvert.SerializeObject(
                new ManualRoute
                {
                    PartitionKey = "PartitionKey",
                    RowKey = "RowKey",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now
                }));
            request.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = m_client.PostAsync(m_baseAddress + "tables/ManualRoutes", request).Result;
            Console.WriteLine("HTTP response:\n{0}", response);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            string content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("HTTP response content:\n{0}", content);

            ManualRoute entity = JsonConvert.DeserializeObject<ManualRoute>(content);

            Assert.AreEqual("PartitionKey", entity.PartitionKey);
            Assert.AreEqual("RowKey", entity.RowKey);
            Assert.AreEqual("DevelopmentUserId", entity.UserId);
        }
    }
}
