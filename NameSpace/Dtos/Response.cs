namespace NameSpace.Dtos
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? data { get; set; }
    }
}
