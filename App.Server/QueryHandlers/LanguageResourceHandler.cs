using App.Shared.Queries;
using App.Shared.CQRSAbstraction;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Resources;

namespace App.Server.QueryHandlers
{
    public class LanguageResourceHandler : IQueryHandler<LanguageResource.Query, LanguageResource.Result>
    {
        public Task<LanguageResource.Result> Handle(LanguageResource.Query request, CancellationToken cancellationToken)
        {
            var resource = GetResource(request);
            return Task.FromResult(new LanguageResource.Result
            {
                Resource = resource
            });
        }

        private Dictionary<string, string> GetResource(LanguageResource.Query request)
        {
            var type = Type.GetType(request.TypeName);
            if (type == null)
            {
                return new Dictionary<string, string>();
            }
            var resourceManager = new ResourceManager(type);
            return resourceManager.ToDictionary(request.Language);
        }
    }

    internal static class ResourceManagerExtensions
    {
        public static Dictionary<string, string> ToDictionary(this ResourceManager resourceManager, string locale)
        {
            var culture = new System.Globalization.CultureInfo(locale);
            var data = resourceManager.GetResourceSet(culture, true, true)
                ?.GetEnumerator();
            var res = new Dictionary<string, string>();
            if (data != null)
            {
                while (data.MoveNext())
                {
                    if (data.Key != null)
                    {
                        res.Add(data.Key.ToString() ?? "", data.Value?.ToString() ?? "");
                    }
                }
            }

            return res;
        }
    }
}
