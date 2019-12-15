#if ClientSideExecution
#else
using System.Net.Http;
#endif

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Linq;
using Components;

namespace Optimiser.Web
{
    public class Startup
    {
        NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            logger.Debug("");
#if ClientSideExecution
            logger.Debug("Beginning Startup.ConfigureServices() in CSE mode");
#else
            logger.Debug("Beginning Startup.ConfigureServices() in SSE mode");

            logger.Debug("Adding AddRazorPages...");
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

            services.AddApplicationComponents();
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

#if ClientSideExecution
            // Serving Client wwwroot folder
            /*
            logger.Debug("UseFileServer...");
            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(
                     Path.Combine(
                         $@"{Directory.GetParent(Directory.GetCurrentDirectory())}\Optimiser.Blazor",
                         @"wwwroot")
                ),
                RequestPath = new PathString("")
            });
            */

            logger.Debug("UseClientSideBlazorFiles...");
            app.UseClientSideBlazorFiles<Optimiser.Blazor.Startup>();

            logger.Debug("UseRouting...");
            app.UseRouting();

            logger.Debug("UseEndpoints...");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToClientSideBlazor<Optimiser.Blazor.Startup>("index_cse.html");
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
