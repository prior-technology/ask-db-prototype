using RestSharp;
using RestSharp.Authenticators;
using System;

namespace AskDb.Library
{
    class OpenApiClient
    {
        private const string BaseUrl = "https://api.openai.com/v1/";

        public string SendQuestion(string question)
        {
            var client = new RestClient(BaseUrl)
            {
                Authenticator = new OpenApiAuthenticator()
            };

            var request = new RestRequest("answers")
                .AddJsonBody(new QuestionRequest
                {
                    model = "curie",
                    question = question,
                    examples = new string[][] {
                        new string[] { "How do I fix slight graininess?", "Use the filter called Selective Blur" },
                    },
                    examples_context = Document.Grain,
                    documents = new string[] { Document.GimpDoc },
                    return_metadata=true
                });
            try
            {
                var response = client.Post(request);
                if (!response.IsSuccessful)
                {
                    Console.WriteLine("Failed");
                    Console.WriteLine(response.ErrorMessage);
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(response.StatusDescription);
                    Console.WriteLine(response.Headers);
                    Console.WriteLine(response.Content);

                    return null;
                }
            
            return response.Content;
            } catch(Exception e)
            {
                Console.WriteLine("Caught Excepton");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
