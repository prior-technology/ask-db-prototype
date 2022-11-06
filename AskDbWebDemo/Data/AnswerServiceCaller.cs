using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using AskDb.Library;
using AskDb.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using System.Text.Json;
using System.Net.Http.Json;
using System.Linq;
using AskDbWebDemo.Pages;

namespace AskDbWebDemo.Data
{
    public class AnswerServiceCaller
    {
        private string OpenAiKey { get; }
        private string FileId { get; }
        private ILogger<AnswerServiceCaller> Log { get; }
        private IQuestionLogger QuestionLogger { get; }
        private ITopicRepository TopicRepository { get; }
        private TopicManager TopicManager { get; }
        private IHttpClientFactory HttpClientFactory { get; }
        private static Dictionary<string, Document> _basicCache { get; } = new Dictionary<string, Document>();
        public AnswerServiceCaller(IConfiguration configuration, IQuestionLogger questionLogger,
             ILogger<AnswerServiceCaller> logger,
             ITopicRepository topicRepository,
            IHttpClientFactory httpClientFactory,
            TopicManager topicManager)
        {
            OpenAiKey = configuration["OpenAI:key"];
            Log = logger;
            QuestionLogger = questionLogger;
            HttpClientFactory = httpClientFactory;
            TopicRepository = topicRepository;
            TopicManager = topicManager;
        }



        public async Task<string[]> Ask(string uid, string question, string topicKey=null)
        {
            try
            {
                var client = GetClient();
                var inquisitor = new QuestionAsker(client);
                var answerLoggerTask = QuestionLogger.LogQuestion(uid, question);
                var answerLogger = await answerLoggerTask;
                var topic = await TopicRepository.GetTopic(uid, topicKey);
                string[] answer;
                if (topic.FileId==null)
                {
                    answer = await AskWithContext(uid, question, topic.FullText);
                } else
                {
                    answer = await inquisitor.AskQuestion(question, topic.FileId, uid);
                }
                
                await answerLogger.LogAnswer(answer[0]);
                return answer;
            }
            catch (Exception e)
            {
                Log.LogError(e,"Failure asking a question");
                return new string[] {"Error!"};
            }
        }

        public async Task<string[]> AskWithContext(string userId, string question, string contextDocument)
        {
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OpenAiKey);
                client.DefaultRequestHeaders.Add("User-Agent", "askdbdemo");
                var completionPrompt = Prompter.GetPrompt(question, contextDocument);
                var completionRequest = new CreateCompletionRequest
                {
                    model= "text-davinci-002",
                    prompt = completionPrompt,
                    max_tokens = 150,
                    temperature= 0.5M,
                    top_p = 1,
                    n=1,
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
                Log.LogError(e, "Failure asking a question");
                return new string[] { "Error!" };
            }
        }

        /// <summary>
        /// Create a topic by splitting up a simple text file
        /// </summary>
        /// <param name="contextDocument"></param>
        /// <returns>topic key</returns>
        public async Task CreateSimpleTopic(string uid, Topic newTopic)
        {           
            await TopicRepository.AddTopic(uid, newTopic.Key,newTopic.Description);
               
        }
        public async Task CreateCompoundTopic(string uid, Topic newTopic)
        {
            var sections = TopicManager.SplitTopic(newTopic.FullText);
            foreach (var section in sections)
            {
                //calculate embedding vector
                var request = new EmbeddingRequest
                {
                    model = "text-search-babbage-doc-001",
                    input = section,
                    user = uid
                };
                
                //persist the section

            }
            await TopicRepository.AddTopic(uid, newTopic.Key, newTopic.Description);
            //calculate and store vectors
            

        }
        public ExampleQuestions GetExamples()
        {
            var examples = new ExampleQuestions();
            return examples;
        }

        private OpenAIClient GetClient()
        {
            return new OpenAIClient(new OpenAIAuthentication(OpenAiKey));
        }

        
        public async Task<Document> GetDocument(Topic topic)
        {
            if (topic.FileId == null || topic.FileId.Length == 0) return null;
            lock (_basicCache)
            {
                if (_basicCache.ContainsKey(topic.FileId)) return _basicCache[topic.FileId];
            }
            var httpClient = HttpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OpenAiKey);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "askdb");

            var fileJsonResponse = await httpClient.GetAsync(string.Format("https://api.openai.com/v1/files/{0}", topic.FileId));
            if (fileJsonResponse.IsSuccessStatusCode)
            {
                var content = await fileJsonResponse.Content.ReadAsStringAsync();
                var document = JsonSerializer.Deserialize<Document>(content);
                lock(_basicCache)
                {
                    if (_basicCache.ContainsKey(topic.FileId)) return _basicCache[topic.FileId];
                    _basicCache.Add(topic.FileId, document);
                }
                
                return document;
            }

            Log.LogError("Request failed, status " + fileJsonResponse.StatusCode);
            try
            {
                var content = await fileJsonResponse.Content.ReadAsStringAsync();
                Log.LogDebug(content);
            }catch (Exception ex)
            {
                Log.LogDebug(ex, "Could not read response content");
            }
            
            return null;
        }
    }
}
