using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using App.Shared.Queries;
using Core.Localization;
using Pipaslot.Mediator.Abstractions;

namespace App.Client.Services
{
    public class ResourceManager : ResourceManagerWithCulture
    {
        private readonly IMediator _mediator;
        private readonly Type _classType;

        public ResourceManager(Type classType, IMediator mediator) : base(classType.FullName, classType.Assembly)
        {
            _classType = classType;
            _mediator = mediator;
        }

        public override string GetString(string name, CultureInfo? culture)
        {
            return GetString(name);
        }

        public override string GetString(string name)
        {
            if (_translations.TryGetValue(name, out var value))
            {
                return value;
            }
            return base.GetString(name) ?? "";
        }

        private Dictionary<string, string> _translations = new Dictionary<string, string>();

        protected override async Task OnCultureChanged(string culture)
        {
            var typeName = _classType.AssemblyQualifiedName;
            var result = await _mediator.Send(new LanguageResource.Query
            {
                Language = culture,
                TypeName = typeName,
            });
            if (result.Success)
            {
                _translations = result.Result.Resource;
            }
            else
            {
                throw new InvalidOperationException("Can not load resource data");
            }
        }
    }
}