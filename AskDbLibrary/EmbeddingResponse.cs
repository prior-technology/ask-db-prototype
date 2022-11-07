
namespace AskDb.Library
{



    public class EmbeddingResponse
    {
        public string _object { get; set; }
        public Datum[] data { get; set; }
        public EmbeddingResponseUsage usage { get; set; }
    }

    public class EmbeddingResponseUsage
    {
        public int prompt_tokens { get; set; }
        public int total_tokens { get; set; }
    }

    public class Datum
    {
        public string _object { get; set; }
        public float[] embedding { get; set; }
        public int index { get; set; }
    }
}