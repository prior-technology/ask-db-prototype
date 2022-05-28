using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using Azure.Identity;

namespace AskDbWebDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var vaultUri = Environment.GetEnvironmentVariable("VaultUri");
            var builder = Host.CreateDefaultBuilder(args);
            if (vaultUri != null)
            {
                builder = builder.ConfigureAppConfiguration((context, config) =>
                {
                    var keyVaultEndpoint = new Uri(vaultUri);
                    config.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
                });
            }
            
            return builder.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}
