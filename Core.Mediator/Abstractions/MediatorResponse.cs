namespace Core.Mediator.Abstractions
{
    public class MediatorResponse<TResult> : MediatorResponse, IMediatorResponse<TResult>
    {
        /// <summary>
        /// Constructor for deserialization only
        /// </summary>
        public MediatorResponse()
        {
        }

        public MediatorResponse(TResult result)
        {
            Result = result;
        }

        public MediatorResponse(string errorMessage) : base(errorMessage)
        {
        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public TResult Result { get; set; } = default!;
    }

    public class MediatorResponse : IMediatorResponse
    {
        public MediatorResponse()
        {
        }

        public MediatorResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public bool Success => string.IsNullOrWhiteSpace(ErrorMessage);
        
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public string ErrorMessage { get; set; } = "";
    }
}