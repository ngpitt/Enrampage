namespace Enrampage.Models
{
    public class ApiResponse
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public object Payload { get; private set; }

        public ApiResponse(bool Success, string Message, object Payload)
        {
            this.Success = Success;
            this.Message = Message;
            this.Payload = Payload;
        }
    }
}