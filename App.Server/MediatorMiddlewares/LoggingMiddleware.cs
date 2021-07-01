using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging;

namespace App.Server.MediatorMiddlewares
{
    public class LoggingMiddleware : IMediatorMiddleware
    {
        private readonly ILogger<Program> _logger;

        public LoggingMiddleware(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public async Task Invoke<TAction>(TAction action, MediatorResponse response, MiddlewareDelegate next, CancellationToken cancellationToken)
        {
            using (_logger.BeginMethod(action, action?.GetType()?.FullName ?? ""))
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    await next();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Pipeline exception");
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    _logger.LogInformation($"Execution time = {stopwatch.ElapsedMilliseconds}ms");
                }
            }
        }
    }
}