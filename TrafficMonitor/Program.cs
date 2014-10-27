using BingMapsRESTService.Common.JSON;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace TrafficMonitor
{
    //
    // Doc:
    //  - Microsoft Azure WebJobs   http://go.microsoft.com/fwlink/?LinkID=401557
    //  - Bing Maps                 http://msdn.microsoft.com/en-us/library/ff701717.aspx
    //
    // To add an entity to the EndPoints table:
    //
    //endPointTable.Execute(
    //  TableOperation.Insert(
    //    new EndPointEntity
    //    {
    //        PartitionKey = "0",
    //        RowKey = "HomeToWork",
    //        StartLatitude = 47.697571,
    //        StartLongitude = -122.0942,
    //        EndLatitude = 47.644361,
    //        EndLongitude = -122.141386
    //    }
    //  )
    //);
    //

    class Program
    {
        static readonly string m_routeBatchId = DateTime.UtcNow.Ticks.ToString("X");

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString
                );
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable endPointTable = tableClient.GetTableReference("EndPoints");
            CloudTable routeTable = tableClient.GetTableReference("Routes");

            string keyBingMaps = ((MiscEntity)tableClient.GetTableReference("Misc")
                .Execute(TableOperation.Retrieve<MiscEntity>("0", "BingMapsKey"))
                .Result).Value;

            WebClient client = new WebClient();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Response));

            foreach (EndPointEntity endPoint in endPointTable.ExecuteQuery(new TableQuery<EndPointEntity>()))
            {
                Route route = await GetRouteAsync(keyBingMaps, client, serializer, endPoint);
                if (route == null)
                {
                    continue;
                }

                StoreRoute(routeTable, route, endPoint.RowKey);
            }
        }

        private static async Task<Route> GetRouteAsync(
            string keyBingMaps, 
            WebClient client, 
            DataContractJsonSerializer serializer, 
            EndPointEntity endPoint
            )
        {
            Console.WriteLine("Querying route: {0}", endPoint.RowKey);

            string uri = String.Format(
                    "http://dev.virtualearth.net/REST/v1/Routes/Driving?o=json&key={0}&wp.1={1},{2}&wp.2={3},{4}&optmz=timeWithTraffic&rpo=Points&du=mi",
                    keyBingMaps,
                    endPoint.StartLatitude,
                    endPoint.StartLongitude,
                    endPoint.EndLatitude,
                    endPoint.EndLongitude
                );

            try
            {
                using (Stream stream = await client.OpenReadTaskAsync(uri))
                {
                    var response = (Response)serializer.ReadObject(stream);

                    if ((response.ResourceSets.Length > 0) && (response.ResourceSets[0].Resources.Length > 0))
                    {
                        return (Route)response.ResourceSets[0].Resources[0];
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }

            return null;
        }

        private static void StoreRoute(CloudTable routeTable, Route route, string endPointId)
        {
            // Serialize path (using WPF's Path Markup Syntax)
            var builder = new StringBuilder();
            foreach (double[] point in route.RoutePath.Line.Coordinates)
            {
                builder.AppendFormat("{0},{1} ", point[0], point[1]);
            }

            var routeEntity = new RouteEntity
            {
                PartitionKey = endPointId,
                RowKey = m_routeBatchId,
                Time = DateTime.Now,
                DurationUnit = route.DurationUnit,
                Duration = route.TravelDuration,
                Distance = route.TravelDistance,
                Path = builder.ToString()
            };

            routeTable.Execute(TableOperation.Insert(routeEntity));
        }
    }
}
