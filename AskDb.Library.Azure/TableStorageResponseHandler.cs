using System;
using System.IO;
using Azure;
using Microsoft.Extensions.Logging;

namespace AskDb.Library.Azure
{
    public class TableStorageResponseHandler
    {
        private readonly ILogger _logger;

        public TableStorageResponseHandler(ILogger<TableStorageResponseHandler> logger)
        {
            _logger = logger;
        }

        public void CheckResponse(Response response)
        {
            if (response.Status <= 299) return;
            _logger.LogWarning($"Failed to log answer - status: {response.Status}");
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                foreach (var responseHeader in response.Headers)
                {
                    _logger.LogDebug("Header:{0}:{1}", responseHeader.Name, responseHeader.Value);
                }
                var responseContent = GetResponseContent(response);
                _logger.LogDebug("Content: {0}", responseContent);
            }
        }

        private string GetResponseContent(Response response)
        {
            if (response.ContentStream == null) return "";
            try
            {
                var streamReader = new StreamReader(response.ContentStream);
                return streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                _logger.LogDebug(e, "Failed to read response content");
                return "";
            }
        }

    }
}