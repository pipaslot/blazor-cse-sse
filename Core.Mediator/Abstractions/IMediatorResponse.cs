namespace Core.Mediator.Abstractions
{
    public interface IMediatorResponse<TResult> : IMediatorResponse
    {
        TResult Result { get; }
    }

    public interface IMediatorResponse
    {
        bool Success { get; }
        string ErrorMessage { get; }
    }
}
