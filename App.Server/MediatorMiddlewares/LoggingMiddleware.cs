using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging;

namespace App.Server.MediatorMiddlewares
{
    public class LoggingMiddleware : IRequestMiddleware, IEventMiddleware
    {
        private readonly ILogger<Program> _logger;

        public LoggingMiddleware(ILogger<Program> logger)
        {
            _logger = logger;
        }


        public async Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, MiddlewareDelegate<TResponse> next) where TRequest : IRequest<TResponse>
        {
            using (_logger.BeginMethod(request, request.GetType().FullName ?? ""))
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    return await next();
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

        public async Task Handle<TEvent>(TEvent @event, CancellationToken cancellationToken, MiddlewareDelegate next) where TEvent : IEvent
        {
            using (_logger.BeginMethod(@event, @event.GetType().FullName ?? ""))
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