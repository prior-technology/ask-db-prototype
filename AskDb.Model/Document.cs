using System.Text.Json.Serialization;

namespace AskDb.Model
{
    public class Document
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("filename")] 
        public string FileName { get; set; }
        [JsonPropertyName("purpose")]
        public string Purpose { get; set; }
    }
}
