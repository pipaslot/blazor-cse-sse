using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Components.Resources
{
    public class ResourceCollection : IReadOnlyCollection<ResourceManagerWithCulture>
    {
        private readonly ResourceDefinitions _definitions;
        private readonly IResourceManagerFactory _managerFactory;
        private readonly List<ResourceManagerWithCulture> _managers = new List<ResourceManagerWithCulture>();

        public ResourceCollection(ResourceDefinitions definitions, IResourceManagerFactory managerFactory)
        {
            _definitions = definitions;
            _managerFactory = managerFactory;
        }
        
        public IEnumerator<ResourceManagerWithCulture> GetEnumerator()
        {
            if (_managers.Count == 0)
            {
                foreach (var definition in _definitions)
                {
                    var manager = _managerFactory.Create(definition);
                    _managers.Add(manager);
                    var fieldToHack = definition.GetField("resourceMan", BindingFlags.Static | BindingFlags.NonPublic);
                    fieldToHack.SetValue(null, manager);
                }
            }

            return _managers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _definitions.Count;

        public async Task SetCulture(string culture)
        {
            var tasks = this.Select(rm => rm.SetCulture(culture));
            await Task.WhenAll(tasks);
        }
    }
}
