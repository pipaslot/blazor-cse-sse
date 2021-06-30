﻿using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator.Abstractions
{
    /// <summary>
    ///     Request / Event dispatched
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Execute action and wait for response data
        /// </summary>
        Task<IMediatorResponse<TResponse>> Execute<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Execute action without feedback
        /// </summary>
        Task<IMediatorResponse> Fire(IEvent @event, CancellationToken cancellationToken = default);
    }
}
