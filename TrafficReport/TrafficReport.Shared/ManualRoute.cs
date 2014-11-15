using System;

namespace TrafficReport
{
    /// <summary>
    /// Data Transfer Object (DTO) for entities in the ManualRoute table
    /// </summary>
    public class ManualRoute
    {
        /// <summary></summary>
        public string EndPointsId { get; set; }

        /// <summary></summary>
        public DateTime StartTime { get; set; }

        /// <summary></summary>
        public DateTime EndTime { get; set; }

        [Obsolete("Only here to keep MobileServiceTable.InsertAsync() happy")]
        public string Id { get; set; }
    }
}