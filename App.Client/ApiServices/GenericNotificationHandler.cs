﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using App.Shared;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace App.Client.ApiServices
{
    public class GenericNotificationHandler<TRequest> : INotificationHandler<TRequest> where TRequest : INotification
    {
        private readonly HttpClient _httpClient;

        public GenericNotificationHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Handle(TRequest request, CancellationToken cancellationToken)
        {
            await _httpClient.PostJsonAsync("api/mediator/notification?type="+typeof(TRequest).FullName, new RequestNotificationContract(request));
        }
    }
}