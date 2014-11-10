// From https://github.com/sjkp/AzureMobileServiceTableStorage/blob/master/MobileService1/DataObjects/StorageData.cs

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace TrafficMonitorMobileService
{
    /// <summary></summary>
    [JsonObject]
    public abstract class StorageData : TableEntity, ITableData
    {
        #region DisableSerializationOfAzureTableProperties

        /// <summary></summary>
        public new string PartitionKey
        {
            get { return base.PartitionKey; }
            set { base.PartitionKey = value; }
        }

        /// <summary></summary>
        public new string RowKey
        {
            get { return base.RowKey; }
            set { base.RowKey = value; }
        }

        /// <summary></summary>
        public new string ETag
        {
            get { return base.ETag; }
            set { base.ETag = value; }
        }

        /// <summary></summary>
        public new DateTimeOffset Timestamp
        {
            get { return base.Timestamp; }
            set { base.Timestamp = value; }
        }

        #endregion

        /// <summary></summary>
        [IgnoreProperty]
        public string Id
        {
            get
            {
                return ((object)new CompositeTableKey(new string[2] { this.PartitionKey, this.RowKey })).ToString();
            }
            set
            {
                CompositeTableKey compositeTableKey;
                if (!CompositeTableKey.TryParse(value, out compositeTableKey) || compositeTableKey.Segments.Count != 2)
                {
                    this.PartitionKey = value;
                    this.RowKey = value;
                }
                else
                {
                    this.PartitionKey = compositeTableKey.Segments[0];
                    this.RowKey = compositeTableKey.Segments[1];
                }
            }
        }

        /// <summary></summary>
        [JsonIgnore]
        [IgnoreProperty]
        public System.DateTimeOffset? UpdatedAt
        {
            get
            {
                return new DateTimeOffset?(this.Timestamp);
            }
            set { this.Timestamp = value.HasValue ? value.Value : DateTimeOffset.UtcNow; }
        }

        /// <summary></summary>
        [JsonIgnore]
        [IgnoreProperty]
        public byte[] Version
        {
            get
            {
                if (this.ETag == null)
                    return (byte[])null;
                else
                    return Encoding.UTF8.GetBytes(this.ETag);
            }
            set
            {
                this.ETag = value != null ? Encoding.UTF8.GetString(value) : (string)null;
            }
        }

        /// <summary></summary>
        [JsonIgnore]
        public System.DateTimeOffset? CreatedAt { get; set; }

        /// <summary></summary>
        [JsonIgnore]
        public bool Deleted { get; set; }

        /// <summary></summary>
        public bool ShouldSerializeTimestamp()
        {
            return false;
        }

        /// <summary></summary>
        public bool ShouldSerializePartitionKey()
        {
            return false;
        }

        /// <summary></summary>
        public bool ShouldSerializeRowKey()
        {
            return false;
        }

        /// <summary></summary>
        public bool ShouldSerializeETag()
        {
            return false;
        }
    }
}
