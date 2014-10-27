using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficMonitor
{
    class MiscEntity : TableEntity
    {
        public MiscEntity()
        {
        }

        public string Value { get; set; }
    }
}
