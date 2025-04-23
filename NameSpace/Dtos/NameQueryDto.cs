namespace NameSpace.Dtos
{
    public class NameQueryDto
    {
        public required char Letter { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int? MinCount { get; set; }
        public int? MaxCount { get; set; }
    }
}
