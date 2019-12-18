using System.Threading.Tasks;
using App.Shared;
using Microsoft.Extensions.Options;

namespace App.Server.Services
{
    public class AppSettingConfigProvider : IConfigProvider
    {
        private readonly IOptions<Config> _config;

        public AppSettingConfigProvider(IOptions<Config> config)
        {
            _config = config;
        }

        public Task<Config> GetConfig()
        {
            return Task.FromResult(_config.Value);
        }
    }
}
