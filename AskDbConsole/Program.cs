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
            askCommand.AddArgument(new Argument<string>("file-id"));

            var addCommand = new Command("add", "Parse an html file sending content to OpenAPI");
            addCommand.AddArgument(new Argument<FileInfo>("file"));
            addCommand.AddOption(new Option<FileInfo>("--outfile"));
            var indexCommand = new Command("index", "Parse all  html documents in a folder, sending content to OpenAPI as a single file");
            indexCommand.AddArgument(new Argument<DirectoryInfo>("folder"));
            indexCommand.AddArgument(new Argument<string>("name"));
            indexCommand.AddOption(new Option<FileInfo>("--outfile"));

            var enginesCommand = new Command("engines",
                "Describes and provide access to the various models available in the API.");
            enginesCommand.AddOption(new Option<string>("--id"));

            var testCommand = new Command("test", "Test");

            var rootCommand = new RootCommand
            {
                askCommand, addCommand, indexCommand, enginesCommand, testCommand
            };

            var client = new OpenAIClient();
            var consumer = new LibraryConsumer(client);

            askCommand.Handler = CommandHandler.Create<string,string>(consumer.AskQuestion);
            addCommand.Handler = CommandHandler.Create<FileInfo, FileInfo>(consumer.IndexFile);
            indexCommand.Handler = CommandHandler.Create<DirectoryInfo,string, FileInfo> (consumer.IndexFolder);
            enginesCommand.Handler = CommandHandler.Create<string>(EnginesCommandHandler.CallEngines);
            var result = rootCommand.Invoke(args);
            Console.ReadLine();
            Console.WriteLine("fin");
        }
    }
}
