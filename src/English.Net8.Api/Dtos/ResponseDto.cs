namespace English.Net8.Api.Dtos
{
    public class ResponseDto
    {
        public bool Success { get; set; }
        public object? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
