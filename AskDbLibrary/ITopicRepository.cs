using AskDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskDb.Library
{
    public interface ITopicRepository
    {
        IEnumerable<Topic> GetTopics();
        Task<string> GetFileIdForTopic(string topic);
        void RemoveTopic(Topic topic);
        Task AddTopic(string topicKey, string contextDocument);
    }
}
