using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AskDb.Library.Azure
{
    public class TableStorageQuestionLogger : IQuestionLogger
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly TableStorageResponseHandler _tableStorageResponseHandler;
        private readonly string _userSid;

        public TableStorageQuestionLogger(IConfiguration configuration, ILoggerFactory loggerFactory, TableStorageResponseHandler tableStorageResponseHandler, ClaimsPrincipal user)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<TableStorageQuestionLogger>();
            _loggerFactory = loggerFactory;
            _tableStorageResponseHandler = tableStorageResponseHandler;
            _userSid = GetUserSid(user);
        }

        public async Task<IAnswerLogger> LogQuestion(string question)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var tableName = "AskDbQuestions";
                var storageUri = new Uri(_configuration["TableStorageUri"]);
                var accountName = _configuration["TableStorageAccount"];
                var storageAccountKey = _configuration["TableStorageKey"];

                //var serviceClient = new TableServiceClient(
                //    storageUri,
                //    new TableSharedKeyCredential(accountName, storageAccountKey));
                //TableItem table = await serviceClient.CreateTableIfNotExistsAsync(tableName);
                //TableItem table = serviceClient.CreateTableIfNotExists(tableName);
                //throws NullReferenceException from https://github.com/Azure/azure-sdk-for-net/blob/9c9ec82c36580deff41b8940af420519a28cd4e1/sdk/core/Azure.Core/src/ResponseOfT.cs#L35
                //https://github.com/Azure/azure-sdk-for-net/issues/25434

                //_logger.LogInformation($"Have table {table.Name}.");
                var tableClient = new TableClient(
                    storageUri,
                    tableName,
                    new TableSharedKeyCredential(accountName, storageAccountKey));
                var partition = $"UID_{_userSid}";
                var rowKey = startTime.ToString("O");
                var entity = new TableEntity(partition, rowKey)
                {
                    {"Started", startTime.ToUniversalTime()},
                    {"Question", question},
                };
                _logger.LogDebug("Created entity");
                //using var response = await tableClient.AddEntityAsync(entity);
                using var response = await tableClient.UpsertEntityAsync(entity);
                _logger.LogDebug($"Entity added, response code {response.Status}");

                _tableStorageResponseHandler.CheckResponse(response);

                return new TableStorageAnswerLogger(_configuration, _loggerFactory, _tableStorageResponseHandler, partition, rowKey, startTime, question);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Exception thrown while logging question");
                return new StubAnswerLogger(question);
            }
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
            var fakeSid = Guid.NewGuid().ToString();
            _logger.LogDebug($"Using fake sid {fakeSid}");
            return fakeSid;
        }
    }
}