namespace eComm.DOMAIN.Responses
{
    public class BaseResponse<T> where T : class
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; } = default!;
        public List<string>? Errors { get; set; }
    }
}
