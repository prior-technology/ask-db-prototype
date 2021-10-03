using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI;

namespace AskDbConsole
{
    public class EnginesCommandHandler 
    {
        public static async Task CallEngines(string id)
        {
            var client = new OpenAIClient();
            var endpoint = client.EnginesEndpoint;
            if (id == null)
            {
                var engines = await endpoint.GetEnginesAsync();
                var resultJson = JsonSerializer.Serialize(engines, new JsonSerializerOptions {WriteIndented = true});
                Console.WriteLine(resultJson);
                return;
            }

            var engine = await endpoint.GetEngineDetailsAsync(id);
            var engineJson = JsonSerializer.Serialize(engine, new JsonSerializerOptions {WriteIndented = true});
            Console.WriteLine(engineJson);

        }
    }
}
