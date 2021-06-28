using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Action which does not return data. All derived types can have own specific pipelines and handlers.
    /// </summary>
    public interface IEvent
    {
    }
}
