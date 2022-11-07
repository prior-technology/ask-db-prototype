
using AskDb.Library;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace TestAskDb.Library
{
    public class TestTopicManager
    {
        [Fact]
        public void TestSplitTopic()
        {
            //given a multiline string smaller than BlockSize
            var testString = @"This is a test string
With a couple of lines,   
    and some indented lines";

            //when the string is split into blocks
            var target = new TopicManager(null,null,null) { BlockSize = 100 };
            var blocks = target.SplitTopic(testString);

            //then the blocks are the same as the original string
            var firstBlock = blocks.First();
            firstBlock.Should().BeEquivalentTo("This is a test string With a couple of lines, and some indented lines");
        }

        [Fact]
        public void TestSplitTopicLongerThanBlockSize()
        {
            //given a multiline string smaller than BlockSize
            var testString = @"This is a test string
With a couple of lines,   
    and some indented lines";

            //when the string is split into blocks
            var target = new TopicManager(null, null, null) { BlockSize = 25 };
            var blocks = target.SplitTopic(testString).ToArray();

            //then the blocks are the same as the original string
            
            blocks[0].Should().BeEquivalentTo("This is a test string");
            blocks[1].Should().BeEquivalentTo("With a couple of lines,");
            blocks[2].Should().BeEquivalentTo("and some indented lines");
        }
    }
}
