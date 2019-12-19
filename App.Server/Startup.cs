﻿#if ClientSideExecution
#else
using System.Net.Http;
using Westwind.AspNetCore.LiveReload;
#endif
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Linq;
using App.Server.Services;
using App.Shared;
using Core.Auth;
using Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace App.Server
{
    public class Startup
    {
        NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

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
            logger.Debug("Beginning Startup.ConfigureServices() in CSE mode");
#else
            logger.Debug("Beginning Startup.ConfigureServices() in SSE mode");
            services.AddRazorPages();

            // Adds the Server-Side Blazor services, and those registered by the app project's startup.
            logger.Debug("Adding AddServerSideBlazor...");
            services.AddServerSideBlazor();

#endif

            logger.Debug("Adding Mvc...");
            services.AddMvc();

            logger.Debug("Adding ResponseCompression...");
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
#if ServerSideExecution
            logger.Debug("Adding LiveReload");
            services.AddLiveReload();
#endif

            logger.Debug("Adding ApplicationComponents");
            services.AddApplicationComponents();

            //Configure custom services
            services.Configure<Config>(_configuration.GetSection("App"));
            services.AddSingleton<IConfigProvider, AppSettingConfigProvider>();
            services.AddAuthorization();
            services.AddCoreAuth(_configuration.GetSection("Auth"), isClientSide);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthService,AuthService>();

            logger.Debug("Completed Startup.ConfigureServices()");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            logger.Debug("");
            logger.Debug("Beginning Startup.Configure()");

            logger.Debug("UseResponseCompression...");
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
#if ServerSideExecution
                app.UseLiveReload();
#endif
                logger.Debug("UseDeveloperExceptionPage...");
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }
            else
            {
                logger.Debug("UseExceptionHandler...");
                app.UseExceptionHandler("/Home/Error");

                logger.Debug("UseHsts...");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            logger.Debug("UseHttpsRedirection...");
            app.UseHttpsRedirection();

            logger.Debug("UseStaticFiles...");
            app.UseStaticFiles();
            app.UseMiddleware<ConfigProviderMiddleware>();
            app.UseAuthentication();
#if ClientSideExecution

            logger.Debug("UseClientSideBlazorFiles...");
            app.UseClientSideBlazorFiles<App.Client.Startup>();

            logger.Debug("UseRouting...");
            app.UseRouting();

            logger.Debug("UseEndpoints...");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToClientSideBlazor<App.Client.Startup>("index_cse.html");
            });
#else
            logger.Debug("UseRouting...");
            app.UseRouting();

            logger.Debug("UseEndpoints...");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/index_sse");
            });
#endif

            logger.Debug("Completed Startup.Configure()");
        }
    }

}
