namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Marks pipeline as final/last wchich executes handlers. Any other pipeline wont be performed after this one.
    /// This interface was introduced to connect pipeline definitions and query handler existence check.
    /// </summary>
    public interface IExecutivePipeline
    {
        bool ExecuteMultipleHandlers { get; }
    }
}
