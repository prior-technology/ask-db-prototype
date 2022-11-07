using System.Collections.Generic;

namespace AskDb.Model
{
    public class Topic
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public string FileId { get; set; }
        public string FullText { get; set; }
        public IList<TopicSection> Sections { get; set; } = new List<TopicSection>();
        
    }
}
