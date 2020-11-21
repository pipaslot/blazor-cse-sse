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

        public async Task<object> ExecuteQuery(CommandQueryContract commandQuery, CancellationToken cancellationToken)
        {
            var query = commandQuery.GetObject();

            var queryInterfaceType = typeof(IQuery<>);
            var resultType = query.GetType()
                .GetInterfaces()
                .FirstOrDefault(t=>t.GetGenericTypeDefinition() == queryInterfaceType)
                ?.GetGenericArguments()
                .FirstOrDefault();
            if (resultType == null)
            {
                throw new Exception($"Object {query.GetType()} is not assignable to type {queryInterfaceType }");
            }

            var method = _mediator.GetType()
                    .GetMethod(nameof(IMediator.Send))!
                .MakeGenericMethod(resultType);

            var task = (Task)method.Invoke(_mediator, new[] {query, cancellationToken})!;
            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty?.GetValue(task);
        }
        
        public async Task ExecuteCommand(CommandQueryContract commandQuery, CancellationToken cancellationToken)
        {
            var query = (ICommand)commandQuery.GetObject();
            await _mediator.Dispatch(query, cancellationToken);
        }
    }
}