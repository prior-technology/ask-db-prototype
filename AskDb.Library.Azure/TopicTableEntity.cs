using AskDb.Model;
using Azure.Data.Tables;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

namespace AskDb.Library.Azure
{
    public class TopicTableEntity 
    {
        const string Description = "Description";
        const string FileId = "FileId";
        const string Key = "Key";

        private Topic Topic { get; }
        public TopicTableEntity(Topic topic)
        {
            Topic = topic;
        }
        public TopicTableEntity(TableEntity tableEntity)
        {
            Topic = new Topic
            {
                Description = tableEntity.GetString(Description),
                FileId = tableEntity.GetString(FileId),
                Key = tableEntity.GetString(Key)
            };
            var sectionKeys = tableEntity.Keys.Where(k => k.StartsWith("section_"));
            if (!sectionKeys.Any())
            {
                return;
            }
            Topic.Sections = new TopicSection[sectionKeys.Count()];
            for (var i = 0; i < Topic.Sections.Length; i++)
            {
                Topic.Sections[i] = new TopicSection { Id = i, EncodedEmbeddingVector = tableEntity.GetString($"section_{i}") };
            }
        }

        public TableEntity GetTableEntity()
        {
            var tableEntity = new TableEntity(PartitionKey, Topic.Key);
            tableEntity.Add(Description, Topic.Description);
            tableEntity.Add(FileId, Topic.FileId);
            tableEntity.Add(Key, Topic.Key);
            for(var i=0; i<Topic.Sections.Length; i++)
            {
                var section = Topic.Sections[i];
                tableEntity.Add($"section_{i}", section.EncodedEmbeddingVector);
            }
            return tableEntity;
        }
        public string PartitionKey { get; set; }
        public Topic GetTopic() => Topic;
    }
}
