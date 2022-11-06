using System.Threading.Tasks;

namespace AskDb.Library
{
    public interface IQuestionLogger
    {
        public Task<IAnswerLogger> LogQuestion(string userSid, string question);

    }

    public interface IAnswerLogger
    {
        public Task LogAnswer(string answer);
    }
}
