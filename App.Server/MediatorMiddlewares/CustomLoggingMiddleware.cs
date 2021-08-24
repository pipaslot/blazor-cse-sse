using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging;
using Pipaslot.Mediator.Middlewares;

namespace App.Server.MediatorMiddlewares
{
    public class CustomLoggingMiddleware : IMediatorMiddleware
    {
        private readonly ILogger<Program> _logger;

        public CustomLoggingMiddleware(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public async Task Invoke<TAction>(TAction action, MediatorContext context, MiddlewareDelegate next, CancellationToken cancellationToken)
        {
            using (_logger.BeginMethod(action, action?.GetType()?.FullName ?? ""))
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    await next(context);
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