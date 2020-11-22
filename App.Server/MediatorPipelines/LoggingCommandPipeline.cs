using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging;

namespace App.Server.MediatorPipelines
{
    public class LoggingCommandPipeline<TCommand> : ICommandPipeline<TCommand> where TCommand : notnull
    {
        private readonly ILogger<Program> _logger;

        public LoggingCommandPipeline(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public async Task Handle(TCommand command, CancellationToken cancellationToken, CommandHandlerDelegate next)
        {
            using (_logger.BeginMethod(command, typeof(TCommand)?.FullName ?? ""))
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    await next();
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