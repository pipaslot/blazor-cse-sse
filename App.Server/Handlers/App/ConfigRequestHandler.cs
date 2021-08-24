using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Mediator.Abstractions;
using Microsoft.Extensions.Options;
using App.Shared.App;

namespace App.Server.Handlers.App
{
    // ReSharper disable once UnusedType.Global
    public class ConfigRequestHandler : IRequestHandler<ConfigRequest.Query, ConfigRequest.Result>
    {
        private readonly IOptions<ConfigRequest.Result> _configOptions;

        public ConfigRequestHandler(IOptions<ConfigRequest.Result> config)
        {
            _configOptions = config;
        }

        public Task<ConfigRequest.Result> Handle(ConfigRequest.Query query, CancellationToken cancellationToken)
        {
            var config = _configOptions.Value;
            return Task.FromResult(new ConfigRequest.Result
            {
                Languages = config.Languages,
                Name = config.Name
            });
        }
    }
}