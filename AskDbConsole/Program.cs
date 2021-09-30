using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using AskDb.Library;
using OpenAI;

namespace AskDbConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Setup command line arguments
            var askCommand = new Command("ask", "Send a question to the question answering API");
            askCommand.AddArgument(new Argument<string>("question"));

            var indexCommand = new Command("index", "Parse an html file, or folder containing html files, sending content to OpenAPI");
            indexCommand.AddArgument(new Argument<FileInfo>("file"));
            //indexCommand.AddArgument(new Argument<DirectoryInfo>("folder"));

            var testCommand = new Command("test", "Test");

            var rootCommand = new RootCommand
            {
                askCommand, indexCommand, testCommand
            };

            var client = new OpenAIClient();
            var consumer = new LibraryConsumer(client);

            var inquisitor = new QuestionAsker(client);
            askCommand.Handler = CommandHandler.Create<string>(inquisitor.AskQuestion);
            indexCommand.Handler = CommandHandler.Create<FileInfo>(consumer.IndexFile);
            //indexCommand.Handler = CommandHandler.Create<DirectoryInfo>(consumer.IndexFolder);
            testCommand.Handler = CommandHandler.Create(DoTest);
            var result = rootCommand.Invoke(args);
            Console.ReadLine();
            Console.WriteLine("fin");
            

        }

        static void DoTest()
        {
            var q = new QuickTest();
            q.DoTest();
        }
    }
}
