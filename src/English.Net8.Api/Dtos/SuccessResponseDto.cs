namespace English.Net8.Api.Dtos
{
    public class SuccessResponseDto
    {
        public bool Success { get; } = true;
    }

    public class SuccessResponseDto<T>
    {
        public SuccessResponseDto(T data)
        {
            Success = true;
            Data = data;
        }

        public bool Success { get; }
        public T Data { get; }
    }
}
