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
        
        public void IndexFile(FileInfo file, FileInfo outfile = null)
        {
            var indexer = new FileIndexer(Client, outfile);
            indexer.BasePath = new DirectoryInfo("C:\\");
            var response = indexer.Index(file).GetAwaiter().GetResult();
            if (response==null)
            {
                Console.WriteLine("Couldn't get file content");
                return;
            }
            var responseJson = JsonSerializer.Serialize(response);
            Console.WriteLine(responseJson);
        }

        public void IndexFolder(DirectoryInfo folder, string name, FileInfo outfile=null)
        {
            var indexer = new FileIndexer(Client, outfile);
            indexer.BasePath = new DirectoryInfo("repos\\gimp-help\\html\\en");
            var response = indexer.Index(folder, name).GetAwaiter().GetResult();
            var responseJson = JsonSerializer.Serialize(response);
            Console.WriteLine(responseJson);
        }

        public async Task AskQuestion(string question, string fileId)
        {

            var inquisitor = new QuestionAsker(Client);

            var results = await inquisitor.AskQuestion(question, fileId);
            
            foreach (var answerList in results)
            {
                Console.WriteLine("========== Next Result ==========");
                foreach (var answer in answerList)
                {
                    Console.WriteLine(answer);
                }
            }
        }

    }
}
