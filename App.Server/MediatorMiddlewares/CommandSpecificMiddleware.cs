﻿using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Mediator.Abstractions;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorMiddlewares
{
    public class CommandSpecificMiddleware : IMediatorMiddleware
    {
        private readonly ILogger<Program> _logger;

        public CommandSpecificMiddleware(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public async Task Invoke<TAction>(TAction action, MediatorResponse response, MiddlewareDelegate next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hello from " + nameof(CommandSpecificMiddleware));

            await next();
        }
    }
}