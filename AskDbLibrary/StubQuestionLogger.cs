using System;
using System.Threading.Tasks;

namespace AskDb.Library
{
    public class StubQuestionLogger: IQuestionLogger
    {
        public Task<IAnswerLogger> LogQuestion(string userSid, string question)
        {
            return Task.FromResult<IAnswerLogger>(new StubAnswerLogger(question));

        }
    }

    public class StubAnswerLogger : IAnswerLogger
    {
        private readonly string _question;

        public StubAnswerLogger(string question)
        {
            _question = question;
        }
        public Task LogAnswer(string answer)
        {
            return Task.Run(() =>
                Console.WriteLine($"Question: {_question}, Answer: {answer}")
            );


        }
    }
}
