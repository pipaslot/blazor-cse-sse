using System.Threading;
using System.Threading.Tasks;
using App.Shared.CQRSAbstraction;
using Microsoft.Extensions.Options;
using Config = App.Shared.Queries.Config;

namespace App.Server.QueryHandlers
{
    // ReSharper disable once UnusedType.Global
    public class ConfigQueryHandler : IQueryHandler<Config.Query, Config.Result>
    {
        private readonly IOptions<Config.Result> _configOptions;

        public ConfigQueryHandler(IOptions<Config.Result> config)
        {
            _configOptions = config;
        }

        public Task<Config.Result> Handle(Config.Query query, CancellationToken cancellationToken)
        {
            var config = _configOptions.Value;
            return Task.FromResult(new Config.Result
            {
                Languages = config.Languages,
                Name = config.Name
            });
        }
    }
}