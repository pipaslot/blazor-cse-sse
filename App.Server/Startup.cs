using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using App.Server.MediatorPipelines;
using App.Server.QueryHandlers;
using App.Server.Services;
using App.Shared;
using App.Shared.AuthModels;
using Core.Mediator;
using App.Shared.Queries;
using Core.Jwt;
using Core.Mediator.Abstractions;
using Core.Mediator.Pipelines;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Core.Mediator.Server;

namespace App.Server
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"appsettings.Local.json", optional: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        private readonly IConfigurationRoot _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            Client.Program.ConfigureServerAndClientSharedServices<ResourceManagerServerFactory>(services);

            services.Configure<Config.Result>(_configuration.GetSection("App"));

            services.AddAuthorization();
            services.AddCoreAuthForClient(_configuration.GetSection("Auth"));


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Mediator with pipelines
            services.AddMediator()
                .AddHandlersFromAssemblyOf<ConfigQueryHandler>()
                .Use<LoggingPipeline>()
                .UseEventOnly<CommandSpecificPipeline, ICommand>()
                .UseRequestOnly<QuerySpecificPipeline, IQuery>()
                .Use<ValidationPipeline>()
                .UseEventOnly<MultiHandlerSequenceExecutionEventPipeline, ICommand>();

            // Register all validators from project App.Shared
            services.AddTransient<IValidatorFactory, ValidatorFactory>();
            services.Scan(scan => scan
                .FromAssemblyOf<UserCredentials>()
                .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, HandlerExistenceChecker handlerExistenceChecker)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                handlerExistenceChecker
                    .ScanFromAssemblyOf<Config.Query>()
                    .SearchCustomEvents<ICommand>()
                    .SearchCustomRequests<IQuery>()
                    .Verify();
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMediator();

            app.UseBlazorFrameworkFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }

}
