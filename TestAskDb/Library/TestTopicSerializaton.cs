using AskDb.Model;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace TestAskDb.Library
{
    public class TestTopicSerializaton
    {
        const string ExpectedTopicWithSectionsJson = "{\"Key\":\"K1\",\"Description\":\"TEST\",\"FileId\":null,\"FullText\":\"Longer Text\",\"Sections\":[{\"Id\":1,\"V\":\"zcyMP83MDEAzM1NA\"}]}";
        [Fact]
        public void TestSerializeTopic()
        {
            var topic = new Topic { Key="K1", Description = "TEST", FullText = "Longer Text" };
            string jsonString = JsonSerializer.Serialize(topic);
            jsonString.Should().BeEquivalentTo("{\"Key\":\"K1\",\"Description\":\"TEST\",\"FileId\":null,\"FullText\":\"Longer Text\",\"Sections\":[]}");
        }

        [Fact]
        public void TestSerializeTopicWithSections()
        {
            var topicSection = new TopicSection { EmbeddingVector = new float[] { 1.1F, 2.2F, 3.3F }, Id = 1 };
            var topic = new Topic { Key = "K1", Description = "TEST", FullText = "Longer Text", Sections = new TopicSection[] { topicSection } };
            string jsonString = JsonSerializer.Serialize(topic);
            jsonString.Should().BeEquivalentTo(ExpectedTopicWithSectionsJson);
        }

        [Fact]
        public void TestDeserializeTopicWithSections()
        {
            var json = ExpectedTopicWithSectionsJson;
            var topic = JsonSerializer.Deserialize<Topic>(json);
            topic.Key.Should().Be("K1");
            topic.Sections.Length.Should().Be(1);
            topic.Sections[0].EmbeddingVector[0].Should().Be(1.1F);

        }
    }
}
