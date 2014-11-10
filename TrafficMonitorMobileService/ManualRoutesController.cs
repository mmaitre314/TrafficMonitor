using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using System.Configuration;
using System.Web.Http.OData.Builder;
using System.Collections.Generic;
using System.Web.Http.OData.Query;
using System.Web.Http.Hosting;
using Microsoft.Data.Edm;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using System.Net.Http;

namespace TrafficMonitorMobileService
{
    // Doc on routing HTTP requests (method+uri) to C# class+method: 
    //  http://www.asp.net/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api
    //  http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2

    /// <summary></summary>
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class ManualRoutesController : TableController<ManualRouteEntity>
    {
        /// <summary>
        /// IDomainManager factory enabling Dependency Inversion in tests
        /// </summary>
        public static Func<HttpRequestMessage, ApiServices, IDomainManager<ManualRouteEntity>> CreateDomainManager = (request, services) =>
            {
                return new StorageDomainManager<ManualRouteEntity>(
                                "AzureWebJobsStorage",
                                "ManualRoutes",
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
        /// Read all rows from the ManualRoutes Azure Table
        /// </summary>
        [HttpGet, Route("tables/ManualRoutes")]
        public async Task<IEnumerable<ManualRouteEntity>> GetAllRowsAsync()
        {
            ODataModelBuilder modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<ManualRouteEntity>("ManualRoutes");
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
            return entities.Where(entity => entity.UserId == userId);
        }

        /// <summary>
        /// Read one row from the ManualRoutes Azure Table
        /// </summary>
        /// <param name="id">'PartitionKey','RowKey'</param>
        [HttpGet, Route("tables/ManualRoutes/{id}")]
        public async Task<SingleResult<ManualRouteEntity>> GetRowAsync(string id)
        {
            SingleResult<ManualRouteEntity> result = await DomainManager.LookupAsync(id);

            string userId = GetUserId();
            return new SingleResult<ManualRouteEntity>(
                result.Queryable.Where(entity => entity.UserId == userId)
                );
        }

        /// <summary>
        /// Create one row in the ManualRoutes Azure Table
        /// </summary>
        /// <param name="item">Row content</param>
        [HttpPost, Route("tables/ManualRoutes")]
        public async Task<IHttpActionResult> PostRowAsync(ManualRouteEntity item)
        {
            // Set the current user
            item.UserId = GetUserId();

            ManualRouteEntity current = await InsertAsync(item);
            return CreatedAtRoute("tables", new { controller = "ManualRoutes", id = current.Id }, current);
        }

        private string GetUserId()
        {
            string userId = Configuration.GetIsHosted() ? ((ServiceUser)User).Id : "DevelopmentUserId";
            return userId;
        }
    }
}