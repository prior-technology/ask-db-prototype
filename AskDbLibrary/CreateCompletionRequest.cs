
namespace AskDb.Library
{
    public class CreateCompletionRequest
    {
        public string model { get; set; }
        public string prompt { get; set; }
        public int max_tokens { get; set; }
        public decimal temperature { get; set; }
        public int top_p { get; set; }
        public int n { get; set; }
        public bool stream { get; set; }
        public decimal logprobs { get; set; }
        public string stop { get; set; }
    }


}
