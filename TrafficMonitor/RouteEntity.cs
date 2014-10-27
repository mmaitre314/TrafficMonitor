using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficMonitor
{
    class RouteEntity : TableEntity
    {
        public RouteEntity()
        {
        }

        public DateTime Time { get; set; }
        public string DurationUnit { get; set; }
        public double Duration { get; set; }
        public double Distance { get; set; }
        public string Path { get; set; }
    }
}
