using AskDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskDb.Library
{
    public interface ITopicRepository
    {
        IEnumerable<Topic> GetTopics(string uid);
        Task<Topic> GetTopic(string uid, string topicKey);
        void RemoveTopic(string uid, Topic topic);
        Task AddTopic(string uid, string topicKey, string contextDocument);
    }
}
