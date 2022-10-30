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

namespace AskDbWebDemo.Data
{
    public class AnswerServiceCaller
    {
        private string OpenAiKey { get; }
        private string FileId { get; }
        private ILogger<AnswerServiceCaller> Log { get; }
        private IQuestionLogger QuestionLogger { get; }

        private ITopicRepository TopicRepository { get; }
        private string UserId { get; }

        private readonly IHttpClientFactory _httpClientFactory;
        private static Dictionary<string, Document> _basicCache { get; } = new Dictionary<string, Document>();
        public AnswerServiceCaller(IConfiguration configuration, IQuestionLogger questionLogger,
             ILogger<AnswerServiceCaller> logger,
             ClaimsPrincipal user,
             ITopicRepository topicRepository,
            IHttpClientFactory httpClientFactory)
        {
            OpenAiKey = configuration["OpenAI:key"];
            Log = logger;
            QuestionLogger = questionLogger;
            UserId = GetUserSid(user);
            _httpClientFactory = httpClientFactory;
            TopicRepository = topicRepository;
        }



        public async Task<string[]> Ask(string question, string topic=null)
        {
            try
            {
                var client = GetClient();
                var inquisitor = new QuestionAsker(client);
                var answerLoggerTask = QuestionLogger.LogQuestion(question);
                var answerLogger = await answerLoggerTask;
                var fileId = await TopicRepository.GetFileIdForTopic(topic);
                var answer = await inquisitor.AskQuestion(question, fileId, UserId);
                await answerLogger.LogAnswer(answer[0]);
                return answer;
            }
            catch (Exception e)
            {
                Log.LogError(e,"Failure asking a question");
                return new string[] {"Error!"};
            }
        }

        public async Task<string[]> AskWithContext(string question, string contextDocument)
        {
            try
            {
                var key = Environment.GetEnvironmentVariable("OPENAI_KEY");

                using HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
                client.DefaultRequestHeaders.Add("User-Agent", "askdbdemo");
                var completionPrompt = Prompter.GetPrompt(question, contextDocument);
                var completionRequest = new CompletionRequest
                {
                    Prompt = completionPrompt,
                    MaxTokens = 100,
                    Temperature = 0.5,
                    TopP = 1,
                    PresencePenalty = 0,
                    FrequencyPenalty = 0
                };
                var response = await client.PostAsJsonAsync("https://api.openai.com/v1/completions", completionRequest);
                var responseString = await response.Content.ReadAsStringAsync();
                var completionResponse = JsonSerializer.Deserialize<CompletionResult>(responseString);
                return completionResponse.Completions.Select(c => c.Text).ToArray();               

            }
            catch (Exception e)
            {
                Log.LogError(e, "Failure asking a question");
                return new string[] { "Error!" };
            }
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

        private string GetUserSid(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                Log.LogDebug("claimsPrincipal null");
                return UseFakeSid();
            }

            var sid = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

            if (sid == null)
            {
                Log.LogDebug($"No claim of type {ClaimTypes.NameIdentifier}");
                return UseFakeSid();
            }

            Log.LogDebug($"Using userSid {sid.Value}");
            return sid.Value;
        }
        private string UseFakeSid()
        {
            var fakeSid = Guid.NewGuid().ToString();
            Log.LogDebug($"Using fake sid {fakeSid}");
            return fakeSid;
        }

        public async Task<Document> GetDocument(Topic topic)
        {
            if (topic.FileId == null || topic.FileId.Length == 0) return null;
            lock (_basicCache)
            {
                if (_basicCache.ContainsKey(topic.FileId)) return _basicCache[topic.FileId];
            }
            var httpClient = _httpClientFactory.CreateClient();
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
