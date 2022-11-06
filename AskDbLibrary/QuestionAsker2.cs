
namespace AskDb.Library
{
    public class Prompter
    {
        public const string QuestionPrompt = "Answer the question as truthfully as possible using the context below, or if the answer is not there say \"I don't know.\"";

        public static string GetPrompt(string question, string contextDocument)
        {
            return $"{QuestionPrompt}\n\nQuestion:\n{question}\n\nContext:\n{contextDocument}\n\nAnswer:";
        }

    }
}
