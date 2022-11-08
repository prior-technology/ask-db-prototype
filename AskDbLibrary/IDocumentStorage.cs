using System.Threading.Tasks;

namespace AskDb.Library
{
    public interface IDocumentStorage
    {
        Task<string> GetBlob(string blobName);
        Task<string> GetBlob(string uid, string topicKey);
        Task SaveBlob(string uid, string topicKey, string text);
    }
}
