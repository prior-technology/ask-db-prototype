using System;
using System.Collections;
using System.Collections.Generic;
using OpenAI;
namespace AskDb.Library
{
    public class QuestionAsker
    {
        public OpenAIClient Client { get; set; }
        public IList<string> Documents { get; set; }

        public QuestionAsker(OpenAIClient client = null)
        {
            Client = client ??  new OpenAIClient();
        }
        public async void AskQuestion(string question)
        {
            
            var searchEp = Client.SearchEndpoint;
            var results = await searchEp.GetSearchResultsAsync(question, Documents);
            foreach (var result in results)
            {
                Console.WriteLine($"{result.Key}: {result.Value}");
            }
        }


    }
}
