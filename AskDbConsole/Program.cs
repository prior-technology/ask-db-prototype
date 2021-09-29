using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using AskDb.Library;

namespace AskDbConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Setup command line arguments
            var askCommand = new Command("ask", "Send a question to the question answering API");
            askCommand.AddArgument(new Argument<string>("question"));

            var indexCommand = new Command("index", "Parse a folder of html files, sending content to OpenAPI");
            indexCommand.AddArgument(new Argument<DirectoryInfo>("folder"));

            var rootCommand = new RootCommand
            {
                askCommand, indexCommand
            };
            var client = new QuestionAsker();
            askCommand.Handler = CommandHandler.Create<string>(client.AskQuestion);
            var result = rootCommand.Invoke(args);
            Console.ReadLine();
            Console.WriteLine("fin");
            

        }
    }
}
