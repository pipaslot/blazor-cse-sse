using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Components.Resources;

namespace App.Server.Services
{
    public class ResourceManagerServerFactory : IResourceManagerFactory
    {
        public ResourceManagerWithCulture Create(Type resourceType)
        {
            return new ResourceManagerServer(resourceType);
        }
    }

    public class ResourceManagerServer : ResourceManagerWithCulture
    {
        public ResourceManagerServer(Type classType) : base(classType.FullName, classType.Assembly)
        {
        }

        private CultureInfo _culture;
        protected override Task OnCultureChanged(string culture)
        {
            _culture = new CultureInfo(culture);
            return Task.CompletedTask;
        }

        public override string GetString(string name, CultureInfo culture)
        {
            return GetString(name);
        }

        public override string GetString(string name)
        {
            return base.GetString(name, _culture);
        }
    }
}
