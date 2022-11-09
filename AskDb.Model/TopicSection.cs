
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Text;
using System.Text.Json.Serialization;

namespace AskDb.Model
{
    public class TopicSection
    {
        public int Id { get; set; }

        [JsonIgnore]
        public float[] EmbeddingVector { get; set; }
        public string EncodedEmbeddingVector { 
            get
            {
                //return EmbeddingVector as a base64 encoded string
                return Encode(EmbeddingVector);                
            }
            set
            {
                //set EmbeddingVector from a base64 encoded string
                EmbeddingVector = Decode(value);
            }
        }
        public static string Encode(float[] vector)
        {
            var bytes = new byte[vector.Length * sizeof(float)];
            Buffer.BlockCopy(vector, 0, bytes, 0, bytes.Length);
            return Convert.ToBase64String(bytes);
        }
        public static float[] Decode(string encodedVector)
        {
            var bytes = Convert.FromBase64String(encodedVector);
            var vector = new float[bytes.Length / sizeof(float)];
            Buffer.BlockCopy(bytes, 0, vector, 0, bytes.Length);
            return vector;            
        }
    }
}
