﻿using System;
using System.Collections.Generic;
using System.Text;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Components.StateAbstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Components
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationComponents(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<State>()
                .AddClasses(c=> c
                    .AssignableTo<State>())
                .AsSelf()
                .WithScopedLifetime()
            );
            services.AddStorage();
        }
    }
}