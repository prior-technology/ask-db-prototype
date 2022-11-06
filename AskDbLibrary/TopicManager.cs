using System;
using System.Collections.Generic;
using System.Text;

namespace AskDb.Library
{
    public class TopicManager
    {
        public int BlockSize { get; set; } = 4000;
        /// <summary>
        /// split fulltext into blocks of up to 4000 characters, replacing NewLine chars with spaces
        /// </summary>
        /// <param name="fullText"></param>
        /// <returns></returns>
        public IEnumerable<string> SplitTopic(string fullText)
        {
           
            var blocks = new List<string>();
            var block = new StringBuilder();
            var blockLength = 0;
            var lines = fullText.Split(new[] { Environment.NewLine }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            bool firstLine = true;
            foreach (var line in lines)
            {                
                if (blockLength + line.Length > BlockSize)
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
