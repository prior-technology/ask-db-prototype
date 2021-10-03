using OpenAI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AskDb.Library;

namespace AskDbConsole
{
    public class LibraryConsumer
    {
        public LibraryConsumer(OpenAIClient client)
        {
            Client = client;
        }

        public OpenAIClient Client { get; }
        
        public void IndexFile(FileInfo file)
        {
            var indexer = new FileIndexer(Client);
            indexer.BasePath = new DirectoryInfo("C:\\");
            var response = indexer.Index(file).GetAwaiter().GetResult();
            var responseJson = JsonSerializer.Serialize(response);
            Console.WriteLine(responseJson);
        }

        public void IndexFolder(DirectoryInfo folder, string name)
        {
            var indexer = new FileIndexer(Client);
            indexer.BasePath = new DirectoryInfo("C:\\Users\\StephenPrior\\source\\repos\\gimp-help\\html\\en");
            var response = indexer.Index(folder, name).GetAwaiter().GetResult();
            var responseJson = JsonSerializer.Serialize(response);
            Console.WriteLine(responseJson);
        }


    }
}
