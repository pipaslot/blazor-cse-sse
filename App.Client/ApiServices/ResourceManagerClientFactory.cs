using System;
using Core.Localization;
using Core.Mediator.Abstractions;

namespace App.Client.ApiServices
{
    public class ResourceManagerClientFactory : IResourceManagerFactory
    {
        private readonly IMediator _mediator;

        public ResourceManagerClientFactory(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ResourceManagerWithCulture Create(Type resourceType)
        {
            return new ResourceManagerClient(resourceType, _mediator);
        }
    }
}
