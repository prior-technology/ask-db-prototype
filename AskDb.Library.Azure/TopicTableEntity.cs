using System;
using AskDb.Model;
using Azure;
using Azure.Data.Tables;

namespace AskDb.Library.Azure
{
    internal class TopicTableEntity : Topic, ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey
        {
            get { return Key; } 
            set { Key = value; } 
        }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
