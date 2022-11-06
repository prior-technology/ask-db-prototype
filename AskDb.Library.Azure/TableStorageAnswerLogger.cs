using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AskDb.Library.Azure
{
    public class TableStorageAnswerLogger : IAnswerLogger
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly string _partition;
        private readonly DateTime _startTime;
        private readonly string _rowKey;
        private readonly string _question;
        private readonly TableStorageResponseHandler _tableStorageResponseHandler;

        public TableStorageAnswerLogger(IConfiguration configuration, ILoggerFactory loggerFactory, TableStorageResponseHandler tableStorageResponseHandler, string partition, string rowKey, DateTime startTime, string question)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger< TableStorageAnswerLogger>();
            _partition = partition;
            _startTime = startTime;
            _rowKey = rowKey;
            _question = question;
            _tableStorageResponseHandler = tableStorageResponseHandler;
        }
 
        public async Task LogAnswer(string answer)
        {
            try
            {
                var endTime = DateTime.UtcNow;
                string tableName = "AskDbQuestions";
                var storageUri = new Uri(_configuration["TableStorageUri"]);
                var accountName = _configuration["TableStorageAccount"];
                var storageAccountKey = _configuration["TableStorageKey"];
          
                var tableClient = new TableClient(
                    storageUri,
                    tableName,
                    new TableSharedKeyCredential(accountName, storageAccountKey));

                var entity = new TableEntity(_partition, _rowKey)
                {
                    {"Question", _question},
                    {"Answer", answer},
                    {"Started", _startTime},
                    {"Ended", endTime},
                };
                _logger.LogDebug("Created entity");
                var response = await tableClient.UpsertEntityAsync(entity);
                _logger.LogDebug($"Entity upserted, response code {response.Status}");
                _tableStorageResponseHandler.CheckResponse(response);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Exception thrown while logging answer");
            }
        }
    }
}
