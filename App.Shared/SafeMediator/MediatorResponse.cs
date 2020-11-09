namespace App.Shared.SafeMediator
{
    public class MediatorResponse<TResult> : MediatorResponse
    {
        public MediatorResponse(TResult result)
        {
            Result = result;
        }

        public MediatorResponse(string errorMessage) : base(errorMessage)
        {
        }

        public TResult Result { get; }
    }

    public class MediatorResponse
    {
        public MediatorResponse()
        {
        }

        public MediatorResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public bool Success => string.IsNullOrWhiteSpace(ErrorMessage);
        public string ErrorMessage { get; }
    }
}