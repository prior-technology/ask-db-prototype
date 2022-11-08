using AskDb.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Linq;
using MathNet.Numerics;

namespace AskDb.Library
{
    public class TopicManager
    {
        const string DocEmbeddingEngine = "text-search-babbage-doc-001";
        const string QueryEmbeddingEngine = "text-search-babbage-query-001";

        private ITopicRepository TopicRepository { get; }
        private ILogger Logger { get; }
        private IDocumentStorage DocumentStorage { get; }

        private string OpenAiKey { get; }
        public TopicManager(IConfiguration configuration, ITopicRepository topicRepository, IDocumentStorage docStorage, ILogger<TopicManager> logger)
        {
            TopicRepository = topicRepository;
            OpenAiKey = configuration["OpenAI:key"];
            Logger = logger;
            DocumentStorage = docStorage;
        }
        
        public async Task<float[]> GetDocEmbedding(string uid, string section)
        {
            return await GetEmbedding(DocEmbeddingEngine, uid, section);
        }
        public async Task<float[]> GetEmbedding(string engine, string uid, string section)
        {
            try
            {
                //calculate embedding vector
                var request = new EmbeddingRequest
                {
                    model = engine,
                    input = section,
                    user = uid
                };

                using var client = GetHttpClient();
                var response = await client.PostAsJsonAsync("https://api.openai.com/v1/embeddings", request);
                var result = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();

                return result.data.First().embedding;
            }
            catch (Exception)
            {
                var fragment = section.Substring(0, Math.Min(20, section.Length)) + "...";
                Logger.LogError("Error getting embedding for {uid} {section}", uid,fragment );
                throw;
            }
            
        }
        public async Task CreateCompoundTopic(string uid, Topic newTopic)
        {
            var sections = newTopic.FullText.SplitTopic();
            var addDocTask = DocumentStorage.SaveBlob(uid, newTopic.Key, newTopic.FullText);

            
            var taskList = new List<Task> { addDocTask };
            var sectionList = new List<TopicSection>();
            int sectionNumber = 0;
            foreach (var section in sections)
            {
                var embeddingVector = await GetDocEmbedding(uid, section);
                var topicSection = new TopicSection
                {
                    Id = sectionNumber,
                    EmbeddingVector = embeddingVector
                };
                sectionList.Add(topicSection);
                var sectionName = $"{newTopic.Key}-{sectionNumber}";
                var addSectionTask = DocumentStorage.SaveBlob(uid, sectionName, section);
                taskList.Add(addSectionTask);
                sectionNumber++;
            }
            newTopic.Sections = sectionList.ToArray();
            await Task.WhenAll(taskList);

            newTopic.FullText = "";

            await TopicRepository.AddTopic(uid, newTopic);
        }

        private HttpClient GetHttpClient()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OpenAiKey);
            client.DefaultRequestHeaders.Add("User-Agent", "askdbdemo");
            return client;
        }
        public async Task<string[]> AskWithContext(string userId, string question, string contextDocument)
        {
            try
            {
                using var client = GetHttpClient();
                var completionPrompt = Prompter.GetPrompt(question, contextDocument);
                var completionRequest = new CreateCompletionRequest
                {
                    model = "text-davinci-002",
                    prompt = completionPrompt,
                    max_tokens = 150,
                    temperature = 0.5M,
                    top_p = 1,
                    n = 1,
                    stream = false,
                    user = userId,
                };

                var response = await client.PostAsJsonAsync("https://api.openai.com/v1/completions", completionRequest);
                var responseString = await response.Content.ReadAsStringAsync();
                var completionResponse = JsonSerializer.Deserialize<CreateCompletionResponse>(responseString);
                return completionResponse.choices.Select(c => c.text).ToArray();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failure asking a question");
                return new string[] { "Error!" };
            }
        }
        private float Similarity(float[] queryEmbedding, float[] docEmbedding)
        {
            return Distance.Cosine(queryEmbedding, docEmbedding);
        }
        
        public async Task<string[]> AskCompoundTopic(string userId, string question, Topic topic)
        {
            try
            {
                var queryEmbedding = await GetEmbedding(QueryEmbeddingEngine, userId, question);
                var results = new List<SearchResult>();
                foreach (var section in topic.Sections)
                {
                    var result = new SearchResult
                    {
                        Section = section,
                        Score = Similarity(queryEmbedding, section.EmbeddingVector)
                    };
                    results.Add(result);
                }
                var sortedResults = results.OrderByDescending(r => r.Score).ToList();
                var topResult = sortedResults.First();
                var sectionText = await DocumentStorage.GetBlob(userId, $"{topic.Key}-{topResult.Section.Id}");
                var completionPrompt = Prompter.GetPrompt(question, sectionText);
                var completionRequest = new CreateCompletionRequest
                {
                    model = "text-davinci-002",
                    prompt = completionPrompt,
                    max_tokens = 150,
                    temperature = 0.5M,
                    top_p = 1,
                    n = 1,
                    stream = false,
                    user = userId,
                };
                using var client = GetHttpClient();
                var response = await client.PostAsJsonAsync("https://api.openai.com/v1/completions", completionRequest);
                var responseString = await response.Content.ReadAsStringAsync();
                var completionResponse = JsonSerializer.Deserialize<CreateCompletionResponse>(responseString);
                return completionResponse.choices.Select(c => c.text).ToArray();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failure asking a question");
                return new string[] { "Error!" };
            }
        }
    }
}
