using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace TrafficMonitorMobileService
{
    /// <summary></summary>
    public class ManualRouteEntity : StorageData
    {
        /// <summary>Alias for PartitionKey</summary>
        [IgnoreProperty]
        public string UserId {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        /// <summary></summary>
        public string EndPointsId { get; set; }

        /// <summary></summary>
        public DateTime StartTime { get; set; }

        /// <summary></summary>
        public DateTime EndTime { get; set; }
    }
}