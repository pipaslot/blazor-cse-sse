using System;
using Core.Localization;
using Core.Mediator.Abstractions;

namespace App.Client.Services
{
    public class ResourceManagerFactory : IResourceManagerFactory
    {
        private readonly IMediator _mediator;

        public ResourceManagerFactory(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ResourceManagerWithCulture Create(Type resourceType)
        {
            return new ResourceManager(resourceType, _mediator);
        }
    }
}
