using System;
using System.Threading.Tasks;
using OpenAI;
namespace AskDb.Library
{
    public class QuestionAsker
    {
        public OpenAIClient Client { get; set; }
        public ExampleQuestions ExampleQuestions { get; set; }
        public QuestionAsker(OpenAIClient client = null)
        {
            Client = client ??  new OpenAIClient();
            ExampleQuestions = new ExampleQuestions();
        }

        public Task<string[]> AskQuestion(string question, string fileId, string userId = null)
        {
            var answersEndpoint = Client.AnswersEndpoint;
            if (fileId == null)
            {
                throw new NotImplementedException("Only file-id supported");
            }

            return answersEndpoint.GetAnswersAsync(question, fileId, ExampleQuestions.ExamplesAsArray(),
                ExampleQuestions.ContextDocument, userId);

        }

    }
}
