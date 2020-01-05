using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Localization
{
    public class ResourceDefinitions : IReadOnlyCollection<Type>
    {
        private readonly List<Type> _types = new List<Type>();
        public ResourceDefinitions Register<TResource>()
        {
            _types.Add(typeof(TResource));
            return this;
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return _types.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _types.Count;
    }
}
