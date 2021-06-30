using Core.Mediator.Abstractions;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.Mediator.Server
{
    public class MediatorMiddleware
    {
        private readonly RequestDelegate _next;

        public MediatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == DataContract.Endpoint && context.Request.Method.ToUpper() == "POST")
            {
                var mediator = context.RequestServices.GetService(typeof(IMediator)) as IMediator;
                if (mediator == null)
                {
                    throw new System.Exception($"Interface {typeof(IMediator).FullName} was not registered in service collection");
                }
                var contract = await GetContract(context);
                if (contract == null)
                {
                    throw new System.Exception($"Can not parse contract object from request body");
                }
                var executor = new RequestContractExecutor(mediator);
                var result = await executor.ExecuteQuery(contract, context.RequestAborted);
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(result);
            }
            else
            {
                await _next(context);
            }
        }

        private async Task<DataContract?> GetContract(HttpContext context)
        {
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                var body = await reader.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(body))
                {
                    throw new System.Exception("Request body has empty body. JSON was expected.");
                }
                return JsonSerializer.Deserialize<DataContract>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }
    }
}
