namespace ChatApp.Application.Responses
{
    public class BaseCommonResponse
    {
        public int Id { get; set; }
        public bool IsSuccess { get; set; } = false;
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public object? Data { get; set; }
    }
}
