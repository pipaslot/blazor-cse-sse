using System;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace Core.Localization
{
    public abstract class ResourceManagerWithCulture : ResourceManager
    {
        protected ResourceManagerWithCulture()
        {
        }

        protected ResourceManagerWithCulture(string baseName, Assembly assembly) : base(baseName, assembly)
        {
        }

        protected ResourceManagerWithCulture(string baseName, Assembly assembly, Type usingResourceSet) : base(baseName, assembly, usingResourceSet)
        {
        }

        protected ResourceManagerWithCulture(Type resourceSource) : base(resourceSource)
        {
        }

        private string _culture;
        private Task _cultureTask;
        public async Task SetCulture(string culture)
        {
            if (_culture != culture)
            {
                _culture = culture;
                _cultureTask = null;
            }

            if (_cultureTask == null)
            {
                _cultureTask = OnCultureChanged(culture);
            }
            await _cultureTask;
        }

        protected abstract Task OnCultureChanged(string culture);
    }
}
