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
        static Func<HttpRequestMessage, ApiServices, IDomainManager<ManualRouteEntity>> m_initialDomainManagerFactory;
        static IDisposable m_webAppDispose;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            // Start the Web App
            m_webAppDispose = WebApp.Start<MockWebApp>(url: m_baseAddress);
            
            // Cut the dependency on Azure Tables
            m_initialDomainManagerFactory = ManualRouteController.CreateDomainManager;
            ManualRouteController.CreateDomainManager = (request, services) =>
            {
                return new MockDomainManager();
            };
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // Re-enable dependency on Azure Tables
            ManualRouteController.CreateDomainManager = m_initialDomainManagerFactory;
            m_initialDomainManagerFactory = null;

            // Shutdow the Web App
            m_webAppDispose.Dispose();
            m_webAppDispose = null;
        }

        [TestMethod]
        public void TestGetAllRows()
        {
            HttpResponseMessage response = m_client.GetAsync(m_baseAddress + "tables/ManualRoute").Result;
            Console.WriteLine("HTTP response:\n{0}", response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("HTTP response content:\n{0}", content);

            List<ManualRoute> entities = JsonConvert.DeserializeObject<List<ManualRoute>>(content);

            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual("EndPointsId", entities[0].EndPointsId);
        }

        [TestMethod]
        public void TestGetOneRow()
        {
            HttpResponseMessage response = m_client.GetAsync(m_baseAddress + "tables/ManualRoute/RowKey").Result;
            Console.WriteLine("HTTP response:\n{0}", response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("HTTP response content:\n{0}", content);

            ManualRoute entity = JsonConvert.DeserializeObject<ManualRoute>(content);

            Assert.AreEqual("EndPointsId", entity.EndPointsId);
        }

        [TestMethod]
        public void TestGetOneRowNoMatch()
        {
            HttpResponseMessage response = m_client.GetAsync(m_baseAddress + "tables/ManualRoute/BadId").Result;
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
                    EndPointsId = "EndPointsId",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now
                }));
            request.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = m_client.PostAsync(m_baseAddress + "tables/ManualRoute", request).Result;
            Console.WriteLine("HTTP response:\n{0}", response);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            string content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("HTTP response content:\n{0}", content);

            string id = JsonConvert.DeserializeAnonymousType(content, new { id = "" }).id;

            Assert.IsTrue(id.Contains("EndPointsId@"));
        }
    }
}
