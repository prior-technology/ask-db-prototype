using System;
using System.Drawing;
using AskDbWebDemo.Data;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Linq;
using AskDb.Library;
using AskDb.Library.Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace AskDbWebDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var section = Configuration.GetSection("AzureAdB2C");
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(section);
            services.AddControllersWithViews()
                .AddMicrosoftIdentityUI();

            services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
            services.AddSignalR().AddAzureSignalR();
            services.AddRazorPages();
            services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddScoped<ITopicRepository>(MakeTopicRepository);
            services.AddScoped<AnswerServiceCaller>(MakeAnswerServiceCaller);
            services.AddScoped<IQuestionLogger, TableStorageQuestionLogger>(MakeTableStorageQuestionLogger);
            services.AddSingleton<TableStorageResponseHandler>();
            
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        TableStorageQuestionLogger MakeTableStorageQuestionLogger(IServiceProvider serviceProvider)
        {
            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor?.HttpContext;
            return new TableStorageQuestionLogger(serviceProvider.GetService<IConfiguration>(),
                serviceProvider.GetService<ILoggerFactory>(), 
                serviceProvider.GetService<TableStorageResponseHandler>(),
                httpContext?.User);
        }

        ITopicRepository MakeTopicRepository(IServiceProvider serviceProvider)
        {
            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor?.HttpContext;
            return new TableStorageTopicRepository(serviceProvider.GetService<IConfiguration>(),
                serviceProvider.GetService<ILogger<TableStorageTopicRepository>>(),
                httpContext?.User
                );
        }
        AnswerServiceCaller MakeAnswerServiceCaller(IServiceProvider serviceProvider)
        {
            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor?.HttpContext;
            return new AnswerServiceCaller(serviceProvider.GetService<IConfiguration>(),
                serviceProvider.GetService<IQuestionLogger>(),
                serviceProvider.GetService<ILogger<AnswerServiceCaller>>(),
                httpContext?.User,
                serviceProvider.GetService<ITopicRepository>(),
                serviceProvider.GetService<IHttpClientFactory>());
        }
    }
}
