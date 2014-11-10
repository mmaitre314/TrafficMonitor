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
    class MockDomainManager : IDomainManager<ManualRoute>
    {
        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ManualRoute> InsertAsync(ManualRoute data)
        {
            return Task.FromResult(data);
        }

        public SingleResult<ManualRoute> Lookup(string id)
        {
            throw new NotImplementedException();
        }

        public Task<SingleResult<ManualRoute>> LookupAsync(string id)
        {
            var list = new List<ManualRoute>();

            if (id == "'PartitionKey','RowKey'")
            {
                list.Add(new ManualRoute
                {
                    UserId = "DevelopmentUserId",
                    PartitionKey = "PartitionKey",
                    RowKey = "RowKey",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now
                });
            }

            return Task.FromResult(
                SingleResult<ManualRoute>.Create(
                    list.AsQueryable<ManualRoute>()
                ));
        }

        public IQueryable<ManualRoute> Query()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ManualRoute>> QueryAsync(ODataQueryOptions query)
        {
            var list = new List<ManualRoute>();
            list.Add(new ManualRoute
            {
                UserId = "DevelopmentUserId",
                PartitionKey = "PartitionKey",
                RowKey = "RowKey",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            });

            return Task.FromResult((IEnumerable<ManualRoute>)list);
        }

        public Task<ManualRoute> ReplaceAsync(string id, ManualRoute data)
        {
            throw new NotImplementedException();
        }

        public Task<ManualRoute> UpdateAsync(string id, Delta<ManualRoute> patch)
        {
            throw new NotImplementedException();
        }
    }
}
