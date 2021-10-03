using System.IO;
using AskDb.Library;
using Xunit;
using FluentAssertions;

namespace TestAskDb.Library
{
    public class TestFileParser
    {
        [Fact]
        public void TestParseFile()
        {
            //given a FileParser with an HTML document loaded
            var fileParser = new FileParser();
            
            //when a file is loade
            var text = fileParser.GetTextFromHtml(new FileInfo("gimp-first-steps.html"));

            //then the text is visible
            text.Should().Contain("This section provides a brief introduction", "it is the start of the html content");
        }
    }
}
