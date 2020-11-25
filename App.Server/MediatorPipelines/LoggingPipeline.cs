using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging;

namespace App.Server.MediatorPipelines
{
    public class LoggingPipeline<TQuery, TResponse> : IPipeline<TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        private readonly ILogger<Program> _logger;

        public LoggingPipeline(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(TQuery request)
        {
            return true;
        }

        public async Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            using (_logger.BeginMethod(request, typeof(TQuery)?.FullName ?? ""))
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    return await next();
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