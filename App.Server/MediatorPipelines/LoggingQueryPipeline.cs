using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Mediator;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging;

namespace App.Server.MediatorPipelines
{
    public class LoggingQueryPipeline<TRequest, TResponse> : IQueryPipeline<TRequest, TResponse>
    {
        private readonly ILogger<Program> _logger;

        public LoggingQueryPipeline(ILogger<Program> logger)
        {
            _logger = logger;
        }
        
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, QueryHandlerDelegate<TResponse> next)
        {
            using (_logger.BeginMethod(request, typeof(TRequest)?.FullName ?? "")){
                
                var stopwatch = Stopwatch.StartNew();
                try{
                    return await next();
                }
                finally{
                    stopwatch.Stop();
                    _logger.LogInformation($"Execution time = {stopwatch.ElapsedMilliseconds}ms");
                }
            }
        }
    }

}