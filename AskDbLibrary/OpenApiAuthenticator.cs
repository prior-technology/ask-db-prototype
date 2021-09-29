using RestSharp;
using RestSharp.Authenticators;
using System.IO;

namespace AskDb.Library
{
    internal class OpenApiAuthenticator : IAuthenticator
    {
        private const string KeyFile = ".openai";
        private string _cachedKey = null;
        private string ApiKey()
        {
            if (_cachedKey != null)
            {
                return _cachedKey;
            }

            if (!File.Exists(KeyFile))
                return null;
            var lines = File.ReadAllLines(KeyFile);
            _cachedKey = lines[0];

            return _cachedKey;

        }
        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddOrUpdateHeader("Authorization", "Bearer " + ApiKey());
        }
    }
}