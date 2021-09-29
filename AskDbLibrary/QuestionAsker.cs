using System;

namespace AskDb.Library
{
    public class QuestionAsker
    {
        public void AskQuestion(string question)
        {
            var client = new OpenApiClient();
            var answer = client.SendQuestion(question);
            Console.WriteLine(answer);

        }


    }
}
