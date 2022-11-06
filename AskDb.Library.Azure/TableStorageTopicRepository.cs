using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AskDb.Model;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AskDb.Library.Azure
{
    public class TableStorageTopicRepository : ITopicRepository
    {
        private const string TableName = "AskDbTopics";
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public TableStorageTopicRepository(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private Uri StorageUri => new Uri(_configuration["TableStorageUri"]);
        private string AccountName => _configuration["TableStorageAccount"];
        private string StorageAccountKey => _configuration["TableStorageKey"];

        private TableClient GetTableClient()
        {
            return new TableClient(
                    StorageUri,
                    TableName,
                    new TableSharedKeyCredential(AccountName, StorageAccountKey));
        }

        private TableServiceClient GetTableServiceClient()
        {
            return new TableServiceClient(
                    StorageUri,
                    new TableSharedKeyCredential(AccountName, StorageAccountKey));
        }
        public async Task<Topic> GetTopic(string uid, string topicKey)
        {
            if (topicKey == null)
            {
                return new Topic
                {
                    Description = "GNU Image Manipulation Program",
                    FileId = _configuration[$"OpenAI:GIMP:FileId"],
                    Key = "gimp"
                };            
            }

            var partitionKey = GetPartitionKey(uid);
            //lookup a topic from Azure table storage
            try
            {
                var tableClient = GetTableClient();
                var response = await tableClient.GetEntityAsync<TopicTableEntity>(partitionKey, topicKey);

                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to lookup TopicTableEntity {partitionKey} {topicKey}");
                return new Topic
                {
                    Description = "",
                    FileId = _configuration[$"OpenAI:{topicKey}:FileId"],
                    Key = topicKey
                };                
            }                         
        }

        public IEnumerable<Topic> GetTopics(string uid)
        {
            var startTime = DateTime.UtcNow;
            try
            {

                var serviceClient = GetTableServiceClient();

                //TableItem table = await serviceClient.CreateTableIfNotExistsAsync(TableName);
                var table = serviceClient.CreateTableIfNotExists(TableName);
                //throws NullReferenceException from https://github.com/Azure/azure-sdk-for-net/blob/9c9ec82c36580deff41b8940af420519a28cd4e1/sdk/core/Azure.Core/src/ResponseOfT.cs#L35
                //https://github.com/Azure/azure-sdk-for-net/issues/25434


            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Exception thrown while creating topic table");
                return DefaultTopics();
            }

            var topics = DefaultTopics().ToList();

            try
            {
                //_logger.LogInformation($"Have table {table.Name}.");
                var tableClient = GetTableClient();
                var partition = GetPartitionKey(uid);
                //using var response = await tableClient.AddEntityAsync(entity);
                var topicPages = tableClient.Query<TableEntity>(t => t.PartitionKey == partition );

                foreach (var topicPage in topicPages.AsPages())
                {
                    foreach (var topicContent in topicPage.Values)
                    {
                        var topic = new Topic
                        {
                            Key = topicContent.RowKey,
                            Description = topicContent.GetString("Description"),
                            FileId = topicContent.GetString("FileId")                           
                        };
                        topics.Add(topic);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Exception thrown while querying topics");
                return DefaultTopics();
            }
            if (topics.Count == 0) return DefaultTopics();
            return topics;
        }

        public void RemoveTopic(string uid, Topic topic)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Topic> DefaultTopics()
        {
            return new[] {
                    new Topic { Key="gimp", Description= "GNU Image Manipulation Program", FileId=_configuration["OpenAI:GIMP:FileId"] } ,
                    new Topic { Key= "git",Description= "GIT", FileId=_configuration["OpenAI:GIT:FileId"]},
                    new Topic { Key="upload", Description="Uploaded Document" }
                };
        }
        
        public async Task AddTopic(string uid, string topicKey, string contextDocument)
        {
            var tableClient = GetTableClient();
            var topicEntity = new TopicTableEntity
            {
                PartitionKey = GetPartitionKey(uid),
                RowKey = topicKey,
                Description = $"Topic {topicKey} {contextDocument.Substring(0, 20)}",
                FullText = contextDocument
            };

            var response = await tableClient.UpsertEntityAsync(topicEntity);

            if (response.IsError)
            {
                _logger.LogError("Error attempting to add {topicKey}: {status} {reason}", topicKey, response.Status, response.ReasonPhrase);
            }
        }
        
        private string GetPartitionKey(string uid)
        {
            return $"UID_{uid}";
        }
    }
}