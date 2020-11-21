﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator
{
    /// <summary>
    /// Server side executed receiving CommandQueryContract object through network connection
    /// </summary>
    public class CommandQueryContractExecutor
    {
        private readonly IMediator _mediator;

        public CommandQueryContractExecutor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<MediatorResponse> ExecuteQuery(CommandQueryContract commandQuery, CancellationToken cancellationToken)
        {
            var query = commandQuery.GetObject();

            var queryInterfaceType = typeof(IQuery<>);
            var resultType = query.GetType()
                .GetInterfaces()
                .FirstOrDefault(t => t.GetGenericTypeDefinition() == queryInterfaceType)
                ?.GetGenericArguments()
                .FirstOrDefault();
            if (resultType == null)
            {
                throw new Exception($"Object {query.GetType()} is not assignable to type {queryInterfaceType }");
            }

            var method = _mediator.GetType()
                    .GetMethod(nameof(IMediator.Send))!
                .MakeGenericMethod(resultType);
            try
            {
                var task = (Task)method.Invoke(_mediator, new[] { query, cancellationToken })!;
                await task.ConfigureAwait(false);

                var resultProperty = task.GetType().GetProperty("Result");
                return (MediatorResponse)resultProperty?.GetValue(task);
            }
            catch (Exception e)
            {
                return new MediatorResponse(e.Message);
            }
        }

        public async Task<MediatorResponse> ExecuteCommand(CommandQueryContract commandQuery, CancellationToken cancellationToken)
        {
            try
            {
                var query = (ICommand)commandQuery.GetObject();
                return await _mediator.Dispatch(query, cancellationToken);
            }
            catch (Exception e)
            {
                return new MediatorResponse(e.Message);
            }
        }
    }
}
