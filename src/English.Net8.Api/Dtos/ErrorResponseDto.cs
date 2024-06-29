namespace English.Net8.Api.Dtos
{
    public class ErrorResponseDto
    {
        public ErrorResponseDto(IEnumerable<string> errors)
        {
            Success = false;
            Errors = errors;
        }

        public bool Success { get; }
        public IEnumerable<string> Errors { get; }
    }
}
