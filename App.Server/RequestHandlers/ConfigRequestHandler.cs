using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Config = App.Shared.Requests.Config;

namespace App.Server.RequestHandlers
{
    // ReSharper disable once UnusedType.Global
    public class ConfigRequestHandler : IRequestHandler<Config.Request, Config.Result>
    {
        private readonly IOptions<Config.Result> _config;

        public ConfigRequestHandler(IOptions<Config.Result> config)
        {
            _config = config;
        }

        public Task<Config.Result> Handle(Config.Request request, CancellationToken cancellationToken)
        {
            var config = _config.Value;
            return Task.FromResult(new Config.Result
            {
                Languages = config.Languages,
                Name = config.Name
            });
        }
    }
}