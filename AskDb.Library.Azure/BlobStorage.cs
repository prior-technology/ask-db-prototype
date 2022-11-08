using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AskDb.Library.Azure
{
    /// <summary>
    /// Stores document fragments in Azure Blob Storage
    /// </summary>
    public class BlobDocumentStorage : IDocumentStorage
    {
        private const string ContainerName = "askdb";
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public BlobDocumentStorage(IConfiguration configuration, ILogger<BlobDocumentStorage> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private Uri StorageUri => new Uri(_configuration["BlobStorageUri"]);
        private string AccountName => _configuration["TableStorageAccount"];
        private string StorageAccountKey => _configuration["TableStorageKey"];

        private BlobServiceClient GetBlobServiceClient()
        {
            return new BlobServiceClient(
                    StorageUri,
                    new StorageSharedKeyCredential(AccountName, StorageAccountKey));
        }

        private BlobContainerClient GetBlobContainerClient()
        {
            return GetBlobServiceClient().GetBlobContainerClient(ContainerName);
        }

        public async Task<string> GetBlob(string blobName)
        {
            var blobClient = GetBlobContainerClient().GetBlobClient(blobName);
            var downloadInfo = await blobClient.DownloadAsync();
            var stream = downloadInfo.Value.Content;
            var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        public async Task<string> GetBlob(string uid, string topicKey)
        {
            var blobName = GetBlobName(uid, topicKey);
            return await GetBlob(blobName);
        }

        public async Task SaveBlob(string uid, string topicKey, string text)
        {
            try
            {
                var blobName = GetBlobName(uid, topicKey);
                var blobClient = GetBlobContainerClient().GetBlobClient(blobName);
                await blobClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(text)));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving {topicKey} for user {uid}", topicKey, uid);
                throw;
            }
                
            
        }

        private string GetBlobName(string uid, string topicKey)
        {
            return $"{uid}/{topicKey}.txt";
        }
    }
}
