using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Resources;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Components.Resources;
using Components.States;
using Microsoft.Extensions.DependencyInjection;

namespace Components
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationComponents(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<AppState>()
                .AddClasses(c=> c
                    .AssignableTo<TemporaryState>())
                .AsSelf()
                .WithScopedLifetime()
            );
            services.AddStorage();

            //Register all resources
            services.AddResources()
                .Register<LayoutResource>();
        }
    }
}
