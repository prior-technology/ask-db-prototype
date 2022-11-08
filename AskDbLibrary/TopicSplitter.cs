using System;
using System.Collections.Generic;
using System.Text;

namespace AskDb.Library
{
    public static class TopicSplitter
    {
        const int DefaultSectionLength= 4000;
        /// <summary>
        /// split fulltext into blocks of up to 4000 characters, replacing NewLine chars with spaces
        /// </summary>
        /// <param name="fullText"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitTopic(this string fullText, int sectionLength= DefaultSectionLength)
        {
            var blocks = new List<string>();
            var block = new StringBuilder();
            var blockLength = 0;
            var lines = fullText.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            bool firstLine = true;
            foreach (var line in lines)
            {
                if (blockLength + line.Length > sectionLength)
                {
                    blocks.Add(block.ToString());
                    block.Clear();
                    blockLength = 0;
                    firstLine = true;
                }
                if (firstLine)
                {
                    firstLine = false;
                }
                else
                {
                    block.Append(' ');
                    blockLength++;
                }
                block.Append(line);
                blockLength += line.Length;
            }
            //append the final block
            blocks.Add(block.ToString());
            return blocks;
        }
    }
}
