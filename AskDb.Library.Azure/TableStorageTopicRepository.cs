using System;
using System.Collections.Generic;
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
        private readonly string _userSid;

        public TableStorageTopicRepository(IConfiguration configuration, ILogger logger, ClaimsPrincipal user)
        {
            _configuration = configuration;
            _logger = logger;
            _userSid = GetUserSid(user);
        }

        private Uri StorageUri => new Uri(_configuration["TableStorageUri"]);
        private string AccountName => _configuration["TableStorageAccount"];
        private string StorageAccountKey => _configuration["TableStorageKey"];
        private string PartitionKey => $"UID_{_userSid}";
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
        public async Task<string> GetFileIdForTopic(string topicKey)
        {
            if (topicKey == null)
            {
                return _configuration[$"OpenAI:GIMP:FileId"];
            }

            //lookup a topic from Azure table storage
            try
            {
                var tableClient = GetTableClient();
                var response = await tableClient.GetEntityAsync<TopicTableEntity>(PartitionKey, topicKey);

                var topic = response.Value;

                return topic.FileId;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to lookup TopicTableEntity {PartitionKey} {topicKey}");
                return _configuration[$"OpenAI:{topicKey}:FileId"];
            }                         
        }

        public IEnumerable<Topic> GetTopics()
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

            var topics = new List<Topic>();

            try
            {
                //_logger.LogInformation($"Have table {table.Name}.");
                var tableClient = GetTableClient();
                var partition = $"UID_{_userSid}";
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

        public void RemoveTopic(Topic topic)
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
        private string GetUserSid(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                _logger.LogDebug("claimsPrincipal null");
                return UseFakeSid();
            }

            var sid = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            
            if (sid == null)
            {
                _logger.LogDebug($"No claim of type {ClaimTypes.NameIdentifier}");
                return UseFakeSid();
            }

            _logger.LogDebug($"Using userSid {sid.Value}");
            return sid.Value;
        }

        private string UseFakeSid()
        {
            var fakeSid = "U" + (Guid.NewGuid().ToString());
            _logger.LogDebug($"Using fake sid {fakeSid}");
            return fakeSid;
        }

        public async Task AddTopic(string topicKey, string contextDocument)
        {
            var tableClient = GetTableClient();
            var topicEntity = new TopicTableEntity
            {
                PartitionKey = $"UID_{_userSid}",
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
    }
}