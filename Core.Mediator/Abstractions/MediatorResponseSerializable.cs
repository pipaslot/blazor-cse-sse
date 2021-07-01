namespace Core.Mediator.Abstractions
{
    public class MediatorResponseSerializable
    {
        public bool Success { get; set; }
        public object[] Results { get; set; } = new object[0];
        public string[] ErrorMessages { get; set; } = new string[0];
    }
}