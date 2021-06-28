using Microsoft.AspNetCore.Builder;

namespace Core.Mediator.Server
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Register Mediator middleware handling messages
        /// </summary>
        public static IApplicationBuilder UseMediator(this IApplicationBuilder app) {
            app.UseMiddleware<MediatorMiddleware>();
            return app;
        }
    }
}
