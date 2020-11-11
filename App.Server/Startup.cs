#if ClientSideExecution
#else
using System.Net.Http;
using Westwind.AspNetCore.LiveReload;
#endif
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Linq;
using System.Reflection;
using App.Server.MediatorPipelines;
using App.Server.QueryHandlers;
using App.Server.Services;
using App.Shared;
using App.Shared.Mediator;
using App.Shared.Queries;
using Core.Jwt;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

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

        private IConfigurationRoot _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var isClientSide = false;
#if ClientSideExecution
            isClientSide = true;
#else
            services.AddRazorPages();
            services.AddServerSideBlazor();

#endif

            services.AddMvc();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
#if ServerSideExecution
            services.AddLiveReload();
#endif

            //services.AddApplicationComponents<ResourceManagerServerFactory>();
            Client.Program.ConfigureServerAndClientSharedServices<ResourceManagerServerFactory>(services);

            //Configure custom services
            services.Configure<Config.Result>(_configuration.GetSection("App"));
            services.AddAuthorization();
            services.AddCoreAuth(_configuration.GetSection("Auth"), isClientSide);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthService,AuthService>();
            
            // Mediator with pipelines
            services.AddScoped(typeof(IQueryPipeline<,>), typeof(LoggingQueryPipeline<,>));
            services.AddScoped(typeof(ICommandPipeline<>), typeof(LoggingCommandPipeline<>));
#if ServerSideExecution
            services.AddTransient<IMediator, SaveServerMediator>();
            services.AddTransient<ServerMediator>();
            services.AddScoped(typeof(IQueryPipeline<,>), typeof(ValidationQueryPipeline<,>));// Not needed for Client side because is already implemented in controllers
            services.AddScoped(typeof(ICommandPipeline<>), typeof(ValidationCommandPipeline<>));
#else
            services.AddTransient<IMediator, ServerMediator>();
#endif
            services.AddScoped(typeof(IQueryPipeline<,>), typeof(HandlerQueryPipeline<,>));
            services.AddScoped(typeof(ICommandPipeline<>), typeof(HandlerCommandPipeline<>));


            //services.AddScoped(typeof(IQueryHandler<IQuery<Config.Result>, Config.Result>), typeof(ConfigQueryHandler));

            services.Scan(scan => scan
                .FromAssemblyOf<Startup>()
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
            services.Scan(scan => scan
                .FromAssemblyOf<Startup>()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
            
            // Register all validators
            services.AddTransient<IValidatorFactory, ValidatorFactory>();
            services.Scan(scan => scan
                .FromAssemblyOf<RequestNotificationContract>()
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
#if ServerSideExecution
                app.UseLiveReload();
#endif
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
#if ClientSideExecution
            app.UseBlazorFrameworkFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToFile("index_cse.html");
            });
#else
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/index_sse");
            });
#endif
        }
    }

}
