using System.Threading.Tasks;
using App.Shared;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace App.Server
{
    public class ConfigProviderMiddleware
    {
        private readonly RequestDelegate _next;

        public ConfigProviderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IConfigProvider configProvider)
        {
            if (context.Request.Path.Value.StartsWith($"/config.json"))
            {
                context.Response.ContentType = "application/json; charset=utf-8";
                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                var config = await configProvider.GetConfig();
                var json = JsonConvert.SerializeObject(config, serializerSettings);
                await context.Response.WriteAsync(json);
            }
            else
            {
                await _next(context);
            }
        }
    }
}
