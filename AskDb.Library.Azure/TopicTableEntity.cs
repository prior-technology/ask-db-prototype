using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using AskDb.Model;
using Azure;
using Azure.Data.Tables;

namespace AskDb.Library.Azure
{
    internal class TopicTableEntity : ITableEntity
    {
        [JsonIgnore]
        private Topic Topic { get; }
        public TopicTableEntity(Topic topic)
        {
            Topic = topic;
        }

        public TopicTableEntity()
        {
            Topic = new Topic();
        }
        public string PartitionKey { get; set; }

        public string Description
        {
            get => Topic.Description;
            set => Topic.Description = value;
        }
        public string FileId
        {
            get => Topic.FileId;
            set => Topic.FileId = value;
        }
            
        public string FullText
        {
            get
            {
                if (Topic.FullText == null) return null;
                return (Topic.FullText.Length > 31 * 1024) ? Topic.FullText.Substring(0, 31 * 1024) : Topic.FullText;
            }
            set => Topic.FullText = value;
        }
        
        public string Sections
        {
            get
            {
                return JsonSerializer.Serialize(Topic.Sections);
            }
            set
            {
                Topic.Sections = JsonSerializer.Deserialize<TopicSection[]>(value);
            }
        }

        public string RowKey
        {
            get => Topic.Key;
            set => Topic.Key = value;
        }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public Topic GetTopic() => Topic;
    }
}
