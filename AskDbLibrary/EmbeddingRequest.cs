using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskDb.Library
{
    public class EmbeddingRequest
    {
        public string model { get; set; }
        public string input { get; set; }
        public string user { get; set; }
    }

    public class EmbeddingsRequest
    {
        public string model { get; set; }
        public string[] input { get; set; }
        public string user { get; set; }
    }
}
