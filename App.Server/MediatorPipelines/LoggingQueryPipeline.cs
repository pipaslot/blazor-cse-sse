using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging;

namespace App.Server.MediatorPipelines
{
    public class LoggingQueryPipeline<TQuery, TResponse> : IQueryPipeline<TQuery, TResponse> where TQuery : notnull
    {
        private readonly ILogger<Program> _logger;

        public LoggingQueryPipeline(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken, QueryHandlerDelegate<TResponse> next)
        {
            using (_logger.BeginMethod(query, typeof(TQuery)?.FullName ?? ""))
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