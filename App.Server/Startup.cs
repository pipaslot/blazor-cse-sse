#if ServerSideExecution
using System.Net.Http;
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
using App.Server.Services;
using App.Shared;
using App.Shared.AuthModels;
using Core.Mediator;
using App.Shared.Queries;
using Core.Jwt;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
#if ServerSideExecution
    using Microsoft.AspNetCore.Authentication.Cookies;
#else
    using Microsoft.AspNetCore.Authentication.JwtBearer;
#endif

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
            services.AddMvc();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
#if ServerSideExecution
            services.AddRazorPages();
            services.AddServerSideBlazor();
#endif

            Client.Program.ConfigureServerAndClientSharedServices<ResourceManagerServerFactory>(services);
            
            services.Configure<Config.Result>(_configuration.GetSection("App"));

            services.AddAuthorization();
#if ServerSideExecution
            services.AddCoreAuthForServer(_configuration.GetSection("Auth"));
#else
            services.AddCoreAuthForClient(_configuration.GetSection("Auth"));
#endif
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthService,AuthService>();
            
            // Mediator with pipelines
            services.AddTransient<IMediator, Mediator>();
            services.AddMediatorCommandPipeline(typeof(LoggingCommandPipeline<>));
            services.AddMediatorCommandPipeline(typeof(ValidationCommandPipeline<>));

            services.AddMediatorQueryPipeline(typeof(LoggingQueryPipeline<,>));
            services.AddMediatorQueryPipeline(typeof(ValidationQueryPipeline<,>));


            // Automatically register all query handlers from project App.Server
            services.Scan(scan => scan
                .FromAssemblyOf<Startup>()
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );

            // Automatically register all command handlers from project App.Server
            services.Scan(scan => scan
                .FromAssemblyOf<Startup>()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
            
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
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
#if ServerSideExecution
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/index_sse");
            });
            
#else
            app.UseBlazorFrameworkFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToFile("index_cse.html");
            });  
#endif
        }
    }

}
