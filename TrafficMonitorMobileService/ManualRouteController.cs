using Microsoft.Data.Edm;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.OData;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Query;

namespace TrafficMonitorMobileService
{
    // Doc on routing HTTP requests (method+uri) to C# class+method: 
    //  http://www.asp.net/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api
    //  http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2

    // Doc on authentication and authorization:
    //  http://www.asp.net/web-api/overview/security/external-authentication-services
    //  http://azure.microsoft.com/en-us/documentation/articles/mobile-services-dotnet-backend-windows-universal-dotnet-get-started-users/
    //  http://azure.microsoft.com/en-us/documentation/articles/mobile-services-dotnet-backend-windows-store-dotnet-authorize-users-in-scripts/
    //  http://blogs.msdn.com/b/carlosfigueira/archive/2012/10/23/troubleshooting-authentication-issues-in-azure-mobile-services.aspx 

    // To test Microsoft Account sign-on:
    //  https://trafficmonitormobileservice.azure-mobile.net/login/microsoftaccount

    // Note: classes exposing Web APIs must end with a "Controller" suffix
    /// <summary></summary>
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class ManualRouteController : TableController<ManualRouteEntity>
    {
        /// <summary>
        /// IDomainManager factory enabling Dependency Inversion in tests
        /// </summary>
        public static Func<HttpRequestMessage, ApiServices, IDomainManager<ManualRouteEntity>> CreateDomainManager = (request, services) =>
            {
                return new StorageDomainManager<ManualRouteEntity>(
                                "AzureWebJobsStorage",
                                "ManualRoute",
                                request,
                                services
                                );
            };

        /// <summary></summary>
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            // Doc on StorageDomainManager:
            //  http://wp.sjkp.dk/azure-mobile-service-net-backend-using-azure-table-storage/
            //  https://github.com/sjkp/AzureMobileServiceTableStorage

            base.Initialize(controllerContext);
            DomainManager = CreateDomainManager(Request, Services);
        }

        /// <summary>
        /// Read all entities from the ManualRoute table
        /// </summary>
        [HttpGet, Route("tables/ManualRoute")]
        public async Task<IEnumerable<ManualRoute>> ReadAllEntitiesAsync()
        {
            ODataModelBuilder modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<ManualRouteEntity>("ManualRouteEntity");
            IEdmModel edmModel = modelBuilder.GetEdmModel();

            // StorageDomainManager.QueryAsync() requires MS_EdmModel
            var actionDescriptor = Request.Properties[HttpPropertyKeys.HttpActionDescriptorKey] as HttpActionDescriptor;
            if (actionDescriptor != null)
            {
                actionDescriptor.Properties["MS_EdmModel" + typeof(ManualRouteEntity).FullName] = edmModel;
            }

            var options = new ODataQueryOptions(
                new ODataQueryContext(
                    edmModel,
                    typeof(ManualRouteEntity)
                ),
                Request
            );

            IEnumerable<ManualRouteEntity> entities = await DomainManager.QueryAsync(options);

            string userId = GetUserId();
            return entities
                .Where(entity => entity.UserId == userId)
                .Select(entity => new ManualRoute
                {
                    EndPointsId = entity.EndPointsId,
                    StartTime = entity.StartTime,
                    EndTime = entity.EndTime
                });
        }

        /// <summary>
        /// Read one entity from the ManualRoute table
        /// </summary>
        [HttpGet, Route("tables/ManualRoute/{id}")]
        public async Task<SingleResult<ManualRoute>> ReadEntityAsync(string id)
        {
            string userId = GetUserId();

            SingleResult<ManualRouteEntity> result = await DomainManager.LookupAsync(
                String.Format("'{0}','{1}'", userId, id)
                );

            return new SingleResult<ManualRoute>(
                result.Queryable
                .Where(entity => entity.UserId == userId)
                .Select(entity => new ManualRoute
                {
                    EndPointsId = entity.EndPointsId,
                    StartTime = entity.StartTime,
                    EndTime = entity.EndTime
                }));
        }

        /// <summary>
        /// Create one entity in the ManualRoute table
        /// </summary>
        /// <param name="item">Row content</param>
        [HttpPost, Route("tables/ManualRoute")]
        public async Task<IHttpActionResult> CreateEntityAsync(ManualRoute item)
        {
            string userId = GetUserId();
            var entity = new ManualRouteEntity
            {
                PartitionKey = userId,
                RowKey = item.EndPointsId + "@" + item.StartTime.ToBinary(),
                UserId = userId,
                EndPointsId = item.EndPointsId,
                StartTime = item.StartTime,
                EndTime = item.EndTime
            };

            entity = await InsertAsync(entity);

            return CreatedAtRoute("tables", 
                new 
                {
                    controller = "ManualRoute", 
                    id = entity.RowKey 
                }, 
                new 
                { 
                    id = entity.RowKey 
                });
        }

        private string GetUserId()
        {
            string userId = Configuration.GetIsHosted() ? ((ServiceUser)User).Id : "DevelopmentUserId";
            return userId;
        }
    }
}