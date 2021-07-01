using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using App.Server.QueryHandlers;
using App.Server.Services;
using App.Shared.AuthModels;
using Pipaslot.Mediator;
using App.Shared.Queries;
using Core.Jwt;
using Pipaslot.Mediator.Abstractions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pipaslot.Mediator.Server;
using App.Server.MediatorMiddlewares;
using App.Shared.CQRSAbstraction;

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

            services.AddMediator()
                .AddMarkersFromAssemblyOf<Config.Query>()
                .AddHandlersFromAssemblyOf<ConfigQueryHandler>()
                .Use<LoggingMiddleware>()
                .Use<CommandSpecificMiddleware, ICommand>()
                .Use<QuerySpecificMiddleware, IQuery>()
                .Use<ValidationMiddleware>()
                .UseSequenceMultiHandler<ICommand>();

            // Register all validators from project App.Shared
            services.AddTransient<IValidatorFactory, ValidatorFactory>();
            services.Scan(scan => scan
                .FromAssemblyOf<Config.Query>()
                .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
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

            app.UseBlazorFrameworkFiles();
            app.UseAuthentication();
            app.UseMediator(env.IsDevelopment());
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                //endpoints.MapControllers();
                endpoints.MapFallbackToPage("/Index");
            });
        }
    }

}
