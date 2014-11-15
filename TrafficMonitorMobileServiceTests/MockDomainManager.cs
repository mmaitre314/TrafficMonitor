using Microsoft.WindowsAzure.Mobile.Service.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using TrafficMonitorMobileService;

namespace TrafficMonitorMobileServiceTests
{
    class MockDomainManager : IDomainManager<ManualRouteEntity>
    {
        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ManualRouteEntity> InsertAsync(ManualRouteEntity data)
        {
            return Task.FromResult(data);
        }

        public SingleResult<ManualRouteEntity> Lookup(string id)
        {
            throw new NotImplementedException();
        }

        public Task<SingleResult<ManualRouteEntity>> LookupAsync(string id)
        {
            var list = new List<ManualRouteEntity>();

            if (id == "'DevelopmentUserId','RowKey'")
            {
                list.Add(new ManualRouteEntity
                {
                    PartitionKey = "DevelopmentUserId",
                    RowKey = "RowKey",
                    EndPointsId = "EndPointsId",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now
                });
            }

            return Task.FromResult(
                SingleResult<ManualRouteEntity>.Create(
                    list.AsQueryable<ManualRouteEntity>()
                ));
        }

        public IQueryable<ManualRouteEntity> Query()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ManualRouteEntity>> QueryAsync(ODataQueryOptions query)
        {
            var list = new List<ManualRouteEntity>();
            list.Add(new ManualRouteEntity
            {
                PartitionKey = "DevelopmentUserId",
                RowKey = "RowKey",
                EndPointsId = "EndPointsId",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            });

            return Task.FromResult((IEnumerable<ManualRouteEntity>)list);
        }

        public Task<ManualRouteEntity> ReplaceAsync(string id, ManualRouteEntity data)
        {
            throw new NotImplementedException();
        }

        public Task<ManualRouteEntity> UpdateAsync(string id, Delta<ManualRouteEntity> patch)
        {
            throw new NotImplementedException();
        }
    }
}
