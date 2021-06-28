using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Base action marker. All derived types can have own specific pipelines and handlers. No reponse is expected.
    /// </summary>
    public interface IEvent
    {
    }
}
