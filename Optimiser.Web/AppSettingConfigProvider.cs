using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Components.Services;
using Microsoft.Extensions.Options;

namespace Optimiser.Web
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
